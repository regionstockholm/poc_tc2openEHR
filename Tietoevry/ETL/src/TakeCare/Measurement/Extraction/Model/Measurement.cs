namespace TakeCare.Migration.OpenEhr.Measurement.Extraction.Model
{
    public class Measurement
    {
        public Guid Guid { get; set; }
        public Term Term { get; set; }
        public decimal Value { get; set; }
        public double NumDecimals { get; set; }
        public string Comment { get; set; }
        public string ScaleText { get; set; }
    }

    public class Term
    {
        public Guid MeasurementGuid { get; set; }
        public Guid Guid { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
    }

}
