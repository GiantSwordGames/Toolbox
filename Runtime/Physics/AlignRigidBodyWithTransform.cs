using System;
using UnityEngine;

namespace JamKit
{
    public class AlignRigidBodyWithTransform : AlignRigidBodyToDirectionBase
    {
         [SerializeField] private Transform _alignToTransform;
         public override Vector3 direction =>_alignToTransform ? _alignToTransform.forward:Vector3.forward;
    }
}