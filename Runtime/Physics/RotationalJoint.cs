using UnityEngine;

namespace JamKit
{
    
    public static class ConfiguralbleJointUtility
    {
        public static void SetAngularSpringAll(this ConfigurableJoint joint, float spring)
        {
            JointDrive drive = joint.angularXDrive;
            drive.positionSpring = spring;
            joint.angularXDrive = drive;
            drive = joint.angularYZDrive;
            drive.positionSpring = spring;
            joint.angularYZDrive = drive;
        }
        
        public static void SetAngularDamperAll(this ConfigurableJoint joint, float spring)
        {
            JointDrive drive = joint.angularXDrive;
            drive.positionDamper = spring;
            joint.angularXDrive = drive;
            drive = joint.angularYZDrive;
            drive.positionDamper = spring;
            joint.angularYZDrive = drive;
        }
        public static void SetSlerpSpring(this ConfigurableJoint joint, float spring)
        {
            JointDrive drive = joint.slerpDrive;
            drive.positionSpring = spring;
            joint.slerpDrive = drive;
        }
        
        public static void SetSlerpDamper(this ConfigurableJoint joint, float damper)
        {
            JointDrive drive = joint.slerpDrive;
            drive.positionDamper = damper;
            joint.slerpDrive = drive;
        }
        
        
        public static void SetMotionSpring(this ConfigurableJoint joint, float spring)
        {
            JointDrive drive = joint.xDrive;
            drive.positionSpring = spring;
            joint.xDrive = drive;
            drive = joint.yDrive;
            drive.positionSpring = spring;
            joint.yDrive = drive;
            drive = joint.zDrive;
            drive.positionSpring = spring;
            joint.zDrive = drive;
        }

        public static void SetMotionDamper(this ConfigurableJoint joint, float damper)
        {
            JointDrive drive = joint.xDrive;
            drive.positionDamper = damper;
            joint.xDrive = drive;
            drive = joint.yDrive;
            drive.positionDamper = damper;
            joint.yDrive = drive;
            drive = joint.zDrive;
            drive.positionDamper = damper;
            joint.zDrive = drive;
            
        }
        
        public static void SetAngularXSpring(this ConfigurableJoint joint, float spring)
        {
            JointDrive drive = joint.angularXDrive;
            drive.positionSpring = spring;
            joint.angularXDrive = drive;            
        }
        public static void SetAngularXDamper(this ConfigurableJoint joint, float damper)
        {
            JointDrive drive = joint.angularXDrive;
            drive.positionDamper = damper;
            joint.angularXDrive = drive;            
        }
        public static void SetAngularYZSpring(this ConfigurableJoint joint, float spring)
        {
            JointDrive drive = joint.angularYZDrive;
            drive.positionSpring = spring;
            joint.angularYZDrive = drive;            
        }   
        public static void SetAngularYZDamper(this ConfigurableJoint joint, float damper)
        {
            JointDrive drive = joint.angularYZDrive;
            drive.positionDamper = damper;
            joint.angularYZDrive = drive;            
        }

        public static void SetAngularLimitX(this ConfigurableJoint joint, float limit)
        {
            SoftJointLimit softJointLimit = joint.lowAngularXLimit;
            softJointLimit.limit = -limit;
            joint.lowAngularXLimit = softJointLimit;
            softJointLimit = joint.highAngularXLimit;
            softJointLimit.limit = limit;
            joint.highAngularXLimit = softJointLimit;
        }
        
        public static void SetAngularLimitY(this ConfigurableJoint joint, float limit)
        {
            SoftJointLimit softJointLimit = joint.angularYLimit;
            softJointLimit.limit = limit;
            joint.angularYLimit = softJointLimit;
        }
        
        public static void SetAngularLimitZ(this ConfigurableJoint joint, float limit)
        {
            SoftJointLimit softJointLimit = joint.angularZLimit;
            softJointLimit.limit = limit;
            joint.angularZLimit = softJointLimit;
        }
        
        public static void SetAngularLimitSpring(this ConfigurableJoint joint, float spring)
        {
            SoftJointLimitSpring limitSpring = joint.angularXLimitSpring;
            limitSpring.spring = spring;
            joint.angularXLimitSpring = limitSpring;
            limitSpring = joint.angularYZLimitSpring;
            limitSpring.spring = spring;
            joint.angularYZLimitSpring = limitSpring;
        }
        
        public static void SetAngularLimitDamping(this ConfigurableJoint joint, float damping)
        {
            SoftJointLimitSpring limitSpring = joint.angularXLimitSpring;
            limitSpring.damper = damping;
            joint.angularXLimitSpring = limitSpring;
            limitSpring = joint.angularYZLimitSpring;
            limitSpring.damper = damping;
            joint.angularYZLimitSpring = limitSpring;
        }
        
        
    }
    public class RotationalJoint : MonoBehaviour
    {
        [SerializeField] private float _drag =1;
        [SerializeField] private float _angularDrag =1;
        
        [Space]
        [SerializeField] private float _angularLimitX = 0;
        [SerializeField] private float _angularLimitY = 0;
        [SerializeField] private float _angularLimitZ = 0;
        [SerializeField] private float _angularLimitSpring;
        [SerializeField] private float _angularLimitDamping;
        [Space]
        [SerializeField] private float _angularSpring;
        [SerializeField] private float _angularDamping;

        private void OnValidate()
        {
            Apply();
        }

        private void Start()
        {
            
        }

        private void Apply()
        {
            if (enabled == false)
            {
                return;
            }
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            ConfigurableJoint joint = GetComponent<ConfigurableJoint>();
            joint.SetAngularSpringAll(_angularSpring);
            joint.SetAngularDamperAll(_angularDamping);
            joint.SetSlerpSpring(_angularSpring);
            joint.SetSlerpDamper(_angularDamping);
            joint.SetAngularLimitSpring(_angularLimitSpring);
            joint.SetAngularLimitDamping(_angularLimitDamping);
            
            
            joint.angularXMotion = ConfigurableJointMotion.Limited;
            joint.angularYMotion = ConfigurableJointMotion.Limited;
            joint.angularZMotion = ConfigurableJointMotion.Limited;
            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;
            
            joint.SetAngularLimitX(_angularLimitX);
            joint.SetAngularLimitY(_angularLimitY);
            joint.SetAngularLimitZ(_angularLimitZ);

            rigidbody.drag = _drag;
            rigidbody.angularDrag = _angularDrag;
        }

        private void OnDrawGizmosSelected()
        {
            Apply();
        }

        private void OnDrawGizmos()
        {
            //draw the limits
            // ConfigurableJoint joint = GetComponent<ConfigurableJoint>();
            // Vector3 center = transform.position;
            // Vector3 x = transform.right * _angularLimitX;
            // Vector3 y = transform.up * _angularLimitY;
            // Vector3 z = transform.forward * _angularLimitZ;
            // Gizmos.color = Color.red;
            // Gizmos.DrawLine(center, center + x);
            // Gizmos.color = Color.green;
            //
        }
    }
}
