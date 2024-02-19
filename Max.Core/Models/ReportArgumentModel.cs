namespace Max.Core.Models
{
    public class ReportArgumentModel
    {
        public int ReportArgumentId { get; set; }
        public int ReportId { get; set; }
        public string Label { get; set; }
        public string Name { get; set; }
        public string DataType { get; set; }
        public string FieldType { get; set; }
        public sbyte Required { get; set; }
    }
}
