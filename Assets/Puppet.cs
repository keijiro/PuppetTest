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
        _animator.SetIKPosition(AvatarIKGoal.LeftFoot, new Vector3(0.2f, 0.1f, 0));
        _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);

        _animator.SetIKPosition(AvatarIKGoal.RightFoot, new Vector3(-0.2f, 0.1f, 0));
        _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

        _animator.SetIKPosition(AvatarIKGoal.LeftHand, new Vector3(0.5f, 1.3f, 0));
        _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);

        _animator.SetIKPosition(AvatarIKGoal.RightHand, new Vector3(-0.5f, 1.3f, 0));
        _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);

        _animator.bodyPosition = new Vector3(0, 0.8f, 0) + _bodyNoise.Vector(0) * 0.2f;
        _animator.bodyRotation = Quaternion.AngleAxis(180, Vector3.up) * _bodyNoise.Rotation(1, 30, 30, 30);

        _animator.SetLookAtPosition(new Vector3(0, 0, -1));
        _animator.SetLookAtWeight(1);
    }
}
