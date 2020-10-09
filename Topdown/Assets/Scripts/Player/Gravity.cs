using UnityEngine;

namespace CoopGame.PlayerMovement
{
    public class Gravity : MonoBehaviour, IMovementModifier
    {
        [Header("References")]
        [SerializeField] CharacterController controller = null;
        [SerializeField] MovementHandler movementHandler = null;

        [Header("Settings")]
        [SerializeField] float groundedPullMagnitude = 5f;

        [SerializeField] float gravityMagnitude = Physics.gravity.y / 10;

        bool wasGroundedLastFrame;

        public Vector3 Value { get; private set; }

        void OnEnable() => movementHandler.AddModifier(this);

        void OnDisable() => movementHandler.RemoveModifier(this);

        void Update()
        {
            ProcessGravity();
        }

        private void ProcessGravity()
        {
            if (controller.isGrounded)
            {
                Value = new Vector3(Value.x, -groundedPullMagnitude, Value.z);
            }
            else if (wasGroundedLastFrame)
            {
                Value = Vector3.zero;
            }
            else
            {
                Value = new Vector3(Value.x, Value.y + gravityMagnitude * Time.deltaTime, Value.z);
            }

            wasGroundedLastFrame = controller.isGrounded;
        }
    }
}
