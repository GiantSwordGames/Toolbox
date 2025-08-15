using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace JamKit
{
    public class Health : MonoBehaviour
    {
        [FormerlySerializedAs("_value")] [SerializeField] private SmartFloat _health = new SmartFloat( 1f);
        [SerializeField] private float _maxHealth = 100;        
        [SerializeField] private int _randomHealth = 0;        
        [SerializeField] private float _gibThreshold = -10;        
        [SerializeField] private bool _startAtMax = true;   
        [SerializeField] private bool _invulnerable = false;   
        
         [SerializeField] private UnityEvent _onDamageTaken = new UnityEvent();
         [SerializeField] private UnityEvent _onDeath = new UnityEvent();
         [SerializeField] private UnityEvent _onGib = new UnityEvent();

         
         [Header("Not Implemented")]
         [SerializeField] private List<TagAsset> _include;
         [SerializeField] private List<TagAsset> _exclude;

         private bool _isDead; 
         public UnityEvent onDamageTaken => _onDamageTaken;

         public UnityEvent onDeath => _onDeath;

         public float currentHealth => _health;
         public float maxHealth => _maxHealth;
         
         public float damageTaken => _maxHealth - currentHealth;

         public bool invulnerable
         {
             get => _invulnerable;
             set => _invulnerable = value;
         }

         public bool isAlive => currentHealth > 0;


         private void Awake()
         {
             if (_startAtMax)
             {
                 _maxHealth += Random.Range(0, _randomHealth);
                 _health.value =  _maxHealth;
                 
             }
         }

         private void OnValidate()
         {
             if (_startAtMax)
             {
                 _health.value = _maxHealth;
             }
         }

         [Button]
         public void TestDamage()
         {
             Damage damage = Damage.CreateInstance<Damage>();
             damage.damage = 1;
             
                ApplyDamage(DamageIncident.Create(damage, this, this, Vector3.zero, Vector3.zero, Vector3.zero));
         }

         public void ApplyDamage(DamageIncident damageIncident)
        {
            float remainingDamage = damageIncident.damage.damage*damageIncident.attenuation;

            float previousValue = _health.value;
            // if (remainingDamage > _health)
            // {
            //     remainingDamage = _health;
            // }
            //
            

            if (_invulnerable == false)
            {
                _health.value -= remainingDamage;
            }

            // _value.value = Mathf.Max(_value.value, 0);

            if (remainingDamage > 0)
            {
                onDamageTaken.SafeInvoke();
            }
            

            if ( _isDead == false &&  _health <= 0)
            {
                _isDead = true;

                _onDeath.SafeInvoke();
                
            }

            if (previousValue > _gibThreshold && _health <= _gibThreshold )
            {
                _onGib.SafeInvoke();
            }
            
        }

         public float GetHealthNormalized()
         {
             return _health / _maxHealth;
         }

        private IEnumerator IEFreezeFrame()
        {
            Time.timeScale = 0.001f;
            yield return new WaitForSecondsRealtime(.15f);
            Time.timeScale = 1;
        }
        
        public void Heal(float healAmount)
        {
            _health.value += healAmount;
            _health.value = Mathf.Max(0, _health.value);
        }
        
        public void HealOverTime(float healAmount, float duration)
        {
            StartCoroutine(IEHealOverTime(healAmount, duration));
        }

        private IEnumerator IEHealOverTime(float healAmount, float duration)
        {
            float time = 0;
            while (time < duration)
            {
                Heal(healAmount * Time.deltaTime / duration);
                time += Time.deltaTime;
                yield return null;
            }
        }


        [Button]
        public void TriggerDeath()
        {
            _health.value = 0;
            _isDead = true;
            _onDeath?.Invoke();
        }
        
        [Button]
        public void TriggerGib()
        {
            _health.value = _gibThreshold;
            _isDead = true;
            _onGib?.Invoke();
        }
    }
}