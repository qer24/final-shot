using System.Collections.Generic;
using UnityEngine;

namespace CoopGame.PlayerMovement
{
    public class MovementHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] CharacterController controller = null;

        private readonly List<IMovementModifier> modifiers = new List<IMovementModifier>();

        void Update() => Move();

        public void AddModifier(IMovementModifier modifier) => modifiers.Add(modifier);

        public void RemoveModifier(IMovementModifier modifier) => modifiers.Remove(modifier);

        void Move()
        {
            Vector3 movement = Vector3.zero;

            foreach (var modifier in modifiers)
            {
                movement += modifier.Value;
            }

            controller.Move(movement);
        }
    }
}
