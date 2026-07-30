using CraftCalc.Calculator;
using CraftCalc.Model;
using CraftCalc.Storage;
using Microsoft.EntityFrameworkCore;

namespace CraftCalc.UI
{
    public class ConsoleApp(AppDbContext context)
    {
        private readonly AppDbContext _context = context;
        private bool _isRunning;
        private bool _returnToMainMenu;

        public void Run()
        {
            _isRunning = true;
            while (_isRunning)
            {
                _returnToMainMenu = false;
                Console.Clear();
                Console.WriteLine(" ==========================================");
                Console.WriteLine("  🧮 CraftCalc: Калькулятор Хендмейду");
                Console.WriteLine(" ==========================================\n");
                Console.WriteLine("  1. 📦 Управління складом (Матеріали)");
                Console.WriteLine("  2. 💎 Каталог виробів (Кошториси та ціни)");
                Console.WriteLine("  0. 🚪 Вийти з програми");
                Console.Write("\n➡️  Оберіть розділ (0-2): ");

                string? choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": MaterialsMenu(); break;
                    case "2": ProductsMenu(); break;
                    case "0": _isRunning = false; break;
                    default: WaitForKeyPress("❌ Невідома команда. Будь ласка, оберіть цифру від 0 до 2."); break;
                }
            }
            Console.WriteLine("\n  Дякую за використання CraftCalc! Творчого натхнення! ✨\n");
        }
        private void MaterialsMenu()
        {
            while (true)
            {
                if (_returnToMainMenu) return;

                Console.Clear();
                Console.WriteLine(" === 📦 СКЛАД МАТЕРІАЛІВ ===\n");
                Console.WriteLine("  1. ➕ Додати новий матеріал");
                Console.WriteLine("  2. 📋 Переглянути наявні залишки");
                Console.WriteLine("  3. ✏️  Редагувати матеріал");
                Console.WriteLine("  4. ❌ Видалити матеріал");
                Console.WriteLine("  0. ⬅️  На Головне меню");
                Console.Write("\n  ➡️  Оберіть дію: ");

                switch (Console.ReadLine())
                {
                    case "1": AddMaterial(); break;
                    case "2": ShowMaterials(); WaitForKeyPress(); break;
                    case "3": EditMaterial(); break;
                    case "4": DeleteMaterial(); break;
                    case "0": return;
                    default: WaitForKeyPress("❌ Невідома команда."); break;
                }
            }
        }
        private void ProductsMenu()
        {
            while (true)
            {
                if (_returnToMainMenu) return;

                Console.Clear();
                Console.WriteLine(" === 💎 КАТАЛОГ ВИРОБІВ ===\n");
                Console.WriteLine("  1. ➕ Створити новий виріб (кошторис)");
                Console.WriteLine("  2. 📋 Переглянути всі вироби та ціни");
                Console.WriteLine("  3. ✏️  Редагувати існуючий виріб");
                Console.WriteLine("  4. ❌ Видалити виріб");
                Console.WriteLine("  0. ⬅️  На Головне меню");
                Console.Write("\n  ➡️  Оберіть дію: ");

                switch (Console.ReadLine())
                {
                    case "1": CreateProduct(); break;
                    case "2": ShowProducts(); WaitForKeyPress(); break;
                    case "3": EditProduct(); break;
                    case "4": DeleteProduct(); break;
                    case "0": return;
                    default: WaitForKeyPress("❌ Невідома команда."); break;
                }
            }
        }
        private static void WaitForKeyPress(string message = "")
        {
            if (!string.IsNullOrEmpty(message))
                Console.WriteLine($"\n  {message}");

            Console.WriteLine("\n  ⌨️  Натисніть Enter, щоб продовжити...");
            Console.ReadLine();
        }
        private static bool ConfirmAction(string prompt)
        {
            Console.WriteLine($"\n  ❓ {prompt}");
            Console.WriteLine("  1. ✅ Так, підтверджую");
            Console.WriteLine("  0. ❌ Ні, відмінити операцію");

            while (true)
            {
                string? choice = Console.ReadLine();
                if (choice == "1") return true;
                if (choice == "0") return false;
                Console.Write("  ➡️  Будь ласка, введіть 1 або 0: ");
            }
        }
        private static string ChooseUnitOfMeasurement()
        {
            Console.WriteLine("\n  📏 Оберіть одиницю виміру для матеріалу:");
            Console.WriteLine("  1. Грами (г)      - бісер, глина, пряжа");
            Console.WriteLine("  2. Штуки (шт)     - фурнітура, застібки");
            Console.WriteLine("  3. Метри (м)      - нитки, волосінь, ланцюжки");
            Console.WriteLine("  4. Мілілітри (мл) - фарба, лак");
            Console.WriteLine("  5. Сантиметри (см)- стрічки");
            Console.WriteLine("  6. ✍️  Ввести свій варіант вручну");

            while (true)
            {
                int choice = InputValidator.ReadValidInt("\n  ➡️  Ваш вибір (1-6): ");
                return choice switch
                {
                    1 => "г",
                    2 => "шт",
                    3 => "м",
                    4 => "мл",
                    5 => "см",
                    6 => InputValidator.ReadValidString("  ➡️  Введіть свою одиницю виміру: "),
                    _ => throw new Exception("Неправильний вибір.")
                };
            }
        }
        private void AddMaterial()
        {
            Console.Clear();
            Console.WriteLine(" === ➕ ДОДАВАННЯ НОВОГО МАТЕРІАЛУ ===\n");

            Material newMaterial = new()
            {
                Name = InputValidator.ReadValidString("  ➡️  Назва (наприклад, Бісер Miyuki Delica): "),
                PackagingCost = InputValidator.ReadValidDecimal("  ➡️  Вартість цілої упаковки (грн): "),
                UnitOfMeasurement = ChooseUnitOfMeasurement()
            };

            decimal quantity = InputValidator.ReadValidDecimal($"  ➡️  Скільки всього ({newMaterial.UnitOfMeasurement}) у цій упаковці: ");
            newMaterial.TotalQuantity = quantity;
            newMaterial.AvailableQuantity = quantity;

            if (ConfirmAction($"Зберегти '{newMaterial.Name}' на склад?"))
            {
                _context.Materials.Add(newMaterial);
                _context.SaveChanges();
                WaitForKeyPress($"✅ Успіх! Матеріал '{newMaterial.Name}' збережено на складі.");
            }
            else
            {
                WaitForKeyPress("🛑 Операцію скасовано. Матеріал не додано.");
            }
        }
        private void ShowMaterials()
        {
            Console.Clear();
            Console.WriteLine(" === 📋 СТАН СКЛАДУ МАТЕРІАЛІВ ===\n");

            var materials = _context.Materials.AsNoTracking().ToList();
            if (materials.Count == 0)
            {
                Console.WriteLine("  📭 Ваш склад поки що порожній.");
                return;
            }

            foreach (var m in materials)
            {
                decimal costPerUnit = m.TotalQuantity > 0 ? m.PackagingCost / m.TotalQuantity : 0;
                Console.WriteLine($"  🔹 {m.Name}");
                Console.WriteLine($"     Собівартість: {costPerUnit:F2} грн / 1 {m.UnitOfMeasurement}");

                if (m.AvailableQuantity <= 0)
                    Console.WriteLine($"     ⚠️ ЗАЛИШОК: {m.AvailableQuantity} {m.UnitOfMeasurement} (Закінчився!)");
                else
                    Console.WriteLine($"     ✅ ЗАЛИШОК: {m.AvailableQuantity} {m.UnitOfMeasurement}");

                Console.WriteLine("  -----------------------------------");
            }
        }
        private void EditMaterial()
        {
            while (true)
            {
                if (_returnToMainMenu) return;

                Console.Clear();
                Console.WriteLine(" === ✏️  ВИБІР МАТЕРІАЛУ ДЛЯ РЕДАГУВАННЯ ===\n");
                var materials = _context.Materials.ToList();
                if (materials.Count == 0) { WaitForKeyPress("📭 Склад порожній."); return; }

                for (int i = 0; i < materials.Count; i++)
                    Console.WriteLine($"  {i + 1}. {materials[i].Name} (Залишок: {materials[i].AvailableQuantity} {materials[i].UnitOfMeasurement})");

                Console.WriteLine("\n  0. ⬅️  Назад");
                Console.WriteLine("  9. 🏠 На Головне меню");

                int idx = InputValidator.ReadValidInt("\n  ➡️  Оберіть номер матеріалу: ");
                if (idx == 0) return;
                if (idx == 9) { _returnToMainMenu = true; return; }

                if (idx > 0 && idx <= materials.Count)
                {
                    EditSingleMaterialMenu(materials[idx - 1]);
                }
            }
        }
        private void EditSingleMaterialMenu(Material mat)
        {
            while (true)
            {
                if (_returnToMainMenu) return;

                Console.Clear();
                Console.WriteLine($" === ✏️  РЕДАГУВАННЯ: {mat.Name} ===\n");
                Console.WriteLine($"  1. Назва: {mat.Name}");
                Console.WriteLine($"  2. Вартість упаковки: {mat.PackagingCost} грн");
                Console.WriteLine($"  3. Змінити залишок вручну (Зараз: {mat.AvailableQuantity} {mat.UnitOfMeasurement})");
                Console.WriteLine("\n  0. ⬅️  Назад до списку матеріалів");
                Console.WriteLine("  9. 🏠 На Головне меню");

                Console.Write("\n  ➡️  Що хочете змінити?: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        mat.Name = InputValidator.ReadValidString("  ➡️  Нова назва: ");
                        _context.SaveChanges();
                        break;
                    case "2":
                        mat.PackagingCost = InputValidator.ReadValidDecimal("  ➡️ Нова вартість (грн): ");
                        _context.SaveChanges();
                        break;
                    case "3":
                        Console.WriteLine("\n  ⚠️ УВАГА: зміна залишку тут не впливає на вже створені вироби.");
                        mat.AvailableQuantity = InputValidator.ReadValidDecimal($"  ➡️  Новий залишок ({mat.UnitOfMeasurement}): ");
                        _context.SaveChanges();
                        break;
                    case "0": return;
                    case "9": _returnToMainMenu = true; return;
                    default: WaitForKeyPress("❌ Невідома команда."); break;
                }
            }
        }
        private void DeleteMaterial()
        {
            while (true)
            {
                if (_returnToMainMenu) return;

                Console.Clear();
                Console.WriteLine(" === ❌ ВИДАЛЕННЯ МАТЕРІАЛУ ===\n");
                var materials = _context.Materials.ToList();
                if (materials.Count == 0) { WaitForKeyPress("📭 Склад порожній."); return; }

                for (int i = 0; i < materials.Count; i++)
                    Console.WriteLine($"  {i + 1}. {materials[i].Name}");

                Console.WriteLine("\n  0. ⬅️  Назад");
                Console.WriteLine("  9. 🏠 На Головне меню");

                int idx = InputValidator.ReadValidInt("\n  ➡️  Оберіть номер для видалення: ");
                if (idx == 0) return;
                if (idx == 9) { _returnToMainMenu = true; return; }

                if (idx > 0 && idx <= materials.Count)
                {
                    var mat = materials[idx - 1];
                    bool isUsed = _context.UsedMaterials.Any(um => um.MaterialId == mat.Id);

                    if (isUsed)
                    {
                        WaitForKeyPress($"❌ Неможливо видалити '{mat.Name}'.\n  Він використовується у виробах! Спочатку видаліть його з виробів.");
                    }
                    else
                    {
                        if (ConfirmAction($"Ви дійсно хочете назавжди видалити '{mat.Name}'?"))
                        {
                            _context.Materials.Remove(mat);
                            _context.SaveChanges();
                            WaitForKeyPress($"✅ Матеріал '{mat.Name}' успішно видалено.");
                        }
                    }
                }
            }
        }
        private void CreateProduct()
        {
            Console.Clear();
            Console.WriteLine(" === ➕ СТВОРЕННЯ НОВОГО ВИРОБУ ===\n");

            if (!_context.Materials.Any(m => m.AvailableQuantity > 0))
            {
                WaitForKeyPress("❌ На складі немає матеріалів. Спочатку додайте їх у розділі 'Склад'!");
                return;
            }

            Product newProduct = new()
            {
                Name = InputValidator.ReadValidString("  ➡️  Назва виробу: "),
                TimeSpent = InputValidator.ReadValidDecimal("  ➡️  Витрачено годин на роботу: "),
                CostOfAnHourOfWork = InputValidator.ReadValidDecimal("  ➡️  Ставка за годину (грн): "),
                MarkUp = InputValidator.ReadValidDecimal("  ➡️  Фіксована націнка (пакування тощо, грн): "),
                MarkUpPercentage = InputValidator.ReadValidDecimal("  ➡️  Відсоткова націнка (% маржа): ")
            };

            if (!ConfirmAction("Зберегти основу виробу та перейти до додавання матеріалів?"))
            {
                WaitForKeyPress("🛑 Створення виробу скасовано.");
                return;
            }

            _context.Products.Add(newProduct);
            _context.SaveChanges();

            AddMaterialsToProductLoop(newProduct);

            var savedProduct = _context.Products
                .Include(p => p.MaterialsUsed).ThenInclude(um => um.Material)
                .First(p => p.Id == newProduct.Id);

            PriceCalculator calc = new(savedProduct);
            Console.WriteLine($"\n  ✅ Виріб '{savedProduct.Name}' успішно створено та укомплектовано!");
            WaitForKeyPress($"  💰 Рекомендована ціна продажу: {calc.CalculateFinalPrice():F2} грн");
        }
        private void AddMaterialsToProductLoop(Product product)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($" --- 🧩 ДОДАВАННЯ МАТЕРІАЛІВ ДО: {product.Name} ---\n");

