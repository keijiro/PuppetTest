using UnityEngine;
using Klak.Math;

public class Puppet : MonoBehaviour
{
    [SerializeField] float _stepFrequency = 3;
    [SerializeField] float _stride = 0.4f;
    [SerializeField] float _stepHeight = 0.3f;
    [SerializeField] float _bodyHeight = 0.9f;
    [SerializeField] float _bodyUndulation = 10;
    [SerializeField] Vector3 _handPosition = new Vector3(0.3f, 0.3f, -0.2f);
    [SerializeField] float _handMove = 0.2f;
    [SerializeField] float _headMove = 3;

    Animator _animator;
    NoiseGenerator _bodyNoise;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _bodyNoise = new NoiseGenerator(1.1f);
    }

    void Update()
    {
        _bodyNoise.Step();
    }

    void OnAnimatorIK(int layerIndex)
    {
        var footTime = Time.time * _stepFrequency;

        {
            var fx = _stride * 0.5f;
            var fy = Mathf.Max(0, Mathf.Sin(footTime)) * _stepHeight;
            _animator.SetIKPosition(AvatarIKGoal.LeftFoot, new Vector3(fx, fy, 0));
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
        }

        {
            var fx = _stride * -0.5f;
            var fy = Mathf.Max(0, Mathf.Sin(footTime + Mathf.PI)) * _stepHeight;
            _animator.SetIKPosition(AvatarIKGoal.RightFoot, new Vector3(fx, fy, 0));
            _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
        }

        {
            var bx = Mathf.Sin(footTime) * _stride * -0.5f;
            var by = _bodyHeight + Mathf.Cos(footTime * 2) * _stepHeight / 3;
            _animator.bodyPosition = new Vector3(bx, by, 0);
        }

        {
            var pivot = Quaternion.AngleAxis(180, Vector3.up);
            var und = _bodyNoise.Rotation(1, _bodyUndulation);
            _animator.bodyRotation =  pivot * und;
        }

        {
            var pos = _handPosition;
            pos += _animator.bodyPosition + _bodyNoise.Vector(3) * _handMove;
            _animator.SetIKPosition(AvatarIKGoal.LeftHand, pos);
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        }

        {
            var pos = _handPosition;
            pos.x *= -1;
            pos += _animator.bodyPosition + _bodyNoise.Vector(4) * _handMove;
            _animator.SetIKPosition(AvatarIKGoal.RightHand, pos);
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        }

        {
            var pos = _bodyNoise.Vector(5) * _headMove;
            pos.z = -2;
            _animator.SetLookAtPosition(pos);
            _animator.SetLookAtWeight(1);
        }
    }
}
