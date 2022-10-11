using System.Threading.Tasks;
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
    [SerializeField] private float rotationOffset = 135;
    [SerializeField] private float overrideRotationOffset = 180f;

    Quaternion modelLookRotation;
    bool rotationOverride;

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

        if(!rotationOverride)
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

    #region Character Rotation Override
    public void CharacterRotationOverride(float amount)
    {
        rotationOverride = true;
        characterModel.transform.rotation = Quaternion.AngleAxis(amount - overrideRotationOffset, Vector3.up);
    }

    public async void CharacterRotationOverride(float amount, int timeToResetOverrideMsec)
    {
        rotationOverride = true;
        characterModel.transform.rotation = Quaternion.AngleAxis(amount - overrideRotationOffset, Vector3.up);
        await Task.Delay(timeToResetOverrideMsec);
        rotationOverride = false;
    }

    public void StopCharacterRotationOverride() { rotationOverride = false; }
    #endregion
}
