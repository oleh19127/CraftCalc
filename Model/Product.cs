namespace CraftCalc.Model
{
    public class Product
    {
        public Guid Id { get; init; } = Guid.CreateVersion7();
        public string Name { get; set; } = string.Empty;
        public List<UsedMaterial> MaterialsUsed { get; set; } = [];
        public decimal TimeSpent { get; set; }
        public decimal CostOfAnHourOfWork { get; set; }
        public decimal MarkUp { get; set; }
        public decimal MarkUpPercentage { get; set; }
    }
}
