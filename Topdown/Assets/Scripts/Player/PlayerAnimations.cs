
using System.Collections.Generic;
using UnityEngine;

namespace CoopGame.PlayerMovement
{
    public class PlayerAnimations : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] CharacterController controller = null;
        [SerializeField] MovementInputProcessor inputProcessor = null;
        [SerializeField] Animator anim = null;
        [SerializeField] Transform modelTransform = null;

        Vector3 convertedInputDir;
        Transform mainCameraTransform;

        void Start() => mainCameraTransform = Camera.main.transform;

        void LateUpdate()
        {
            float horizontal = controller.isGrounded ? inputProcessor.previousVelocity.magnitude : 0f;
            anim.SetFloat("Horizontal", horizontal);

            Vector3 right = mainCameraTransform.right;
            Vector3 forward = mainCameraTransform.forward;

            right.y = 0f;
            forward.y = 0f;

            if (inputProcessor.previousInputDirection.magnitude > 0.1f)
            {
                convertedInputDir = right * inputProcessor.previousInputDirection.x + forward * inputProcessor.previousInputDirection.y;
            }

            Quaternion rot = Quaternion.identity;
            if (convertedInputDir.magnitude != 0)
                rot = Quaternion.LookRotation(convertedInputDir, Vector3.up);

            modelTransform.localRotation = Quaternion.Slerp(modelTransform.localRotation, rot, Time.deltaTime * 10f);
        }
    }
}
