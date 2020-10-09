using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;



public class AORenderer : ScriptableRendererFeature {

    public enum SampleCount {Two = 2,Four = 4, Eight = 8, Sixteen = 16}

    public class AOPass : ScriptableRenderPass {
        private string tag;

        private RenderTargetHandle screenCopyHandle, frontHandle, backHandle;

        private List<ShaderTagId> shaderTags = new List<ShaderTagId>();

        private bool hasFlippedBuffers = false;

        private RenderTargetHandle ActiveReadTarget {
            get {
                return hasFlippedBuffers ? frontHandle : backHandle;
            }
        }
        private RenderTargetHandle ActiveWriteTarget
        {
            get
            {
                return hasFlippedBuffers ? backHandle : frontHandle;
            }
        }

        private Material horizontalBlurMaterial;
        private Material HorizontalBlurMaterial
        {
            get
            {
                if (horizontalBlurMaterial == null)
                {
                    horizontalBlurMaterial = new Material(Shader.Find("Hidden/Custom/AO/BoxBlur"));
                    horizontalBlurMaterial.SetVector("sampleDirection", Vector2.right);
                }
                return horizontalBlurMaterial;
            }
        }
        private Material verticalBlurMaterial;
        private Material VerticalBlurMaterial
        {
            get
            {
                if (verticalBlurMaterial == null)
                {
                    verticalBlurMaterial = new Material(Shader.Find("Hidden/Custom/AO/BoxBlur"));
                    verticalBlurMaterial.SetVector("sampleDirection", Vector2.up);
                }
                return verticalBlurMaterial;
            }
        }

        private Material aoMaterial;
        private Material AOMaterial {
            get {
                if (aoMaterial == null) {
                    aoMaterial = new Material(Shader.Find("Hidden/Custom/AO/Calculate"));
                }
                return aoMaterial;
            }
        }

        private Material compositMaterial;
        private Material CompositMaterial {
            get {
                if (compositMaterial == null) {
                    compositMaterial = new Material(Shader.Find("Hidden/Custom/AO/Compositor"));
                }
                return compositMaterial;
            }
        }

        public RenderTargetIdentifier Target { get; set;}

        public bool EnableComposit { get; set; } = true;
        public Color Colour{
            set{
                CompositMaterial.SetColor("colour", value);
            }
        }
        public float Multiplier {
            set {
                CompositMaterial.SetFloat("multiplier", value);
            }
        }
        public float Power {
            set {
                CompositMaterial.SetFloat("power", value);
            }
        }

        public float RenderScale { get; set; }
        public int SampleCount{
            set{
                AOMaterial.SetInt("sampleCount", value);
            }
        }
        public float SampleRange {
            set {
                AOMaterial.SetFloat("sampleRange", value);
            }
        }
        public bool InvertNormal {
            set {
                if (value)
                {
                    AOMaterial.EnableKeyword("FLIPAONORMAL");
                }
                else {
                    AOMaterial.DisableKeyword("FLIPAONORMAL");
                }
            }    
        }

 
        public AOPass(string name) {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
            this.tag = name;

            shaderTags.Add(new ShaderTagId("UniversalForward"));
            shaderTags.Add(new ShaderTagId("Unlit"));

            screenCopyHandle.Init(name + "_ScreenCopy");
            frontHandle.Init(name + "_FrontBuffer");
            backHandle.Init(name + "_BackBuffer");

            hasFlippedBuffers = false;
        }

