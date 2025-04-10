using System;
using System.Data;

namespace GiantSword
{
    public static class MathHelper
    {
        public enum Operation
        {
            Increment,
            Decrement,
            Multiply,
            IncreaseByPercent,
            DecreaseByPercent
        }
        
        public static float ApplyOperation(float A, float B, Operation operation)
        {
            switch (operation)
            {
                case Operation.Increment:
                    A += B;
                    break;
                case Operation.Decrement:
                    A -= B;
                    break;
                case Operation.Multiply:
                    A *= B;
                    break;
                case Operation.IncreaseByPercent:
                    A *= 1 + B / 100f;
                    break;
                case Operation.DecreaseByPercent:
                    A *= 1 - B / 100f;
                    break;
            }
            
            return A;
        }
        
        
        // this generates a lot of garbage and is expensive
        public static float TryEvaluateExpression(string expression, out bool failed)
        {
            try
            {
                DataTable table = new DataTable();
                var result = table.Compute(expression, string.Empty);
                failed = false;
                return (float)Convert.ToDouble(result);
            }
            catch (Exception)
            {
                failed = true;
                return 0;
            }
        }

        public static float HoursToYears(float hours)
        {
            return hours / 8760;
        }

    }
}