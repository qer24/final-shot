using UnityEngine;

namespace CoopGame.PlayerMovement
{
    public class ForceReceiver : MonoBehaviour, IMovementModifier
    {
        [Header("References")]
        [SerializeField] MovementHandler movementHandler = null;

        [Header("Settings")]
        [SerializeField] float mass = 1f;
        [SerializeField] float drag = 5f;

        public Vector3 Value { get; private set; }

        void OnEnable() => movementHandler.AddModifier(this);

        void OnDisable() => movementHandler.RemoveModifier(this);

        void Update()
        {
            if (Value.magnitude < 0.2f)
            {
                Value = Vector3.zero;
                return;
            }

            Value = Vector3.Lerp(Value, Vector3.zero, drag * Time.deltaTime);
        }

        public void AddForce(Vector3 force) => Value += force / mass;
    }
}
