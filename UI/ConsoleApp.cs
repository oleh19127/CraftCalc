using CraftCalc.Calculator;
using CraftCalc.Model;
using CraftCalc.Storage;

namespace CraftCalc.UI
{
    public class ConsoleApp(StorageManager storage, List<Material> materials, List<Product> products)
    {
        private bool _isRunning;

        public void Run()
        {
            _isRunning = true;

            while (_isRunning)
            {
                Console.Clear();
                Console.WriteLine("==========================================");
                Console.WriteLine(" 🧮 CraftCalc: Калькулятор Хендмейду ");
                Console.WriteLine("==========================================\n");
                Console.WriteLine("1. 📦 Управління складом (Матеріали)");
                Console.WriteLine("2. 💎 Управління каталогом (Вироби)");
                Console.WriteLine("0. 💾 Зберегти та вийти");
                Console.Write("\nОберіть розділ (0-2): ");

                string? choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        MaterialsMenu();
                        break;
                    case "2":
                        ProductsMenu();
                        break;
                    case "0":
                        SaveAndExit();
                        break;
                    default:
                        Console.WriteLine("\n[ПОМИЛКА] Невідома команда.");
                        WaitForKeyPress();
                        break;
                }
            }

            Console.WriteLine("\nДякую за використання програми! До побачення.");
        }

