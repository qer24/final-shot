using UnityEngine;

namespace CoopGame.PlayerMovement
{
    public class MovementInputProcessor : MonoBehaviour, IMovementModifier
    {
        [Header("References")]
        [SerializeField] CharacterController controller = null;
        [SerializeField] MovementHandler movementHandler = null;

        [Header("Settings")]
        [SerializeField] float movementSpeed = 5f;

        float currentSpeed = 0f;

        [HideInInspector] public Vector3 previousVelocity;
        [HideInInspector] public Vector2 previousInputDirection;

        Transform mainCameraTransform;

        public Vector3 Value { get; private set; }

        void OnEnable() => movementHandler.AddModifier(this);

        void OnDisable() => movementHandler.RemoveModifier(this);

        void Update() => Move();

        void Start() => mainCameraTransform = Camera.main.transform;

        public void SetMovementInput(Vector2 dir) => previousInputDirection = dir;

        void Move()
        {
            float targetSpeed = movementSpeed * Mathf.Clamp(previousInputDirection.magnitude, -1, 1) * Time.deltaTime;
            currentSpeed = targetSpeed;

            Vector3 right = mainCameraTransform.right;
            Vector3 forward = mainCameraTransform.forward;

            right.y = 0f;
            forward.y = 0f;

            Vector3 moveDir;

            if (targetSpeed != 0f)
            {   
                moveDir = right * previousInputDirection.x + forward * previousInputDirection.y;
            }
            else
            {
                moveDir = previousVelocity.normalized;
            }

            Value = moveDir * currentSpeed;

            previousVelocity = new Vector3(controller.velocity.x, 0f, controller.velocity.z);
        }
    }
}
