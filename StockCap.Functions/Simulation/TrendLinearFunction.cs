namespace StockCap.Functions.Simulation
{
    public class TrendLinearFunction
    {
        /// <summary>f(x) = ?, when x = 0.</summary>
        public required float F0 { get; init; }
        /// <summary>The line "slope" factor.</summary>
        public required float K { get; init; }
        
        public float GetValue(int x) =>
            K * x + F0;
    }
}