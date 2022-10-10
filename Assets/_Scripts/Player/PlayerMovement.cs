using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform forwardSource;
    [SerializeField] private CharacterController characterController;

    [Header("Locomotion Settings")]
    [SerializeField] private float walkSpeed;

    // Inputs
    MainInputProfile inputs;
    Vector2 moveInput;

    private void Awake()
    {
        InputInitialization();
    }

    private void InputInitialization()
    {
        inputs = new MainInputProfile();
        inputs.Main.Enable();

        inputs.Main.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputs.Main.Movement.canceled += ctx => moveInput = Vector2.zero;
    }

    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        Vector3 moveDir = forwardSource.right * moveInput.x + forwardSource.forward * moveInput.y;
        characterController.Move(moveDir.normalized * walkSpeed * Time.deltaTime);
    }
}
