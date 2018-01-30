using UnityEngine;
using Klak.Math;

public class Puppet : MonoBehaviour
{
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
        var footTime = Time.time * 4;

        _animator.SetIKPosition(AvatarIKGoal.LeftFoot, new Vector3(0.2f, 0.3f * Mathf.Max(0, Mathf.Sin(footTime)), 0));
        _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);

        _animator.SetIKPosition(AvatarIKGoal.RightFoot, new Vector3(-0.2f, 0.3f * Mathf.Max(0, Mathf.Sin(footTime + Mathf.PI)), 0));
        _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

        _animator.bodyPosition = new Vector3(Mathf.Sin(footTime) * -0.2f, 0.9f + Mathf.Cos(footTime * 2) * 0.1f, 0);
        _animator.bodyRotation = Quaternion.AngleAxis(180, Vector3.up) * _bodyNoise.Rotation(1, 10, 10, 10);

        _animator.SetIKPosition(AvatarIKGoal.LeftHand, _animator.bodyPosition + new Vector3(0.3f, 0.2f, -0.2f) + Vector3.Scale(_bodyNoise.Vector(2), new Vector3(0.1f, 0.3f, 0.1f)));
        _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);

        _animator.SetIKPosition(AvatarIKGoal.RightHand, _animator.bodyPosition + new Vector3(-0.3f, 0.2f, -0.2f) + Vector3.Scale(_bodyNoise.Vector(3), new Vector3(0.1f, 0.3f, 0.1f)));
        _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);

        _animator.SetLookAtPosition(new Vector3(0, 0, -1) + _bodyNoise.Vector(4));
        _animator.SetLookAtWeight(1);
    }
}
