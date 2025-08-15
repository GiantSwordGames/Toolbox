using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JamKit
{
    public class PlaySoundOnUiEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler
    {
        // [SerializeField] private SoundAsset _onPointerEnter;
        // [SerializeField] private SoundAsset _onPointerExit;
        // [SerializeField] private SoundAsset _onPointerClick;
        //
        public void OnPointerEnter(PointerEventData eventData)
        {
            // _onPointerEnter?.Play();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // _onPointerExit?.Play();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // _onPointerClick?.Play();
        }

        [Button]
        public void TestString()
        {
            Debug.Log("_onTest is working".ToUpperCamelCase());
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // _onPointerClick?.Play();
        }
    }
}
