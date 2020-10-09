using UnityEngine;

namespace CoopGame.PlayerMovement
{
    public class PlayerInput : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] MovementInputProcessor inputProcessor = null;

        void Update()
        {
            Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            Vector2.ClampMagnitude(inputVector, 1f);

            inputProcessor.SetMovementInput(inputVector);
        }
    }
}
 