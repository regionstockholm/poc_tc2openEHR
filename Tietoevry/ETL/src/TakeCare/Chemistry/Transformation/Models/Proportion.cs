namespace TakeCare.Migration.OpenEhr.Chemistry.Transformation.Models
{
    public class Proportion
    {
        public decimal Numerator { get; set; }
        public decimal Denominator { get; set; }
        public int Type { get; set; }
        public decimal Value { get; set; }
    }
}