using System.Collections.Generic;
using UnityEngine;

public class Rope
{
    public struct RopeSegment
    {
        public Vector3 PosNow;
        public Vector3 PosOld;

        public RopeSegment(Vector3 pos)
        {
            PosNow = pos;
            PosOld = pos;
        }
    }

    public Transform StartPoint { get; set; }
    public Transform EndPoint { get; set; }

    public bool IsActive { get; private set; }
    public float RopeSegLen { get; set; }

    private List<RopeSegment> _ropeSegments = new List<RopeSegment>();
    private int _segmentsCount = 35;
    private AnimationCurve _weightCurve;
    private Transform[] _ropeBones;
    private float _groundPlane;



    /// <summary> Initialize a rope with two fixed connections </summary>
    public void InitRope(Transform startPoint, Transform endPoint, int segmentsCount, float segmentLength, float groundPlane)
    {
        StartPoint = startPoint;
        EndPoint = endPoint;

        Vector3 ropeStartPoint = StartPoint.position;

        _segmentsCount = segmentsCount;
        RopeSegLen = segmentLength;
        _groundPlane = groundPlane;

        for (int i = 0; i < segmentsCount; i++)
        {
            _ropeSegments.Add(new RopeSegment(ropeStartPoint));
        }

        IsActive = true;
    }

    /// <summary> Initialize a rope with one open end </summary>
    public void InitRope(Transform startPoint, int segmentsCount, float segmentLength)
    {
        StartPoint = startPoint;

        Vector3 ropeStartPoint = StartPoint.position;
        _segmentsCount = segmentsCount;
        RopeSegLen = segmentLength;

        for (int i = 0; i < segmentsCount; i++)
        {
            _ropeSegments.Add(new RopeSegment(ropeStartPoint));
        }

        IsActive = true;
    }

    /// <summary> For using with clothes (not present in this project) </summary>
    public void SetBones(Transform[] bones, AnimationCurve curve)
    {
        _ropeBones = bones;
        _weightCurve = curve;    
    /// <summary> Simulate externally every frame</summary>
    /// <summary> Use this with a LineRenderer to draw rope</summary>
    }

    /// <summary> Simulate externally every frame</summary>
    public void Simulate()
    {
        Vector3 forceGravity = new Vector3(0f, -1f);

        for (int i = 1; i < _segmentsCount; i++)
        {
            RopeSegment firstSegment = _ropeSegments[i];
            Vector3 velocity = firstSegment.PosNow - firstSegment.PosOld;
            firstSegment.PosOld = firstSegment.PosNow;
            firstSegment.PosNow += velocity;
            firstSegment.PosNow += forceGravity * Time.fixedDeltaTime;
            _ropeSegments[i] = firstSegment;
        }

        //CONSTRAINTS
        for (int i = 0; i < 50; i++)
        {
            ApplyConstraint();
        }
    }

    /// <summary> Use this with a LineRenderer to draw rope</summary>
    public Vector3[] GetRopePositions()
    {
        Vector3[] ropePositions = new Vector3[_segmentsCount];
        for (int i = 0; i < _segmentsCount; i++)
        {
            ropePositions[i] = _ropeSegments[i].PosNow;
        }

        return ropePositions;
    }

    private void ApplyConstraint()
    {
        //Constrant to First Point 
        RopeSegment firstSegment = _ropeSegments[0];
        firstSegment.PosNow = StartPoint.position;
        _ropeSegments[0] = firstSegment;

        //Constrant to Second Point 
        if(EndPoint != null)
        {
            RopeSegment endSegment = _ropeSegments[_ropeSegments.Count - 1];
            endSegment.PosNow = EndPoint.position;
            _ropeSegments[_ropeSegments.Count - 1] = endSegment;
        }

        for (int i = 0; i < _segmentsCount - 1; i++)
        {
            RopeSegment firstSeg = _ropeSegments[i];
            RopeSegment secondSeg = _ropeSegments[i + 1];

            float dist = (firstSeg.PosNow - secondSeg.PosNow).magnitude;
            float error = Mathf.Abs(dist - RopeSegLen);
            Vector3 changeDir = Vector3.zero;

            if (dist > RopeSegLen)
            {
                changeDir = (firstSeg.PosNow - secondSeg.PosNow).normalized;
            }
            else if (dist < RopeSegLen)
            {
                changeDir = (secondSeg.PosNow - firstSeg.PosNow).normalized;
            }

            Vector3 changeAmount = changeDir * error;

            if (i != 0)
            {
                if(_ropeBones != null && i < _ropeBones.Length)
                {
                    float boneWeight = _weightCurve.Evaluate((float)i / _ropeBones.Length);
                    Vector3 position = Vector3.Lerp(firstSeg.PosNow + -changeAmount * 0.5f, _ropeBones[i].position, boneWeight);

                    position.y = Mathf.Clamp(position.y, _groundPlane, float.MaxValue);

                    firstSeg.PosNow = position;
                    secondSeg.PosNow += changeAmount * 0.5f;
                    secondSeg.PosNow.y = Mathf.Clamp(secondSeg.PosNow.y, _groundPlane, float.MaxValue);

                    _ropeSegments[i] = firstSeg;
                    _ropeSegments[i + 1] = secondSeg;
                }
                else
                {
                    firstSeg.PosNow -= changeAmount * 0.5f;
                    secondSeg.PosNow += changeAmount * 0.5f;

                    firstSeg.PosNow.y = Mathf.Clamp(firstSeg.PosNow.y, _groundPlane, float.MaxValue);
                    secondSeg.PosNow.y = Mathf.Clamp(secondSeg.PosNow.y, _groundPlane, float.MaxValue);

                    _ropeSegments[i] = firstSeg;
                    _ropeSegments[i + 1] = secondSeg;
                }
            }
            else
            {
                secondSeg.PosNow += changeAmount;
                secondSeg.PosNow.y = Mathf.Clamp(secondSeg.PosNow.y, _groundPlane, float.MaxValue);

                _ropeSegments[i + 1] = secondSeg;
            }
        }
    }
}