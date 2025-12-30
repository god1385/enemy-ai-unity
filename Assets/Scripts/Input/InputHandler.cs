using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class InputHandler : MonoBehaviour
    {
        private InputAction moveAction;
        private InputAction lookAction;
        static private Vector2 moveDirection;
        static private Vector2 lookDirection;

        public static Vector2 MoveDirection => moveDirection;
        public static Vector2 LookDirection => lookDirection;

        private void Start()
        {
            moveAction = InputSystem.actions.FindAction("Move");
            lookAction = InputSystem.actions.FindAction("Look");
        }
        private void FixedUpdate()
        {
            moveDirection = moveAction.ReadValue<Vector2>();
            lookDirection = lookAction.ReadValue<Vector2>();
        }
    }
}
