using UnityEngine;

namespace JamKit
{
    public class TimeButton : MonoBehaviour
    {
        [SerializeField] private ScriptableBool _isPaused;
        [SerializeField] private ScriptableFloat _developers;
        [SerializeField] private GameObject _visibilityRoot;
        [SerializeField] private GameObject _pauseButton;
        [SerializeField] private GameObject _playButton;
        
        void Start()
        {
            Refresh();
            _isPaused.onValueChanged += OnValueChanged;
            _developers.onValueChanged += OnValueChanged; 
        }

        private void OnValueChanged(float obj)
        {
            Refresh();
        }

        private void OnValueChanged(bool obj)
        {
            Refresh();
        }

        private void Refresh()
        {
            _visibilityRoot.gameObject.SetActive(_developers.value > 0);
            _pauseButton.gameObject.SetActive(!_isPaused.value);
            _playButton.gameObject.SetActive(_isPaused.value);
        }
    }
}
