using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace JamKit
{
    public class DamageIncidentManager: MonoBehaviour
    {
        private static DamageIncidentManager _instance;

        public static DamageIncidentManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<DamageIncidentManager>();
                }

                if (_instance == null)
                {
                    GameObject go = new GameObject("DamageIncidentManager");
                    _instance = go.AddComponent<DamageIncidentManager>();
                }

                return _instance;
            }
            
       
        }
        
        public void UpdateCount()
        {
            name = "DamageIncidentManager: " + transform.childCount;
        }
    }
    
    public class DamageIncident : MonoBehaviour
    {
        public MonoBehaviour sender;
        public Component receiver;
        public Damage damage;
        [FormerlySerializedAs("attenation")] [FormerlySerializedAs("forceAttenation")] public float attenuation = 1;
        [ShowNonSerializedField] private int _framw;
        [ShowNonSerializedField] private float _timestamp;
        [ShowNonSerializedField] private Vector3 _normal;
        [ShowNonSerializedField] private Health _toHealth;
        [ShowNonSerializedField] private Rigidbody _toRigidbody;

        public Vector3 normal => _normal;
        public Vector3 position => transform.position;
        public Vector3 direction => transform.forward;
        
        

        public static DamageIncident Create(Damage damage, MonoBehaviour sender, Component reciever,  Vector3 position, Vector3 direction, Vector3 normal, float attenuation = 1)
        {
            GameObject go = new GameObject($"DamageIncident: {(sender != null?sender.name:"Null")}->{(reciever != null?reciever.name:"Null")}");
            go.hideFlags = HideFlags.DontSave;
            DamageIncident damageIncident = go.AddComponent<DamageIncident>();

            damageIncident.transform.parent = DamageIncidentManager.instance.transform;
            damageIncident.transform.position = position;
            damageIncident.transform.forward = direction;
            damageIncident.sender = sender;
            damageIncident.receiver = reciever;
            damageIncident.damage = damage;
            damageIncident._normal = normal;
            damageIncident.attenuation = attenuation;
            damageIncident.Apply();

            if (Application.isPlaying)
            {
                damageIncident.StartCoroutine(damageIncident.IEAutoDestroy());
            }
            DamageIncidentManager.instance.UpdateCount();

            return damageIncident;
        }
        
         private IEnumerator IEAutoDestroy()
        {
            yield return new WaitForSeconds(10f);
            transform.parent = null;
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            DamageIncidentManager.instance.UpdateCount();
        }

      
        [Button("Replay")]
        public void Apply()
        {
            _toHealth = receiver.GetComponentInParent<Health>();
            _toRigidbody = receiver.GetComponentInParent<Rigidbody>();

            if (_toHealth)
            {
                _toHealth.ApplyDamage(this);
            }

            if (_toRigidbody)
            {
                _toRigidbody.AddForceAtPosition(transform.forward * (damage.knockBack * attenuation),  transform.position, ForceMode.Impulse);
            }
        }
        
        [Button]
        public void CancelAutoDestroy()
        {
            StopAllCoroutines();
        }

        public bool IsReceiverIsOrganic()
        {
            List<TagAsset> tags = receiver.GetTagsInParents();
            foreach (var tag in tags)
            {
                if (tag.name.Equals( "Tag_Organic"))
                {
                    return true;
                }
            }

            return false;
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red.WithAlpha(0.5f);
            Gizmos.DrawSphere(transform.position, 0.03f);
        }
    }
}