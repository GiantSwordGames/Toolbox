using UnityEngine;
using UnityEngine.Serialization;

namespace JamKit
{
    public class MenuStyleDefinition : ScriptableObject
    {
        [FormerlySerializedAs("_upKey")] [SerializeField] private InputAsset _up;
        [FormerlySerializedAs("_downKey")] [SerializeField] private InputAsset _down;
        [FormerlySerializedAs("_acceptKey")] [SerializeField] private InputAsset _accept;
        [SerializeField] private InputAsset _backButton;
        [Space] [SerializeField] private Color _selectedColor = Color.white;
        [SerializeField] private Color _deselectedColor = Color.Lerp(Color.white, Color.gray, .2f);
        [SerializeField] private Color _deactivatedColor = Color.grey;
        [Space] [SerializeField] private MenuOption _optionPrefab;
        [Space] [SerializeField] SoundAsset _selectionSound;
        [SerializeField] SoundAsset _clickSound;

        public InputAsset up => _up;
        public InputAsset down => _down;
        public InputAsset accept => _accept;
        public InputAsset backButton => _backButton;
        public Color selectedColor => _selectedColor;
        public Color deselectedColor => _deselectedColor;
        public Color deactivatedColor => _deactivatedColor;
        public MenuOption optionPrefab => _optionPrefab;
        public SoundAsset selectionSound => _selectionSound;
        public SoundAsset clickSound => _clickSound;
    }
}