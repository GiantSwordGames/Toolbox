using UnityEngine;

namespace JamKit
{
    public class MenuStyleDefinition : ScriptableObject
    {
        [SerializeField] private InputKeyAsset _upKey;
        [SerializeField] private InputKeyAsset _downKey;
        [SerializeField] private InputKeyAsset _acceptKey;
        [SerializeField] private InputKeyAsset _backButton;
        [Space] [SerializeField] private Color _selectedColor = Color.white;
        [SerializeField] private Color _deselectedColor = Color.Lerp(Color.white, Color.gray, .2f);
        [SerializeField] private Color _deactivatedColor = Color.grey;
        [Space] [SerializeField] private MenuOption _optionPrefab;
        [Space] [SerializeField] SoundAsset _selectionSound;
        [SerializeField] SoundAsset _clickSound;

        public InputKeyAsset upKey => _upKey;
        public InputKeyAsset downKey => _downKey;
        public InputKeyAsset acceptKey => _acceptKey;
        public InputKeyAsset backButton => _backButton;
        public Color selectedColor => _selectedColor;
        public Color deselectedColor => _deselectedColor;
        public Color deactivatedColor => _deactivatedColor;
        public MenuOption optionPrefab => _optionPrefab;
        public SoundAsset selectionSound => _selectionSound;
        public SoundAsset clickSound => _clickSound;
    }
}