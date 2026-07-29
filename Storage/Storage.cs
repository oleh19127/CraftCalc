using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using CraftCalc.Model;

namespace CraftCalc.Storage
{
    public class StorageManager
    {
        private readonly string _dataFile = "craftcalc_data.json";

        private readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
        };

        public void SaveData(List<Material> materials, List<Product> products)
        {
            AppData data = new()
            {
                Materials = materials,
                Products = products
            };

            string jsonText = JsonSerializer.Serialize(data, _options);
            File.WriteAllText(_dataFile, jsonText);
        }

        public (List<Material>, List<Product>) LoadData()
        {
            if (!File.Exists(_dataFile))
            {
                return ([], []);
            }

            string jsonText = File.ReadAllText(_dataFile);

            AppData? data = JsonSerializer.Deserialize<AppData>(jsonText, _options);

            if (data == null)
            {
                return ([], []);
            }

            return (data.Materials ?? [], data.Products ?? []);
        }
    }
}
