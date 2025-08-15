using System;
using UnityEngine;

namespace GiantSword
{
    public class AlignRigidBodyWithTransform : AlignRigidBodyToDirectionBase
    {
         [SerializeField] private Transform _alignToTransform;
         protected override Vector3 direction =>_alignToTransform ? _alignToTransform.forward:Vector3.forward;
    }
}