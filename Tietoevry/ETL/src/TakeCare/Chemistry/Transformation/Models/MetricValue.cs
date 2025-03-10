namespace TakeCare.Migration.OpenEhr.Chemistry.Transformation.Models
{
    public class MetricValue
    {
        public string Magnitude { get; set; }
        public string Unit { get; set; }

        public string? NormalStatus { get; set; }
    }
}