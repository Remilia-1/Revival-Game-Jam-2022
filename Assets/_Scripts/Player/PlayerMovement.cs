using System.Threading.Tasks;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform forwardSource;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform characterModel;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed;
    private bool movementDisabled = false;
    [SerializeField] private float gravity = -9.81f;
    private float currentGravity = 0f;

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
        if (!movementDisabled)
        {
            Gravity();
            Movement();
        }

        if(!rotationOverride)
            CharacterRotation();
    }

    private void Movement()
    {
        Vector3 moveDir = forwardSource.right * moveInput.x + forwardSource.forward * moveInput.y;
        characterController.Move(moveDir.normalized * walkSpeed * Time.deltaTime);
    }

    private void Gravity()
    {
        currentGravity += gravity * Time.deltaTime;
        characterController.Move(Vector3.up * currentGravity);
    }

    private void CharacterRotation()
    {
        Vector3 lookRotation = new Vector3(moveInput.x, 0, moveInput.y);

        if(lookRotation.magnitude >= 0.08f)
            modelLookRotation = Quaternion.LookRotation(lookRotation, Vector3.up) * Quaternion.AngleAxis(rotationOffset, Vector3.up);

        characterModel.transform.rotation = modelLookRotation;
    }

    // Classes that are needed for the Player Combat to work
    public async void CharacterRotationOverride(float amount, int timeToResetOverrideMsec)
    {
        rotationOverride = true;
        characterModel.transform.rotation = Quaternion.AngleAxis(amount - overrideRotationOffset, Vector3.up);
        await Task.Delay(timeToResetOverrideMsec);
        rotationOverride = false;
    }

    public void MoveCharacterToPosition(System.Action callback, Vector3 target, float speed)
    {
        StopAllCoroutines();
        StartCoroutine(MoveCharacterToPositionRoutine(callback, target, speed));
    }

    private IEnumerator MoveCharacterToPositionRoutine(System.Action callback, Vector3 target, float speed)
    {
        movementDisabled = true;

        Transform transf = characterController.transform;
        float timeEllapsed = 0f;

        while ((transf.position - target).magnitude > 0.08f)
        {
            transf.position = Vector3.Lerp(transf.position, target, timeEllapsed * (speed * 0.1f));
            timeEllapsed += Time.deltaTime;

            yield return null;
        }

        movementDisabled = false;

        callback.Invoke();
    }
}