                var usedItems = _context.UsedMaterials.Include(um => um.Material).Where(um => um.ProductId == product.Id).ToList();
                if (usedItems.Count != 0)
                {
                    Console.WriteLine("  📥 Вже додано до виробу:");
                    foreach (var ui in usedItems)
                        Console.WriteLine($"    - {ui.Material?.Name}: {ui.QuantitySpent} {ui.Material?.UnitOfMeasurement}");
                    Console.WriteLine();
                }

                var availableMats = _context.Materials.Where(m => m.AvailableQuantity > 0).ToList();
                if (availableMats.Count == 0)
                {
                    WaitForKeyPress("⚠️ Більше немає доступних матеріалів на складі.");
                    return;
                }

                Console.WriteLine("  📦 Доступні на складі:");
                for (int i = 0; i < availableMats.Count; i++)
                    Console.WriteLine($"  {i + 1}. {availableMats[i].Name} (Є: {availableMats[i].AvailableQuantity} {availableMats[i].UnitOfMeasurement})");

                Console.WriteLine("\n  0. ⬅️  Завершити додавання матеріалів");

                int idx = InputValidator.ReadValidInt("\n  ➡️  Оберіть номер матеріалу (або 0 для завершення): ");
                if (idx == 0) return;

                if (idx > 0 && idx <= availableMats.Count)
                {
                    var mat = availableMats[idx - 1];
                    decimal qty = InputValidator.ReadValidDecimal($"  ➡️  Скільки витрачено? (Макс {mat.AvailableQuantity} {mat.UnitOfMeasurement}): ");

                    if (qty <= 0) continue;

                    if (qty <= mat.AvailableQuantity)
                    {
                        var existingUse = usedItems.FirstOrDefault(u => u.MaterialId == mat.Id);
                        if (existingUse != null)
                        {
                            existingUse.QuantitySpent += qty;
                        }
                        else
                        {
                            _context.UsedMaterials.Add(new UsedMaterial { ProductId = product.Id, MaterialId = mat.Id, QuantitySpent = qty });
                        }

                        mat.AvailableQuantity -= qty;
                        _context.SaveChanges();

                        Console.WriteLine($"\n  ✅ Успіх! Додано {qty} {mat.UnitOfMeasurement} '{mat.Name}'. Залишок на складі оновлено.");
                        Thread.Sleep(1500);
                    }
                    else
                    {
                        WaitForKeyPress($"❌ Помилка: На складі лише {mat.AvailableQuantity} {mat.UnitOfMeasurement}, а ви намагаєтесь списати {qty}.");
                    }
                }
            }
        }
        private void ShowProducts()
        {
            Console.Clear();
            Console.WriteLine(" === 📋 КАТАЛОГ ВАШИХ ВИРОБІВ ===\n");

            var products = _context.Products.AsNoTracking()
                .Include(p => p.MaterialsUsed).ThenInclude(um => um.Material)
                .ToList();

            if (products.Count == 0)
            {
                Console.WriteLine("  📭 Каталог порожній.");
                return;
            }

            foreach (var p in products)
            {
                PriceCalculator calc = new(p);
                Console.WriteLine($"  💎 Виріб: {p.Name}");
                Console.WriteLine("     [Склад]:");

                if (p.MaterialsUsed.Count == 0)
                {
                    Console.WriteLine("       Матеріали не вказані.");
                }
                else
                {
                    foreach (var um in p.MaterialsUsed)
                    {
                        string matName = um.Material?.Name ?? "[Видалено]";
                        string unit = um.Material?.UnitOfMeasurement ?? "од.";
                        decimal cost = PriceCalculator.CalculateSingleMaterialCost(um);
                        Console.WriteLine($"       - {matName}: {um.QuantitySpent} {unit} ({cost:F2} грн)");
                    }
                }

                Console.WriteLine($"     ---------------------------------");
                Console.WriteLine($"     Матеріали: {calc.CalculateTotalMaterialsCost():F2} грн");
                Console.WriteLine($"     Робота ({p.TimeSpent} год): {calc.CalculateWorkTime():F2} грн");
                Console.WriteLine($"     Націнки: {p.MarkUp} грн | {p.MarkUpPercentage}%");
                Console.WriteLine($"     => 💰 ФІНАЛЬНА ЦІНА: {calc.CalculateFinalPrice():F2} грн\n");
            }
        }
        private void EditProduct()
        {
            while (true)
            {
                if (_returnToMainMenu) return;

                Console.Clear();
                Console.WriteLine(" === ✏️  ВИБІР ВИРОБУ ДЛЯ РЕДАГУВАННЯ ===\n");
                var products = _context.Products.Include(p => p.MaterialsUsed).ToList();

                if (products.Count == 0) { WaitForKeyPress("📭 Каталог порожній."); return; }

                for (int i = 0; i < products.Count; i++)
                    Console.WriteLine($"  {i + 1}. {products[i].Name}");

                Console.WriteLine("\n  0. ⬅️  Назад");
                Console.WriteLine("  9. 🏠 На Головне меню");

                int idx = InputValidator.ReadValidInt("\n  ➡️  Оберіть виріб: ");
                if (idx == 0) return;
                if (idx == 9) { _returnToMainMenu = true; return; }

                if (idx > 0 && idx <= products.Count)
                {
                    EditSingleProductMenu(products[idx - 1]);
                }
            }
        }
        private void EditSingleProductMenu(Product prod)
        {
            while (true)
            {
                if (_returnToMainMenu) return;

                Console.Clear();

                var freshProd = _context.Products
                    .Include(p => p.MaterialsUsed).ThenInclude(m => m.Material)
                    .First(p => p.Id == prod.Id);

                PriceCalculator calc = new(freshProd);

                Console.WriteLine($" === ✏️  РЕДАГУВАННЯ: {freshProd.Name} ===");
                Console.WriteLine($"  💰 Поточна ціна: {calc.CalculateFinalPrice():F2} грн\n");

                Console.WriteLine($"  1. Назва: {freshProd.Name}");
                Console.WriteLine($"  2. Витрачений час: {freshProd.TimeSpent} год");
                Console.WriteLine($"  3. Ставка за годину: {freshProd.CostOfAnHourOfWork} грн");
                Console.WriteLine($"  4. Фіксована націнка: {freshProd.MarkUp} грн");
                Console.WriteLine($"  5. Відсоткова націнка: {freshProd.MarkUpPercentage}%");
                Console.WriteLine($"  --- Робота з матеріалами виробу ---");
                Console.WriteLine($"  6. ➕ Додати новий матеріал");
                Console.WriteLine($"  7. 🔄 Змінити кількість матеріалу");
                Console.WriteLine($"  8. ❌ Видалити матеріал з виробу");
                Console.WriteLine("\n  0. ⬅️  Назад до списку виробів");
                Console.WriteLine("  9. 🏠 На Головне меню");

                Console.Write("\n  ➡️  Що хочете змінити?: ");

                switch (Console.ReadLine())
                {
                    case "1": freshProd.Name = InputValidator.ReadValidString("  ➡️  Нова назва: "); _context.SaveChanges(); break;
                    case "2": freshProd.TimeSpent = InputValidator.ReadValidDecimal("  ➡️  Новий час (год): "); _context.SaveChanges(); break;
                    case "3": freshProd.CostOfAnHourOfWork = InputValidator.ReadValidDecimal("  ➡️  Нова ставка (грн): "); _context.SaveChanges(); break;
                    case "4": freshProd.MarkUp = InputValidator.ReadValidDecimal("  ➡️  Нова націнка (грн): "); _context.SaveChanges(); break;
                    case "5": freshProd.MarkUpPercentage = InputValidator.ReadValidDecimal("  ➡️  Нова націнка (%): "); _context.SaveChanges(); break;
                    case "6": AddMaterialsToProductLoop(freshProd); break;
                    case "7": EditMaterialQuantityInProduct(freshProd); break;
                    case "8": RemoveMaterialFromProduct(freshProd); break;
                    case "0": return;
                    case "9": _returnToMainMenu = true; return;
                    default: WaitForKeyPress("❌ Невідома команда."); break;
                }
            }
        }
        private void EditMaterialQuantityInProduct(Product product)
        {
            if (product.MaterialsUsed.Count == 0)
            {
                WaitForKeyPress("У цьому виробі ще немає матеріалів.");
                return;
            }

            Console.Clear();
            Console.WriteLine($" --- 🔄 ЗМІНА КІЛЬКОСТІ МАТЕРІАЛУ У '{product.Name}' ---\n");
            for (int i = 0; i < product.MaterialsUsed.Count; i++)
            {
                var um = product.MaterialsUsed[i];
                Console.WriteLine($"  {i + 1}. {um.Material?.Name} (Зараз використано: {um.QuantitySpent} {um.Material?.UnitOfMeasurement})");
            }

            int idx = InputValidator.ReadValidInt("\n  ➡️  Оберіть номер матеріалу (0 для відміни): ");
            if (idx > 0 && idx <= product.MaterialsUsed.Count)
            {
                var usedItem = product.MaterialsUsed[idx - 1];
                var dbMaterial = _context.Materials.Find(usedItem.MaterialId);
                if (dbMaterial == null) return;

                Console.WriteLine($"\n  📊 СТАТИСТИКА:");
                Console.WriteLine($"  - Вже використано у виробі: {usedItem.QuantitySpent} {dbMaterial.UnitOfMeasurement}");
                Console.WriteLine($"  - Вільно лежить на складі:  {dbMaterial.AvailableQuantity} {dbMaterial.UnitOfMeasurement}");

                decimal newQty = InputValidator.ReadValidDecimal("\n  ➡️  Введіть НОВУ ЗАГАЛЬНУ кількість для цього виробу: ");

                if (newQty == usedItem.QuantitySpent) return;

                decimal difference = newQty - usedItem.QuantitySpent;

                if (difference > 0)
                {
                    if (dbMaterial.AvailableQuantity >= difference)
                    {
                        dbMaterial.AvailableQuantity -= difference;
                        usedItem.QuantitySpent = newQty;
                        _context.SaveChanges();
                        WaitForKeyPress($"✅ Успіх! Додано ще {difference} {dbMaterial.UnitOfMeasurement}. Залишки на складі оновлено.");
                    }
                    else
                    {
                        WaitForKeyPress($"❌ ПОМИЛКА: Не вистачає на складі!\n  Вам потрібно ще {difference} {dbMaterial.UnitOfMeasurement}, а є лише {dbMaterial.AvailableQuantity}.");
                    }
                }
                else
                {
                    decimal amountToReturn = Math.Abs(difference);
                    dbMaterial.AvailableQuantity += amountToReturn;
                    usedItem.QuantitySpent = newQty;
                    _context.SaveChanges();
                    WaitForKeyPress($"✅ Успіх! Зменшено кількість. На склад повернуто {amountToReturn} {dbMaterial.UnitOfMeasurement}.");
                }
            }
        }
        private void RemoveMaterialFromProduct(Product product)
        {
            if (product.MaterialsUsed.Count == 0)
            {
                WaitForKeyPress("У цьому виробі немає доданих матеріалів.");
                return;
            }

            Console.Clear();
            Console.WriteLine($" --- ❌ ВИДАЛЕННЯ МАТЕРІАЛУ З '{product.Name}' ---\n");
            for (int i = 0; i < product.MaterialsUsed.Count; i++)
            {
                var um = product.MaterialsUsed[i];
                Console.WriteLine($"  {i + 1}. {um.Material?.Name} ({um.QuantitySpent} {um.Material?.UnitOfMeasurement})");
            }

            int idx = InputValidator.ReadValidInt("\n  ➡️  Оберіть номер для видалення (0 для відміни): ");
            if (idx > 0 && idx <= product.MaterialsUsed.Count)
            {
                var usedItem = product.MaterialsUsed[idx - 1];
                var dbMaterial = _context.Materials.Find(usedItem.MaterialId);

                if (ConfirmAction($"Видалити '{dbMaterial?.Name}' з цього виробу?"))
                {
                    dbMaterial?.AvailableQuantity += usedItem.QuantitySpent;

                    _context.UsedMaterials.Remove(usedItem);
                    _context.SaveChanges();
                    WaitForKeyPress("✅ Матеріал видалено з виробу, залишки успішно повернуто на склад.");
                }
            }
        }
        private void DeleteProduct()
        {
            while (true)
            {
                if (_returnToMainMenu) return;

                Console.Clear();
                Console.WriteLine(" === ❌ ВИДАЛЕННЯ ВИРОБУ ===\n");
                var products = _context.Products.Include(p => p.MaterialsUsed).ToList();

                if (products.Count == 0) { WaitForKeyPress("📭 Каталог порожній."); return; }

                for (int i = 0; i < products.Count; i++)
                    Console.WriteLine($"  {i + 1}. {products[i].Name}");

                Console.WriteLine("\n  0. ⬅️  Назад");
                Console.WriteLine("  9. 🏠 На Головне меню");

                int idx = InputValidator.ReadValidInt("\n  ➡️  Оберіть виріб для видалення: ");
                if (idx == 0) return;
                if (idx == 9) { _returnToMainMenu = true; return; }

                if (idx > 0 && idx <= products.Count)
                {
                    var product = products[idx - 1];

                    if (ConfirmAction($"Ви дійсно хочете видалити виріб '{product.Name}'?\n  Усі витрачені матеріали будуть автоматично повернуті на склад."))
                    {
                        foreach (var um in product.MaterialsUsed)
                        {
                            var mat = _context.Materials.Find(um.MaterialId);
                            mat?.AvailableQuantity += um.QuantitySpent;
                        }
                        _context.Products.Remove(product);
                        _context.SaveChanges();
                        WaitForKeyPress($"✅ Виріб '{product.Name}' успішно видалено. Матеріали повернено.");
                    }
                }
            }
        }
    }
}
