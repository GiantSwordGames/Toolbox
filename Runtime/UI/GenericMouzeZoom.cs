using JamKit;
using UnityEngine;

public class GenericMouzeZoom : MonoBehaviour
{
    private RectTransform _parentRectTransform;
    private RectTransform _rectTransform;
    private Camera _uiCamera = default; // Set if using Screen Space - Camera
    [SerializeField]private float _sensitivity = 0.05f;
    [SerializeField]  public Vector2 _minMax = new Vector2(0.2f,2f);
    private void Start()
    {
        _parentRectTransform = transform.parent.GetComponent<RectTransform>();
        _rectTransform = GetComponent<RectTransform>();
        // uiCamera = Camera.main;
    }

    void Update()
    {
        Vector2 scroll = Input.mouseScrollDelta*_sensitivity;
        if (scroll.y != 0)
        {
            Vector2 localMousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRectTransform, Input.mousePosition, _uiCamera, out localMousePosition);

            Vector2 delta = localMousePosition.To(_rectTransform.anchoredPosition);
            float zoom = 1 + scroll.y;
            float newScale = transform.localScale.x * zoom;
            newScale = Mathf.Clamp(newScale,_minMax.x,_minMax.y);
            zoom = newScale/transform.localScale.x;
            
            transform.localScale = new Vector3(newScale,newScale,newScale);
            _rectTransform.anchoredPosition = localMousePosition + delta*zoom ;
        }
    }
}
