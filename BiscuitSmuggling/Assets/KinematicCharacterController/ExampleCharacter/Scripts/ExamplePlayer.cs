using UnityEngine;

namespace KinematicCharacterController.Examples
{
    public class ExamplePlayer : MonoBehaviour
    {
        public ExampleCharacterController Character;

        [SerializeField]
        public bool _lockCursor = true;

        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";

        private void Start()
        {

            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            /*if (_lockCursor)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }*/

            HandleCharacterInput();
        }

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new()
            {
                // Build the CharacterInputs struct
                MoveAxisForward = Input.GetAxisRaw(VerticalInput),
                MoveAxisRight = Input.GetAxisRaw(HorizontalInput),
                CameraRotation = Quaternion.identity, // No camera rotation
                CrouchDown = Input.GetKeyDown(KeyCode.C),
                CrouchUp = Input.GetKeyUp(KeyCode.C)
            };

            // Apply inputs to character
            Character.SetInputs(ref characterInputs);
        }

        public void ChangeLockCursor(bool newState)
        {
            _lockCursor = newState;
        }
    }
}