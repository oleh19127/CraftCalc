namespace CraftCalc.Model
{
    public class Material
    {
        public Guid Id { get; init; } = Guid.CreateVersion7();
        public string Name { get; set; } = string.Empty;
        public decimal PackagingCost { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal AvailableQuantity { get; set; }
        public string UnitOfMeasurement { get; set; } = string.Empty;
    }
}
