using JamKit;
using NaughtyAttributes;
using UnityEngine;


public class GenerateRope : MonoBehaviour
{
    [InlineScriptableObject]
    [SerializeField] private GenerateRopeConfiguration _configuration;
    [SerializeField] private int length = 5;

    void Start()
    {
        AvoidCollision();
    }
    
    private void AvoidCollision()
    {      
        for (int i = 2; i < length; i++)
        {
            Collider colliderA = transform.GetChild(i-2).GetComponentInChildren<Collider>();
            Collider colliderB = transform.GetChild(i).GetComponentInChildren<Collider>();
            Physics.IgnoreCollision(colliderA, colliderB);
        }
    }


    [Button]
    private void Generate()
    {
        transform.DestroyChildren();
        Vector3 offset = Vector3.zero;
        for (int i = 0; i < length; i++)
        {
            GameObject instantiate = _configuration.segment.gameObject.SmartInstantiate(  transform );
            instantiate.transform.localPosition = offset;
                offset +=Quaternion.Euler( _configuration.rotationOffset*i)*_configuration.offset;
            instantiate.transform.localRotation = Quaternion.Euler( _configuration.rotationOffset*i);
            instantiate.transform.localScale = Vector3.one;
            instantiate.GetOrAddComponent<Rigidbody>();
            instantiate.name = "Segment" + i;
        }

        for (int i = 1; i < length; i++)
        {
            ConfigurableJoint configurableJoint = transform.GetChild(i).gameObject.GetOrAddComponent<ConfigurableJoint>();
            // configurableJoint.autoConfigureConnectedAnchor = false;
            configurableJoint.connectedBody =   transform.GetChild(i-1).GetComponent<Rigidbody>();
            Debug.Log("Connected body: " + configurableJoint.connectedBody);
            // configurableJoint.connectedAnchor = new Vector3(0, 0, 0.5f);
            // configurableJoint.anchor anchor= new Vector3(0, 0, 0);
        }

        if (_configuration.removeTheFirstJoint == false)
        {
            ConfigurableJoint configurableJoint = transform.GetChild(0).gameObject.AddComponent<ConfigurableJoint>();
            configurableJoint.autoConfigureConnectedAnchor = true;
        }
        else
        {
            ConfigurableJoint joint = transform.GetChild(0).GetComponent<ConfigurableJoint>();
            if(joint)
                
                RuntimeEditorHelper.SmartDestroy(joint);
        }
  
        for (int i = 0; i < length; i++)
        {
            ApplyRopeConfiguration(transform.GetChild(i).gameObject);
        }
    }

    private void ApplyRopeConfiguration(GameObject segment)
    {
        Rigidbody rigidbody = segment.GetComponent<Rigidbody>();
        ConfigurableJoint configurableJoint = segment.GetComponent<ConfigurableJoint>();

        if (rigidbody)
        {
            rigidbody.mass = _configuration.segmentMass;
            rigidbody.drag = _configuration.segmentDrag;
        }

        if (configurableJoint)
        {
            configurableJoint.xMotion = ConfigurableJointMotion.Locked;
            configurableJoint.yMotion = ConfigurableJointMotion.Locked;
            configurableJoint.zMotion = ConfigurableJointMotion.Locked;
            //
            // configurableJoint.angularXMotion = ConfigurableJointMotion.Free;
            // configurableJoint.angularYMotion = ConfigurableJointMotion.Free;
            // configurableJoint.angularZMotion = ConfigurableJointMotion.Locked;
            //
            //
            // JointDrive angularXDrive = configurableJoint.angularXDrive;
            // angularXDrive.positionSpring = _configuration.springStrength;
            // angularXDrive.positionDamper = _configuration.springDamper;
            // configurableJoint.angularXDrive = angularXDrive;
            //
            // JointDrive angularYZDrive = configurableJoint.angularYZDrive;
            // angularYZDrive.positionSpring = _configuration.springStrength;
            // angularYZDrive.positionDamper = _configuration.springDamper;
            // configurableJoint.angularYZDrive = angularYZDrive;
        }
    }
}
