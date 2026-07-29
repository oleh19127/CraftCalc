using System.Text;
using CraftCalc.Storage;
using CraftCalc.UI;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

StorageManager storage = new();

var (materials, products) = storage.LoadData();

Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e)
{
    Console.WriteLine("\n[УВАГА] Екстрене завершення роботи. Зберігаємо дані...");
    storage.SaveData(materials, products);
};

AppDomain.CurrentDomain.ProcessExit += delegate (object? sender, EventArgs e)
{
    storage.SaveData(materials, products);
};

ConsoleApp app = new(storage, materials, products);
app.Run();
