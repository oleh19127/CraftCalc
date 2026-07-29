using CraftCalc.Model;

namespace CraftCalc.Calculator
{
    public class PriceCalculator(Product product, List<Material> materials)
    {
        public decimal CalculateSingleMaterialCost(UsedMaterial usedItem)
        {
            Material? foundMaterial = materials.FirstOrDefault(m => m.Id == usedItem.MaterialId);

            if (foundMaterial == null || foundMaterial.TotalQuantity == 0)
            {
                return 0m;
            }

            decimal costPerUnit = foundMaterial.PackagingCost / foundMaterial.TotalQuantity;
            decimal finalCostForThisItem = costPerUnit * usedItem.QuantitySpent;
            return finalCostForThisItem;
        }

        public decimal CalculateTotalMaterialsCost() =>
            product.MaterialsUsed.Sum(CalculateSingleMaterialCost);

        public decimal CalculateWorkTime() => product.TimeSpent * product.CostOfAnHourOfWork;

        public decimal CalculateFinalPrice()
        {
            decimal materialsCost = CalculateTotalMaterialsCost();
            decimal workCost = CalculateWorkTime();
            decimal basePrice = materialsCost + workCost + product.MarkUp;
            decimal percentageAmount = basePrice * (product.MarkUpPercentage / 100m);
            return basePrice + percentageAmount;
        }
    }
}
