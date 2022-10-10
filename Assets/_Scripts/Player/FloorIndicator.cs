using UnityEngine;

public class FloorIndicator : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform indicatorOffset;
    [Space]
    [SerializeField] private LayerMask groundLayer;

    Vector3 worldPositionHit = Vector3.zero;

    // Inputs
    MainInputProfile inputs;
    Vector2 mousePositionInput;

    private void Awake()
    {
        InputInitialization();
    }

    private void InputInitialization()
    {
        inputs = new MainInputProfile();
        inputs.Main.Enable();

        inputs.Main.MousePosition.performed += ctx => mousePositionInput = ctx.ReadValue<Vector2>();
    }

    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(mousePositionInput);

        if (Physics.Raycast(ray, out RaycastHit hitData, groundLayer))
            worldPositionHit = hitData.point;

        indicatorOffset.LookAt(worldPositionHit, Vector3.up);
        indicatorOffset.rotation = Quaternion.AngleAxis(indicatorOffset.eulerAngles.y, Vector3.up);
    }
}
