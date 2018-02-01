using UnityEngine;
using Klak.Math;

public class Puppet : MonoBehaviour
{
    [SerializeField] float _stepFrequency = 2;
    [SerializeField] float _stride = 0.4f;
    [SerializeField] float _stepHeight = 0.3f;
    [SerializeField] float _bodyHeight = 0.9f;
    [SerializeField] float _bodyUndulation = 10;
    [SerializeField] Vector3 _handPosition = new Vector3(0.3f, 0.3f, -0.2f);
    [SerializeField] float _handMove = 0.2f;
    [SerializeField] float _headMove = 3;
    [SerializeField] float _noiseFrequency = 1.1f;

    Animator _animator;
    NoiseGenerator _bodyNoise;

    Vector3 _leftFootPos;
    Vector3 _rightFootPos;

    Vector3 _newLeftFootPos;
    Vector3 _newRightFootPos;

    int _stepCount;
    float _stepTime;

    Vector3 GetNextStepPos(Vector3 pivot, Vector3 prev)
    {
        var dest = new Vector3(Random.value * 2 - 1, 0, Random.value * 2 - 1);
        var dot = Vector3.Dot((prev - pivot).normalized, (dest - pivot).normalized);
        var flip = dot < 0 ? -1 : 1;
        return pivot + (dest - pivot).normalized * _stride * flip;
    }

    void Start()
    {
        _animator = GetComponent<Animator>();
        _bodyNoise = new NoiseGenerator(_noiseFrequency);

        _leftFootPos = _newLeftFootPos = new Vector3(_stride * 0.5f, 0, 0);
        _rightFootPos = _newRightFootPos = new Vector3(_stride * -0.5f, 0, 0);

        _stepTime = 0;
    }

    void Update()
    {
        _bodyNoise.Frequency = _noiseFrequency;
        _bodyNoise.Step();

        _stepTime += _stepFrequency * Time.deltaTime;

        if (_stepTime > 1)
        {
            if ((_stepCount & 1) == 0)
            {
                _rightFootPos = _newRightFootPos;
                _newRightFootPos = GetNextStepPos(_newLeftFootPos, _rightFootPos);
            }
            else
            {
                _leftFootPos = _newLeftFootPos;
                _newLeftFootPos = GetNextStepPos(_newRightFootPos, _leftFootPos);
            }

            _stepCount += 1;
            _stepTime -= 1;
        }
    }

    void OnAnimatorIK(int layerIndex)
    {
        var rad = (_stepTime + (_stepCount & 1)) * Mathf.PI;

        {
            var param = Mathf.Clamp01(_stepTime + (_stepCount & 1));
            var pos = Vector3.Lerp(_leftFootPos, _newLeftFootPos, param);
            var up = (Vector3.up * _stepHeight + _bodyNoise.Vector(10) * 0.3f) * Mathf.Max(0, Mathf.Sin(rad));
            _animator.SetIKPosition(AvatarIKGoal.LeftFoot, pos + up);
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
        }

        {
            var param = Mathf.Clamp01(_stepTime + 1 - (_stepCount & 1));
            var pos = Vector3.Lerp(_rightFootPos, _newRightFootPos, param);
            var up = (Vector3.up * _stepHeight + _bodyNoise.Vector(11) * 0.3f) * Mathf.Max(0, Mathf.Sin(rad + Mathf.PI));
            _animator.SetIKPosition(AvatarIKGoal.RightFoot, pos + up);
            _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
        }

        {
            var pos = Vector3.Lerp(
                (rad < Mathf.PI / 2) ? _leftFootPos : _newLeftFootPos,
                (rad > Mathf.PI && rad < Mathf.PI * 1.5f) ? _rightFootPos : _newRightFootPos,
                Mathf.Sin(rad) * 0.5f + 0.5f
            );
            var up = Vector3.up * (_bodyHeight + Mathf.Cos(rad * 2) * _stepHeight / 2);
            _animator.bodyPosition = pos + up;
        }

        {
            var right = _animator.GetIKPosition(AvatarIKGoal.RightFoot) - _animator.GetIKPosition(AvatarIKGoal.LeftFoot);
            right.y = 0;
            right = right.normalized;
            var pivot = Quaternion.AngleAxis(-90, Vector3.up) * Quaternion.LookRotation(right);
            var und = _bodyNoise.Rotation(1, _bodyUndulation);
            _animator.bodyRotation = pivot * und;
        }

        {
            var rot = _bodyNoise.Rotation(5, 20, 20, 30);
            _animator.SetBoneLocalRotation(HumanBodyBones.Spine, rot);
            _animator.SetBoneLocalRotation(HumanBodyBones.Chest, rot);
            _animator.SetBoneLocalRotation(HumanBodyBones.UpperChest, rot);
        }

        {
            var pos = _handPosition;
            pos.x *= -1;
            pos = _animator.bodyRotation * pos + _animator.bodyPosition + _bodyNoise.Vector(3) * _handMove;
            _animator.SetIKPosition(AvatarIKGoal.LeftHand, pos);
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        }

        {
            var pos = _handPosition;
            pos = _animator.bodyRotation * pos + _animator.bodyPosition + _bodyNoise.Vector(4) * _handMove;
            _animator.SetIKPosition(AvatarIKGoal.RightHand, pos);
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        }

        {
            var pos = _bodyNoise.Vector(5) * _headMove;
            pos.z = 2;
            _animator.SetLookAtPosition(_animator.bodyRotation * pos + _animator.bodyPosition);
            _animator.SetLookAtWeight(1);
        }
    }
}
