namespace CraftCalc.Model
{
    public class UsedMaterial
    {
        public Guid Id { get; init; } = Guid.CreateVersion7();
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        public Guid MaterialId { get; set; }
        public Material? Material { get; set; }
        public decimal QuantitySpent { get; set; }
    }
}
