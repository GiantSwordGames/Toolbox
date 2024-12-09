using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace GiantSword
{
public class ExplodeGibs : MonoBehaviour
{
    [SerializeField] private GameObject _gibHolder;
    [FormerlySerializedAs("_additionalGibHolder")] [SerializeField] private GameObject[] _additionalGibHolders;
    [SerializeField] private PhysicalMaterialDefinition _materialDefinition;
    [SerializeField] private SoundAsset _breakSound;
    [SerializeField] private DamageIncident _damageIncident;
    [SerializeField] float _forceMultiplyer = 1;
    public void Trigger(DamageIncident damageIncident)
    {
        _damageIncident = damageIncident;
        Trigger();
    }

    public void Trigger()
    {
        _breakSound?.Play(transform.position);
        ExplodeHolder(_gibHolder);

        foreach (GameObject holder in _additionalGibHolders)
        {
            ExplodeHolder(holder);
        }
    }

    private void ExplodeHolder(GameObject holder)
    {
        if (holder == null)
        {
            return;
        }
        holder.gameObject.SetActive(true);
        MeshRenderer[] meshRenderers = holder.GetComponentsInChildren<MeshRenderer>(true);

        foreach(MeshRenderer meshRenderer in meshRenderers)
        {
            try
            {

                Gib gib = meshRenderer.gameObject.AddComponent<Gib>();
                gib.Setup();
                gib.transform.SetParent(null, true);
                Vector3 force = holder.transform.position.To(meshRenderer.transform.position).normalized;
                gib.gameObject.AddComponent<SetDensity>().SetDensityAtRuntime(_materialDefinition);

                if (_damageIncident)
                {
                    force = Vector3.Lerp(force, _damageIncident.direction, 0.8f);

                    force *= _damageIncident.damage.knockBack * Random.Range(.5f, 1.5f);
                }

                Vector3 position = transform.position;

                if (_damageIncident)
                {
                    position =   _damageIncident.position;
                }
           
                force *= _forceMultiplyer;

                gib.rigidbody.AddForceAtPosition(force, position, ForceMode.Impulse);
            }
            catch (Exception e)
            {
                Debug.LogException( e, this);
            }
            
        }
    }
}
}