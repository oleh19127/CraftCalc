using System.Text;
using CraftCalc.Storage;
using CraftCalc.UI;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

using AppDbContext dbContext = new();
dbContext.Database.EnsureCreated();

ConsoleApp app = new(dbContext);
app.Run();
