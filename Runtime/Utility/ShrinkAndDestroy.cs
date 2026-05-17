using System.Collections;
using JamKit;
using UnityEngine;

public class ShrinkAndDestroy : MonoBehaviour
{
    public float _delay = 5f;
    private float _timer = 0.0f;
    private Vector3 _initialScale;

    private void Start()
    {
        this._initialScale = this.transform.localScale;
        StartCoroutine(IEShrinkAndDestroy());
    }
    
    private IEnumerator IEShrinkAndDestroy()
    {
        yield return new WaitForSeconds(this._delay);

        var component = GetComponent<Rigidbody>();
        if (component != null)
        {
            
            // component.isKinematic = true;
        }
        float t = 0.0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            this.transform.localScale = Vector3.Lerp(this._initialScale, Vector3.one*0.00001f, t);
            yield return null;
        }
        Object.Destroy((Object) this.gameObject);
    }

 

    public void SetDelay(float delay)
    {
        this._delay = delay;
    }
}