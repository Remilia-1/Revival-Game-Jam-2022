using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform forwardSource;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform characterModel;

    [Header("Locomotion Settings")]
    [SerializeField] private float walkSpeed;

    [Header("Character Model Settings")]
    [SerializeField] private float rotationOffset = -180f;
    Quaternion modelLookRotation;

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
        CharacterRotation();
    }

    private void Movement()
    {
        Vector3 moveDir = forwardSource.right * moveInput.x + forwardSource.forward * moveInput.y;
        characterController.Move(moveDir.normalized * walkSpeed * Time.deltaTime);
    }

    private void CharacterRotation()
    {
        Vector3 lookRotation = new Vector3(moveInput.x, 0, moveInput.y);

        if(lookRotation.magnitude >= 0.08f)
            modelLookRotation = Quaternion.LookRotation(lookRotation, Vector3.up) * Quaternion.AngleAxis(rotationOffset, Vector3.up);

        characterModel.transform.rotation = modelLookRotation;
    }
}
