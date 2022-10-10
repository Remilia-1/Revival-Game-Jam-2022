using UnityEngine;

public class RopeObject : MonoBehaviour
{
    [SerializeField] private Transform _attachTransformA;
    [SerializeField] private Transform _attachTransformB;
    [SerializeField] private LineRenderer _ropeRenderer;

    [SerializeField] private int _segmentCount;

    [SerializeField] private float _segmentLengthByDistance;
    [SerializeField] private float _segmentLengthMin;
    [SerializeField] private float _segmentLengthMax;

    [SerializeField] private float _initialGroundPlane;

    [SerializeField] private bool _connectBothPoints = true;

    private Rope _rope;



    public void SetGroundPlane(float groundPlane)
    {
        _rope = new Rope();
        if(_connectBothPoints)
            _rope.InitRope(_attachTransformA, _attachTransformB, _segmentCount, _segmentLengthMin, groundPlane);
        else 
            _rope.InitRope(_attachTransformA, _segmentCount, _segmentLengthMin, groundPlane);
    }

    [ContextMenu("REINIT")]
    private void Awake()
    {
        _rope = new Rope();
        if (_connectBothPoints)
            _rope.InitRope(_attachTransformA, _attachTransformB, _segmentCount, _segmentLengthMin, _initialGroundPlane);
        else
            _rope.InitRope(_attachTransformA, _segmentCount, _segmentLengthMin, _initialGroundPlane);

        _ropeRenderer.positionCount = _segmentCount;
    }

    private void FixedUpdate()
    {
        _rope.Simulate();
    }

    private void Update()
    {
        var ropePoints = _rope.GetRopePositions();

        _ropeRenderer.SetPositions(ropePoints);

        UpdateSegmentLength();
    }

    private void UpdateSegmentLength()
    {
        if (_attachTransformB == null)
            return;

        var dist = Vector3.Distance(_attachTransformA.position, _attachTransformB.position);
        var dMult = dist * _segmentLengthByDistance;

        var resultLength = Mathf.Clamp(dMult, _segmentLengthMin, _segmentLengthMax);

        _rope.RopeSegLen = resultLength;
    }
}
