using CraftCalc.Model;

namespace CraftCalc.Calculator
{
    public class PriceCalculator(Product product)
    {
        public static decimal CalculateSingleMaterialCost(UsedMaterial usedItem)
        {
            if (usedItem.Material == null || usedItem.Material.TotalQuantity == 0)
            {
                return 0m;
            }

            decimal costPerUnit = usedItem.Material.PackagingCost / usedItem.Material.TotalQuantity;
            return costPerUnit * usedItem.QuantitySpent;
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
