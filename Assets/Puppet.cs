using UnityEngine;
using Klak.Math;

public class Puppet : MonoBehaviour
{
    [SerializeField] float _stepFrequency = 2;
    [SerializeField] float _stride = 0.4f;
    [SerializeField] float _stepHeight = 0.3f;
    [SerializeField] float _stepAngle = 90;
    [SerializeField] float _bodyHeight = 0.9f;
    [SerializeField] float _bodyUndulation = 10;
    [SerializeField] Vector3 _handPosition = new Vector3(0.3f, 0.3f, -0.2f);
    [SerializeField] float _handMove = 0.2f;
    [SerializeField] float _headMove = 3;
    [SerializeField] float _noiseFrequency = 1.1f;
    [SerializeField] int _randomSeed = 123;

    float SmoothStep(float x)
    {
        return x * x * (3 - 2 * x);
    }

    int StepCount { get { return Mathf.FloorToInt(_stepTime); } }
    int StepSeed { get { return StepCount * 100; } }
    float StepFracTime { get { return _stepTime - Mathf.Floor(_stepTime); } }
    float StepAngle { get { return _hash.Range(0.5f, 1.0f, StepSeed) * _stepAngle * (_hash.Value01(StepSeed + 1) > 0.5f ? 1 : -1); } }
    bool PivotIsLeft { get { return (StepCount & 1) == 0; } }

    Quaternion StepRotation {
        get { return Quaternion.AngleAxis(StepAngle, Vector3.up); }
    }

    Quaternion StepFracRotation {
        get { return Quaternion.AngleAxis(StepAngle * StepFracTime, Vector3.up); }
    }

    Vector3 FootBias { get { return Vector3.up * _animator.leftFeetBottomHeight; } }

    Vector3 LeftFootPos {
        get {
            if (PivotIsLeft) return _feet[0] + FootBias;
            var pos = _feet[1] + StepFracRotation * (_feet[0] - _feet[1]);
            var up = Mathf.Sin(SmoothStep(StepFracTime) * Mathf.PI) * _stepHeight * Vector3.up;
            return FootBias + pos + up;
        }
    }

    Vector3 RightFootPos {
        get {
            if (!PivotIsLeft) return _feet[1] + FootBias;
            var pos = _feet[0] + StepFracRotation * (_feet[1] - _feet[0]);
            var up = Mathf.Sin(SmoothStep(StepFracTime) * Mathf.PI) * _stepHeight * Vector3.up;
            return FootBias + pos + up;
        }
    }

    Vector3 BodyPosition {
        get {
            var pivot = Vector3.Lerp(RightFootPos, LeftFootPos, Mathf.Sin(StepFracTime * Mathf.PI + (PivotIsLeft ? 0 : Mathf.PI)) * 0.5f + 0.5f);
            pivot.y = 0;
            return pivot + Vector3.up * (_bodyHeight + Mathf.Cos(StepFracTime * Mathf.PI * 2) * _stepHeight / 2);
        }
    }

    Quaternion BodyRotation {
        get {
            var right = RightFootPos - LeftFootPos;
            right.y = 0;
            return
                Quaternion.AngleAxis(-90, Vector3.up) *
                Quaternion.LookRotation(right.normalized) *
                _bodyNoise.Rotation(1, _bodyUndulation);
        }
    }

    Vector3 LeftHandPosition {
        get {
            var pos = _handPosition;
            pos.x *= -1;

            pos = _animator.bodyRotation * pos;
            pos += _animator.bodyPosition;
            pos += _bodyNoise.Vector(3) * _handMove;

            pos = _spineMatrixInv * new Vector4(pos.x, pos.y, pos.z, 1);
            pos.z = Mathf.Max(pos.z, 0.2f);
            pos.y = Mathf.Max(pos.y, 0.2f);
            pos = _spineMatrix * new Vector4(pos.x, pos.y, pos.z, 1);

            return pos;
        }
    }

    Vector3 RightHandPosition {
        get {
            var pos = _handPosition;

            pos = _animator.bodyRotation * pos;
            pos += _animator.bodyPosition;
            pos += _bodyNoise.Vector(4) * _handMove;

            pos = _spineMatrixInv * new Vector4(pos.x, pos.y, pos.z, 1);
            pos.z = Mathf.Max(pos.z, -0.2f);
            pos.y = Mathf.Max(pos.y, 0.2f);
            pos = _spineMatrix * new Vector4(pos.x, pos.y, pos.z, 1);

            return pos;
        }
    }

    Vector3 LookAtPosition {
        get {
            var pos = _bodyNoise.Vector(5) * _headMove;
            pos.z = 2;
            return _animator.bodyRotation * pos + _animator.bodyPosition;
        }
    }

    Animator _animator;

    XXHash _hash;
    NoiseGenerator _bodyNoise;

    float _stepTime;
    Vector3[] _feet = new Vector3[2];

    Matrix4x4 _spineMatrix;
    Matrix4x4 _spineMatrixInv;

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_feet[0], 0.05f);
        Gizmos.DrawWireSphere(_feet[1], 0.05f);
        Gizmos.DrawLine(_feet[0], _feet[1]);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(LeftFootPos, 0.05f);
        Gizmos.DrawWireSphere(RightFootPos, 0.05f);
        Gizmos.DrawLine(LeftFootPos, RightFootPos);
    }

    void Start()
    {
        _animator = GetComponent<Animator>();

        _hash = new XXHash(_randomSeed);
        _bodyNoise = new NoiseGenerator(_noiseFrequency);

        _feet[0] = Vector3.right * _stride * +0.5f;
        _feet[1] = Vector3.right * _stride * -0.5f;
    }

    void Update()
    {
        _bodyNoise.Frequency = _noiseFrequency;
        _bodyNoise.Step();

        var delta = _stepFrequency * Time.deltaTime;

        if (StepCount < Mathf.FloorToInt(_stepTime + delta))
        {
            var lf2rf = (_feet[1] - _feet[0]).normalized * _stride;

            if (PivotIsLeft)
                _feet[1] = _feet[0] + StepRotation * lf2rf;
            else
                _feet[0] = _feet[1] - StepRotation * lf2rf;
        }

        _stepTime += delta;

        var chest = _animator.GetBoneTransform(HumanBodyBones.Chest);
        _spineMatrix = chest.localToWorldMatrix;
        _spineMatrixInv = chest.worldToLocalMatrix;
    }

    void OnAnimatorIK(int layerIndex)
    {
        _animator.SetIKPosition(AvatarIKGoal.LeftFoot, LeftFootPos);
        _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);

        _animator.SetIKPosition(AvatarIKGoal.RightFoot, RightFootPos);
        _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

        _animator.bodyPosition = BodyPosition;
        _animator.bodyRotation = BodyRotation;

        var twist = Quaternion.AngleAxis(-8, Vector3.forward) * _bodyNoise.Rotation(5, 20);
        _animator.SetBoneLocalRotation(HumanBodyBones.Spine, twist);
        _animator.SetBoneLocalRotation(HumanBodyBones.Chest, twist);
        _animator.SetBoneLocalRotation(HumanBodyBones.UpperChest, twist);

        _animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandPosition);
        _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);

        _animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandPosition);
        _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);

        _animator.SetLookAtPosition(LookAtPosition);
        _animator.SetLookAtWeight(1);
    }
}
