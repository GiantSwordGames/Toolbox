using UnityEngine;

namespace GiantSword
{
    public class MenuDefinition : ScriptableObject
    {
        [SerializeField] private InputKeyAsset _upKey;
        [SerializeField] private InputKeyAsset _downKey;
        [SerializeField] private InputKeyAsset _acceptKey;
        [Space]
        [SerializeField] private Color _selectedColor = Color.white;
        [SerializeField] private Color _deselectedColor = Color.Lerp(Color.white, Color.gray, .2f );
        [SerializeField] private Color _deactivatedColor = Color.grey;
        [Space]
        [SerializeField] private MenuOption _optionPrefab;
        [SerializeField] private MenuOptionAsset[] _options;
        [SerializeField] private MenuOptionAsset _initialSelection;

        public InputKeyAsset upKey => _upKey;

        public InputKeyAsset downKey => _downKey;

        public InputKeyAsset acceptKey => _acceptKey;

        public MenuOption optionPrefab => _optionPrefab;

        public MenuOptionAsset[] options => _options;

        public MenuOptionAsset initialSelection => _initialSelection;

        public Color selectedColor => _selectedColor;

        public Color deselectedColor => _deselectedColor;

        public Color deactivatedColor => _deactivatedColor;
    }
}