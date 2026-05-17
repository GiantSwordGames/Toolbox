namespace JamKit
{
    public class ScriptableInt : ScriptableFloat
    {
        public new int initialValue
        {
            get => base.initialValue.ToInt();
            set => base.initialValue = value;
        }
        public new int value
        {
            get => base.value.ToInt();
            set => base.value = value;
        }

        public static implicit operator int(ScriptableInt scriptableInt)
        {
            return scriptableInt.value;
        }
        
    }
}