        private void MaterialsMenu()
        {
            bool inMaterials = true;
            while (inMaterials)
            {
                Console.Clear();
                Console.WriteLine("=== 📦 СКЛАД МАТЕРІАЛІВ ===");
                Console.WriteLine("1. Додати новий матеріал");
                Console.WriteLine("2. Переглянути залишки на складі");
                Console.WriteLine("3. Редагувати матеріал");
                Console.WriteLine("4. Видалити матеріал");
                Console.WriteLine("0. ⬅️ Повернутися до Головного меню");
                Console.Write("\nОберіть дію (0-4): ");

                string? choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": AddMaterial(); WaitForKeyPress(); break;
                    case "2": ShowMaterials(); WaitForKeyPress(); break;
                    case "3": EditMaterial(); WaitForKeyPress(); break;
                    case "4": DeleteMaterial(); WaitForKeyPress(); break;
                    case "0": inMaterials = false; break;
                    default:
                        Console.WriteLine("\n[ПОМИЛКА] Невідома команда.");
                        WaitForKeyPress();
                        break;
                }
            }
        }

        private void ProductsMenu()
        {
            bool inProducts = true;
            while (inProducts)
            {
                Console.Clear();
                Console.WriteLine("=== 💎 КАТАЛОГ ВИРОБІВ ===");
                Console.WriteLine("1. Створити новий виріб (Кошторис)");
                Console.WriteLine("2. Показати всі вироби (Деталі вартості)");
                Console.WriteLine("3. Редагувати існуючий виріб");
                Console.WriteLine("4. Видалити виріб");
                Console.WriteLine("0. ⬅️ Повернутися до Головного меню");
                Console.Write("\nОберіть дію (0-4): ");

                string? choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": CreateProduct(); WaitForKeyPress(); break;
                    case "2": ShowProducts(); WaitForKeyPress(); break;
                    case "3": EditProduct(); WaitForKeyPress(); break;
                    case "4": DeleteProduct(); WaitForKeyPress(); break;
                    case "0": inProducts = false; break;
                    default:
                        Console.WriteLine("\n[ПОМИЛКА] Невідома команда.");
                        WaitForKeyPress();
                        break;
                }
            }
        }

        private static void WaitForKeyPress()
        {
            Console.WriteLine("\nНатисніть Enter для продовження...");
            Console.ReadLine();
        }

        private static string ChooseUnitOfMeasurement()
        {
            Console.WriteLine("\nОберіть одиницю виміру для цього матеріалу:");
            Console.WriteLine("1. Грами (г) - для бісеру, глини, тощо");
            Console.WriteLine("2. Штуки (шт) - для фурнітури, намистин, застібок");
            Console.WriteLine("3. Метри (м) - для ниток, волосіні, ланцюжків");
            Console.WriteLine("4. Мілілітри (мл) - для фарби, клею, лаку");
            Console.WriteLine("5. Сантиметри (см) - для коротких відрізків стрічок");
            Console.WriteLine("6. ✍️ Ввести свій варіант вручну");

            while (true)
            {
                int choice = InputValidator.ReadValidInt("Ваш вибір (1-6): ");
                switch (choice)
                {
                    case 1: return "грами";
                    case 2: return "штуки";
                    case 3: return "метри";
                    case 4: return "мілілітри";
                    case 5: return "сантиметри";
                    case 6: return InputValidator.ReadValidString("Введіть свою одиницю виміру (наприклад: мотки, пачки): ");
                    default:
                        Console.WriteLine("[ПОМИЛКА] Неправильний вибір. Введіть число від 1 до 6.");
                        break;
                }
            }
        }

        private void AddMaterial()
        {
            Console.Clear();
            Console.WriteLine("=== ДОДАВАННЯ НОВОГО МАТЕРІАЛУ ===\n");

            string name = InputValidator.ReadValidString("Введіть назву (наприклад, Бісер Miyuki Delica 11/0): ");
            decimal cost = InputValidator.ReadValidDecimal("Введіть вартість цілої упаковки при покупці (грн): ");

            string unit = ChooseUnitOfMeasurement();
            decimal quantity = InputValidator.ReadValidDecimal($"Введіть скільки {unit} в одній упаковці (для розрахунку ціни): ");

            Material newMaterial = new()
            {
                Id = Guid.NewGuid(),
                Name = name,
                PackagingCost = cost,
                TotalQuantity = quantity,
                AvailableQuantity = quantity,
                UnitOfMeasurement = unit
            };

            materials.Add(newMaterial);
            storage.SaveData(materials, products);
            Console.WriteLine($"\n✅ Успіх! Матеріал '{newMaterial.Name}' успішно додано на склад.");
        }

        private void ShowMaterials()
        {
            Console.Clear();
            Console.WriteLine("=== 📦 СТАН СКЛАДУ МАТЕРІАЛІВ ===\n");

            if (materials.Count == 0)
            {
                Console.WriteLine("Ваш склад поки що порожній.");
                Console.WriteLine("Додайте перші матеріали, щоб почати роботу.");
                return;
            }

            foreach (Material m in materials)
            {
                decimal costPerUnit = m.TotalQuantity > 0 ? m.PackagingCost / m.TotalQuantity : 0;

                Console.WriteLine($"> {m.Name}");
                Console.WriteLine($"  Куплено: {m.TotalQuantity} {m.UnitOfMeasurement} за {m.PackagingCost} грн (Собівартість: {costPerUnit:F2} грн/од)");

                if (m.AvailableQuantity <= 0)
                {
                    Console.WriteLine($"  ⚠️ В НАЯВНОСТІ: {m.AvailableQuantity} {m.UnitOfMeasurement} (Закінчився!)");
                }
                else
                {
                    Console.WriteLine($"  ✅ В НАЯВНОСТІ: {m.AvailableQuantity} {m.UnitOfMeasurement}");
                }
                Console.WriteLine("-----------------------------------");
            }
        }

        private void EditMaterial()
        {
            Console.Clear();
            Console.WriteLine("=== РЕДАГУВАННЯ МАТЕРІАЛУ ===\n");

            if (materials.Count == 0)
            {
                Console.WriteLine("Ваш склад порожній. Немає що редагувати.");
                return;
            }

            for (int i = 0; i < materials.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {materials[i].Name} (В наявності: {materials[i].AvailableQuantity} {materials[i].UnitOfMeasurement})");
            }

            int selectedIndex = InputValidator.ReadValidInt("\nВведіть номер матеріалу для редагування (або 0 для відміни): ");

            if (selectedIndex > 0 && selectedIndex <= materials.Count)
            {
                Material mat = materials[selectedIndex - 1];
                bool editing = true;

                while (editing)
                {
                    Console.Clear();
                    Console.WriteLine($"\n--- ✏️ Редагуємо: {mat.Name} ---");
                    Console.WriteLine($"1. Назва (Зараз: {mat.Name})");
                    Console.WriteLine($"2. Вартість упаковки (Зараз: {mat.PackagingCost} грн) - впливає на собівартість");
                    Console.WriteLine($"3. Одиниця виміру (Зараз: {mat.UnitOfMeasurement})");
                    Console.WriteLine($"4. Початкова кількість упаковки (Зараз: {mat.TotalQuantity}) - впливає на собівартість");
                    Console.WriteLine($"5. Поточний залишок на складі (Зараз: {mat.AvailableQuantity})");
                    Console.WriteLine("0. 💾 Зберегти зміни та вийти");
                    Console.Write("\nЩо саме ви хочете змінити? (0-5): ");

                    string? choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1": mat.Name = InputValidator.ReadValidString("Введіть нову назву: "); break;
                        case "2": mat.PackagingCost = InputValidator.ReadValidDecimal("Введіть нову вартість цілої упаковки (грн): "); break;
                        case "3": mat.UnitOfMeasurement = ChooseUnitOfMeasurement(); break;
                        case "4": mat.TotalQuantity = InputValidator.ReadValidDecimal("Введіть нову початкову кількість (для розрахунку ціни): "); break;
                        case "5": mat.AvailableQuantity = InputValidator.ReadValidDecimal("Введіть новий фактичний залишок на складі: "); break;
                        case "0": editing = false; break;
                        default:
                            Console.WriteLine("[ПОМИЛКА] Невідомий вибір. Натисніть Enter.");
                            Console.ReadLine();
                            break;
                    }

                    storage.SaveData(materials, products);
                }
                Console.WriteLine("\n✅ Успіх! Зміни збережено.");
            }
        }

        private void DeleteMaterial()
        {
            Console.Clear();
            Console.WriteLine("=== ВИДАЛЕННЯ МАТЕРІАЛУ ===\n");

            if (materials.Count == 0)
            {
                Console.WriteLine("Ваш склад порожній. Немає що видаляти.");
                return;
            }

            for (int i = 0; i < materials.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {materials[i].Name}");
            }

            int selectedIndex = InputValidator.ReadValidInt("\nВведіть номер матеріалу для видалення (або 0 для відміни): ");

            if (selectedIndex > 0 && selectedIndex <= materials.Count)
            {
                Material matToDelete = materials[selectedIndex - 1];

                bool isUsedInProducts = products.Any(p => p.MaterialsUsed.Any(um => um.MaterialId == matToDelete.Id));

                if (isUsedInProducts)
                {
                    Console.WriteLine($"\n❌ [ПОМИЛКА] Неможливо видалити '{matToDelete.Name}'!");
                    Console.WriteLine("Цей матеріал вже використовується у створених вами виробах.");
                    Console.WriteLine("Щоб його видалити, спочатку потрібно видалити виріб, який містить цей матеріал.");
                }
                else
                {
                    materials.Remove(matToDelete);
                    storage.SaveData(materials, products);
                    Console.WriteLine($"\n✅ Успіх! Матеріал '{matToDelete.Name}' назавжди видалено зі складу.");
                }
            }
        }

        private void CreateProduct()
        {
            Console.Clear();
            Console.WriteLine("=== СТВОРЕННЯ НОВОГО ВИРОБУ ===\n");

            if (materials.Count == 0)
            {
                Console.WriteLine("❌ Помилка: На складі немає матеріалів!");
                Console.WriteLine("Спочатку додайте матеріали через меню Складу.");
                return;
            }

            string productName = InputValidator.ReadValidString("Введіть назву виробу (наприклад, Силянка на зав'язках): ");
            decimal timeSpent = InputValidator.ReadValidDecimal("Скільки годин ви витратили на роботу? (наприклад, 2.5): ");
            decimal hourlyRate = InputValidator.ReadValidDecimal("Яка ваша ставка за годину роботи? (грн): ");

            decimal markupUAH = InputValidator.ReadValidDecimal("Введіть додаткову націнку у гривнях (пакування, тощо) або 0: ");
            decimal markupPercent = InputValidator.ReadValidDecimal("Введіть додаткову націнку у відсотках % (податок, маржа) або 0: ");

            Product newProduct = new()
            {
                Id = Guid.NewGuid(),
                Name = productName,
                TimeSpent = timeSpent,
                CostOfAnHourOfWork = hourlyRate,
                MarkUp = markupUAH,
                MarkUpPercentage = markupPercent
            };

            bool addingMaterials = true;
            while (addingMaterials)
            {
                List<Material> availableMaterials = [.. materials.Where(m => m.AvailableQuantity > 0)];

                if (availableMaterials.Count == 0)
                {
                    Console.WriteLine("\nℹ️ На складі закінчилися доступні матеріали!");
                    break;
                }

                Console.WriteLine("\n--- ДОДАВАННЯ МАТЕРІАЛІВ У ВИРІБ ---");
                Console.WriteLine("Ось що є на складі (доступно для вибору):");

                for (int i = 0; i < availableMaterials.Count; i++)
                {
                    var mat = availableMaterials[i];
                    var existingUsage = newProduct.MaterialsUsed.FirstOrDefault(um => um.MaterialId == mat.Id);

                    if (existingUsage != null)
                    {
                        Console.WriteLine($"{i + 1}. {mat.Name} (В наявності: {mat.AvailableQuantity} {mat.UnitOfMeasurement})  [Вже додано: {existingUsage.QuantitySpent}]");
                    }
                    else
                    {
                        Console.WriteLine($"{i + 1}. {mat.Name} (В наявності: {mat.AvailableQuantity} {mat.UnitOfMeasurement})");
                    }
                }
                Console.WriteLine("0. Завершити додавання матеріалів і розрахувати вартість");

                int selectedIndex = InputValidator.ReadValidInt("\nВведіть номер матеріалу (або 0 для завершення): ");

                if (selectedIndex == 0)
                {
                    addingMaterials = false;
                }
                else if (selectedIndex > 0 && selectedIndex <= availableMaterials.Count)
                {
                    Material selectedMaterial = availableMaterials[selectedIndex - 1];

                    decimal quantitySpent = 0;
                    while (true)
                    {
                        quantitySpent = InputValidator.ReadValidDecimal($"Скільки '{selectedMaterial.Name}' ви витратили? (Максимум {selectedMaterial.AvailableQuantity} {selectedMaterial.UnitOfMeasurement}): ");
                        if (quantitySpent <= selectedMaterial.AvailableQuantity)
                        {
                            break;
                        }
                        Console.WriteLine($"\n❌ [ПОМИЛКА] У вас немає стільки матеріалу на складі! В наявності лише {selectedMaterial.AvailableQuantity}.");
                    }

                    var existingItem = newProduct.MaterialsUsed.FirstOrDefault(um => um.MaterialId == selectedMaterial.Id);

                    if (existingItem != null)
                    {
                        existingItem.QuantitySpent += quantitySpent;
                    }
                    else
                    {
                        newProduct.MaterialsUsed.Add(new UsedMaterial
                        {
                            MaterialId = selectedMaterial.Id,
                            QuantitySpent = quantitySpent
                        });
                    }

                    selectedMaterial.AvailableQuantity -= quantitySpent;
                    Console.WriteLine("✅ Матеріал успішно додано до виробу, а залишки на складі оновлено!");
                }
                else
                {
                    Console.WriteLine("❌ Неправильний номер. Спробуйте ще раз.");
                }
            }

            PriceCalculator calc = new(newProduct, materials);
            decimal finalPrice = calc.CalculateFinalPrice();

            products.Add(newProduct);

            storage.SaveData(materials, products);

            Console.WriteLine($"\n✅ Успіх! Виріб '{newProduct.Name}' успішно створено.");
            Console.WriteLine($"💰 Рекомендована ціна для продажу: {finalPrice:F2} грн");
        }

        private void ShowProducts()
        {
            Console.Clear();
            Console.WriteLine("=== 💎 КАТАЛОГ ВАШИХ ВИРОБІВ (ДЕТАЛЬНИЙ КОШТОРИС) ===\n");

            if (products.Count == 0)
            {
                Console.WriteLine("Ви ще не створили жодного виробу.");
                Console.WriteLine("Перейдіть до меню створення виробів, щоб додати першу роботу.");
                return;
            }

            foreach (Product p in products)
            {
                PriceCalculator productCalculator = new(p, materials);

                decimal finalPrice = productCalculator.CalculateFinalPrice();
                decimal totalMaterialsCost = productCalculator.CalculateTotalMaterialsCost();
                decimal workTimeCost = productCalculator.CalculateWorkTime();

                Console.WriteLine($"\n> Виріб: {p.Name}");
                Console.WriteLine("  [Склад матеріалів]:");

                if (p.MaterialsUsed.Count == 0)
                {
                    Console.WriteLine("    Матеріали не додано.");
                }
                else
                {
                    foreach (UsedMaterial um in p.MaterialsUsed)
                    {
                        Material? mat = materials.FirstOrDefault(m => m.Id == um.MaterialId);

                        if (mat != null)
                        {
                            decimal itemCost = productCalculator.CalculateSingleMaterialCost(um);
                            Console.WriteLine($"    - {mat.Name}: витрачено {um.QuantitySpent} {mat.UnitOfMeasurement}. (Собівартість: {itemCost:F2} грн)");
                        }
                        else
                        {
                            Console.WriteLine($"    - [Матеріал був видалений зі складу]: {um.QuantitySpent} од.");
                        }
                    }
                }

                Console.WriteLine($"  ---------------------------------");
                Console.WriteLine($"  Загальна вартість матеріалів: {totalMaterialsCost:F2} грн");
                Console.WriteLine($"  Оплата роботи ({p.TimeSpent} год по {p.CostOfAnHourOfWork} грн/год): {workTimeCost:F2} грн");

                if (p.MarkUp > 0)
                {
                    Console.WriteLine($"  Додаткова націнка (фіксована): {p.MarkUp:F2} грн");
                }
                if (p.MarkUpPercentage > 0)
                {
                    Console.WriteLine($"  Додаткова націнка (відсоток): {p.MarkUpPercentage:F2} %");
                }

                Console.WriteLine($"  => 💰 ФІНАЛЬНА ЦІНА ДЛЯ КЛІЄНТА: {finalPrice:F2} грн");
                Console.WriteLine("===================================");
            }
        }

        private void EditProduct()
        {
            Console.Clear();
            Console.WriteLine("=== РЕДАГУВАННЯ ВИРОБУ ===\n");

            if (products.Count == 0)
            {
                Console.WriteLine("Ваш каталог виробів порожній.");
                return;
            }

            for (int i = 0; i < products.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {products[i].Name}");
            }

            int selectedIndex = InputValidator.ReadValidInt("\nВведіть номер виробу для редагування (або 0 для відміни): ");

            if (selectedIndex > 0 && selectedIndex <= products.Count)
            {
                Product prod = products[selectedIndex - 1];
                bool editing = true;

                while (editing)
                {
                    Console.Clear();
                    Console.WriteLine($"\n--- ✏️ Редагуємо виріб: {prod.Name} ---");
                    Console.WriteLine($"1. Назва (Зараз: {prod.Name})");
                    Console.WriteLine($"2. Витрачений час (Зараз: {prod.TimeSpent} год)");
                    Console.WriteLine($"3. Вартість години роботи (Зараз: {prod.CostOfAnHourOfWork} грн)");
                    Console.WriteLine($"4. Фіксована націнка (Зараз: {prod.MarkUp} грн)");
                    Console.WriteLine($"5. Націнка у відсотках (Зараз: {prod.MarkUpPercentage} %)");
                    Console.WriteLine($"6. 📦 [Керування матеріалами] (Матеріалів у виробі: {prod.MaterialsUsed.Count})");
                    Console.WriteLine("0. 💾 Зберегти зміни та вийти");
                    Console.Write("\nЩо саме ви хочете змінити? (0-6): ");

                    string? choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1": prod.Name = InputValidator.ReadValidString("Введіть нову назву: "); break;
                        case "2": prod.TimeSpent = InputValidator.ReadValidDecimal("Введіть новий час роботи: "); break;
                        case "3": prod.CostOfAnHourOfWork = InputValidator.ReadValidDecimal("Введіть нову вартість години роботи (грн): "); break;
                        case "4": prod.MarkUp = InputValidator.ReadValidDecimal("Введіть нову фіксовану націнку (грн): "); break;
                        case "5": prod.MarkUpPercentage = InputValidator.ReadValidDecimal("Введіть нову націнку у відсотках (%): "); break;
                        case "6": ManageProductMaterials(prod); break;
                        case "0": editing = false; break;
                        default:
                            Console.WriteLine("[ПОМИЛКА] Невідомий вибір. Натисніть Enter.");
                            Console.ReadLine();
                            break;
                    }

                    storage.SaveData(materials, products);
                }
                Console.WriteLine("\n✅ Успіх! Зміни збережено. Кошторис автоматично перераховано.");
            }
        }

        private void ManageProductMaterials(Product prod)
        {
            bool managing = true;
            while (managing)
            {
                Console.Clear();
                Console.WriteLine($"\n--- 📦 СКЛАД МАТЕРІАЛІВ У ВИРОБІ: {prod.Name} ---");

                if (prod.MaterialsUsed.Count == 0)
                {
                    Console.WriteLine("Матеріалів поки не додано.");
                }
                else
                {
                    for (int i = 0; i < prod.MaterialsUsed.Count; i++)
                    {
                        var um = prod.MaterialsUsed[i];
                        Material? mat = materials.FirstOrDefault(m => m.Id == um.MaterialId);
                        string matName = mat != null ? mat.Name : "[Видалений зі складу]";
                        string matUnit = mat != null ? mat.UnitOfMeasurement : "од.";

                        Console.WriteLine($"{i + 1}. {matName} — {um.QuantitySpent} {matUnit}");
                    }
                }

                Console.WriteLine("\n1. ➕ Додати новий матеріал зі складу (або збільшити кількість)");
                Console.WriteLine("2. ✏️ Змінити кількість використаного матеріалу");
                Console.WriteLine("3. ❌ Видалити матеріал з виробу (повернеться на склад)");
                Console.WriteLine("0. ⬅️ Назад до меню виробу");
                Console.Write("\nВаш вибір (0-3): ");

                string? choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        List<Material> available = [.. materials.Where(m => m.AvailableQuantity > 0)];
                        if (available.Count == 0)
                        {
                            Console.WriteLine("\nℹ️ На складі закінчилися матеріали.");
                            WaitForKeyPress();
                            break;
                        }

                        Console.WriteLine("\nДоступні матеріали на складі:");
                        for (int i = 0; i < available.Count; i++)
                        {
                            var mat = available[i];
                            var existingUsage = prod.MaterialsUsed.FirstOrDefault(um => um.MaterialId == mat.Id);

                            if (existingUsage != null)
                            {
                                Console.WriteLine($"{i + 1}. {mat.Name} (В наявності: {mat.AvailableQuantity})  [Вже у виробі: {existingUsage.QuantitySpent}]");
                            }
                            else
                            {
                                Console.WriteLine($"{i + 1}. {mat.Name} (В наявності: {mat.AvailableQuantity})");
                            }
                        }

                        int addIdx = InputValidator.ReadValidInt("\nВведіть номер матеріалу для додавання (або 0 для відміни): ");
                        if (addIdx > 0 && addIdx <= available.Count)
                        {
                            Material selected = available[addIdx - 1];

                            decimal qty = 0;
                            while (true)
                            {
                                qty = InputValidator.ReadValidDecimal($"Скільки '{selected.Name}' ви використали? (Максимум {selected.AvailableQuantity}): ");
                                if (qty <= selected.AvailableQuantity) break;
                                Console.WriteLine($"❌ [ПОМИЛКА] В наявності лише {selected.AvailableQuantity}.");
                            }

                            var existingItem = prod.MaterialsUsed.FirstOrDefault(um => um.MaterialId == selected.Id);
                            if (existingItem != null)
                            {
                                existingItem.QuantitySpent += qty;
                            }
                            else
                            {
                                prod.MaterialsUsed.Add(new UsedMaterial { MaterialId = selected.Id, QuantitySpent = qty });
                            }

                            selected.AvailableQuantity -= qty;
                            storage.SaveData(materials, products);
                            Console.WriteLine("✅ Матеріал успішно додано (або збільшено його кількість)!");
                            WaitForKeyPress();
                        }
                        break;

                    case "2":
                        if (prod.MaterialsUsed.Count == 0) break;
                        int editIdx = InputValidator.ReadValidInt("Введіть номер матеріалу зі списку вище (або 0): ");
                        if (editIdx > 0 && editIdx <= prod.MaterialsUsed.Count)
                        {
                            var selectedUm = prod.MaterialsUsed[editIdx - 1];
                            Material? mat = materials.FirstOrDefault(m => m.Id == selectedUm.MaterialId);

                            if (mat == null)
                            {
                                Console.WriteLine("❌ Матеріал не знайдено на складі. Зміна неможлива.");
                                WaitForKeyPress();
                                break;
                            }

                            decimal newQty = 0;
                            while (true)
                            {
                                newQty = InputValidator.ReadValidDecimal($"Введіть нову кількість (було {selectedUm.QuantitySpent}, доступно ще {mat.AvailableQuantity} на складі): ");
                                decimal difference = newQty - selectedUm.QuantitySpent;

                                if (difference > mat.AvailableQuantity)
                                {
                                    Console.WriteLine($"❌ [ПОМИЛКА] Недостатньо матеріалу на складі для такого збільшення! Ви можете додати максимум ще {mat.AvailableQuantity}.");
                                }
                                else
                                {
                                    mat.AvailableQuantity -= difference;
                                    selectedUm.QuantitySpent = newQty;
                                    storage.SaveData(materials, products);
                                    Console.WriteLine("✅ Кількість успішно змінено!");
                                    WaitForKeyPress();
                                    break;
                                }
                            }
                        }
                        break;

                    case "3":
                        if (prod.MaterialsUsed.Count == 0) break;
                        int delIdx = InputValidator.ReadValidInt("Введіть номер матеріалу для ВИДАЛЕННЯ з виробу (або 0): ");
                        if (delIdx > 0 && delIdx <= prod.MaterialsUsed.Count)
                        {
                            var umToRemove = prod.MaterialsUsed[delIdx - 1];
                            Material? matToRestore = materials.FirstOrDefault(m => m.Id == umToRemove.MaterialId);

                            if (matToRestore != null)
                            {
                                matToRestore.AvailableQuantity += umToRemove.QuantitySpent;
                                storage.SaveData(materials, products);
                            }

                            prod.MaterialsUsed.RemoveAt(delIdx - 1);
                            Console.WriteLine("✅ Матеріал видалено з виробу, залишки повернуто на склад.");
                            WaitForKeyPress();
                        }
                        break;

                    case "0":
                        managing = false;
                        break;
                }
            }
        }

        private void DeleteProduct()
        {
            Console.Clear();
            Console.WriteLine("=== ВИДАЛЕННЯ ВИРОБУ ===\n");

            if (products.Count == 0)
            {
                Console.WriteLine("Ваш каталог виробів порожній. Немає що видаляти.");
                return;
            }

            for (int i = 0; i < products.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {products[i].Name}");
            }

            int selectedIndex = InputValidator.ReadValidInt("\nВведіть номер виробу для видалення (або 0 для відміни): ");

            if (selectedIndex > 0 && selectedIndex <= products.Count)
            {
                Product productToDelete = products[selectedIndex - 1];

                foreach (var um in productToDelete.MaterialsUsed)
                {
                    Material? mat = materials.FirstOrDefault(m => m.Id == um.MaterialId);
                    if (mat != null)
                    {
                        mat.AvailableQuantity += um.QuantitySpent;
                    }
                }

                products.Remove(productToDelete);

                storage.SaveData(materials, products);

                Console.WriteLine($"\n✅ Успіх! Виріб '{productToDelete.Name}' назавжди видалено з каталогу.");
                Console.WriteLine("Усі матеріали, які були витрачені на цей виріб, успішно повернені на склад.");
            }
        }

        private void SaveAndExit()
        {
            Console.Clear();
            Console.WriteLine("💾 Збереження даних...");
            storage.SaveData(materials, products);
            _isRunning = false;
        }
    }
}