        public override void Configure(CommandBuffer commandBuffer,RenderTextureDescriptor cameraTextureDescriptor) {
            commandBuffer.GetTemporaryRT(screenCopyHandle.id,cameraTextureDescriptor,FilterMode.Point);

            cameraTextureDescriptor.colorFormat = RenderTextureFormat.RHalf;
            cameraTextureDescriptor.width = Mathf.FloorToInt(cameraTextureDescriptor.width * RenderScale);
            cameraTextureDescriptor.height = Mathf.FloorToInt(cameraTextureDescriptor.height * RenderScale);    
            commandBuffer.GetTemporaryRT(frontHandle.id, cameraTextureDescriptor, FilterMode.Bilinear);
            commandBuffer.GetTemporaryRT(backHandle.id, cameraTextureDescriptor, FilterMode.Bilinear);
            commandBuffer.SetGlobalVector("RenderTextureParams", new Vector4(cameraTextureDescriptor.width, cameraTextureDescriptor.height, 1/(float)cameraTextureDescriptor.width, 1 / (float)cameraTextureDescriptor.height));
        }

        public override void Execute(ScriptableRenderContext context,ref RenderingData renderingData) {
            CommandBuffer commandBuffer = CommandBufferPool.Get(tag);

            commandBuffer.Blit(Target,screenCopyHandle.id);

            //Generate AO map.
            var camera = renderingData.cameraData.camera;
            AOMaterial.SetMatrix("viewMatrix", camera.worldToCameraMatrix);
            AOMaterial.SetMatrix("inverseViewMatrix", Matrix4x4.Inverse(camera.worldToCameraMatrix));
            AOMaterial.SetMatrix("projectionMatrix", GL.GetGPUProjectionMatrix(camera.projectionMatrix, false));
            AOMaterial.SetMatrix("inverseProjectionMatrix", GL.GetGPUProjectionMatrix(camera.projectionMatrix, false).inverse);


            commandBuffer.Blit(Target, ActiveWriteTarget.id, AOMaterial);
            FlipBuffers();

            //Blur.
            commandBuffer.Blit(ActiveReadTarget.id, ActiveWriteTarget.id, HorizontalBlurMaterial);
            FlipBuffers();
            commandBuffer.Blit(ActiveReadTarget.id, ActiveWriteTarget.id, VerticalBlurMaterial);
            FlipBuffers();


            //Composite.
            if (EnableComposit)
            {
                commandBuffer.SetGlobalTexture("_MainTex", screenCopyHandle.id);
                commandBuffer.SetGlobalTexture("AOTexture", ActiveReadTarget.id);
                commandBuffer.Blit(screenCopyHandle.id, Target, CompositMaterial);
            }

            context.ExecuteCommandBuffer(commandBuffer);


            CommandBufferPool.Release(commandBuffer);

        }

        public override void FrameCleanup(CommandBuffer commandBuffer) {
            commandBuffer.ReleaseTemporaryRT(screenCopyHandle.id);
            commandBuffer.ReleaseTemporaryRT(frontHandle.id);
            commandBuffer.ReleaseTemporaryRT(backHandle.id);
        }

        public void FlipBuffers() {
            hasFlippedBuffers = !hasFlippedBuffers;
        }
    }
    [Header("Generation settings")]
    [Range(0.1f,1)]
    public float renderScale = 0.25f;
    public SampleCount sampleCount = SampleCount.Four;
    [Range(0,1)]
    public float sampleRange = 0.5f;
    public bool invert = false;


    [Header("Composit Settings.")]
    public bool enableComposit = true;
    [ColorUsage(false)]
    public Color colour;
    public float multiplier = 1;
    [Range(0,2)]
    public float power = 0.75f;

    AOPass pass;
    GBufferPass GBufferPass;
    public override void Create() {
        pass = new AOPass(name);
        GBufferPass = new GBufferPass();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer,ref RenderingData renderingData) {
        pass.Target = renderer.cameraColorTarget;

        pass.RenderScale = renderScale;
        pass.SampleCount = (int)sampleCount;
        pass.SampleRange = sampleRange;
        pass.InvertNormal = invert;

        pass.EnableComposit = enableComposit;
        pass.Colour = colour;
        pass.Multiplier = multiplier;
        pass.Power = power;

        renderer.EnqueuePass(GBufferPass);
        renderer.EnqueuePass(pass);
    }
}
