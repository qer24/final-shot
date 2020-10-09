using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public class GBufferPass : ScriptableRenderPass {
    //Prevent GBuffer being calculated if another pass has already calculated it.
    private bool hasRenderedThisFrame;
    
    private RenderTargetHandle depthNormalHandle;
    private RenderTargetHandle screenCopyHandle;
    private FilteringSettings filteringSettings;

    private List<ShaderTagId> shaderTags = new List<ShaderTagId>();
    private float cameraDepth = 1000;
    public static string DepthNormalName => "faceDepthNormalTexture";
    private string Tag => "RebuildGBuffer";

    private Material depthNormalMaterial;
    private Material DepthNormalMaterial {
        get {
            if(depthNormalMaterial == null) {
                depthNormalMaterial = new Material(Shader.Find("Hidden/Custom/Deferred/DepthNormal"));
            }

            return depthNormalMaterial;
        }
    }

    public GBufferPass() {
        renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
        filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

        shaderTags.Add(new ShaderTagId("UniversalForward"));
        shaderTags.Add(new ShaderTagId("Unlit"));

        depthNormalHandle.Init(DepthNormalName);

    }

    public override void Configure(CommandBuffer cmd,RenderTextureDescriptor cameraTextureDescriptor) {
        if (hasRenderedThisFrame) {
            return;
        }
        cameraTextureDescriptor.colorFormat = RenderTextureFormat.ARGBFloat;
        cmd.GetTemporaryRT(depthNormalHandle.id,cameraTextureDescriptor,FilterMode.Bilinear);

        ConfigureTarget(depthNormalHandle.Identifier());
        ConfigureClear(ClearFlag.All,new Color(0,0,0,cameraDepth));
    }

    public override void Execute(ScriptableRenderContext context,ref RenderingData renderingData) {
        if (hasRenderedThisFrame) {
            return;
        }
        hasRenderedThisFrame = true;
        CommandBuffer commandBuffer = CommandBufferPool.Get(Tag);

        cameraDepth = renderingData.cameraData.camera.farClipPlane;
        DrawSceneWithMaterial(commandBuffer,ref context,ref renderingData);
        commandBuffer.SetGlobalTexture(DepthNormalName,depthNormalHandle.id);
        context.ExecuteCommandBuffer(commandBuffer);

        CommandBufferPool.Release(commandBuffer);

    }

    public override void FrameCleanup(CommandBuffer commandBuffer) {
        commandBuffer.ReleaseTemporaryRT(depthNormalHandle.id);
        hasRenderedThisFrame = false;
    }

    private void DrawSceneWithMaterial(CommandBuffer commandBuffer,ref ScriptableRenderContext context,ref RenderingData renderingData) {
        commandBuffer.SetViewProjectionMatrices(renderingData.cameraData.camera.worldToCameraMatrix,renderingData.cameraData.camera.projectionMatrix);
        context.ExecuteCommandBuffer(commandBuffer);
        var drawSettings = CreateDrawingSettings(shaderTags,ref renderingData,SortingCriteria.CommonOpaque);
        drawSettings.overrideMaterial = DepthNormalMaterial;
        context.DrawRenderers(renderingData.cullResults,ref drawSettings,ref filteringSettings);
    }
}
