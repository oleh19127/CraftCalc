# 🧮 CraftCalc

CraftCalc is a C# console application designed specifically for handmade artisans. It helps creators manage their material inventory and accurately calculate the final selling price of their products (such as bead necklaces, crocheted bags, or paintings) based on materials used, labor time, and custom markups.

## 🌟 Features

- **📦 Inventory Management**: Add, edit, and track materials (e.g., beads, yarn, clay, canvas) with specific units of measurement (grams, pieces, meters).
- **💎 Product Pricing Calculator**: Build products by selecting materials directly from your inventory.
- **⚙️ Automatic Stock Deduction**: When a material is used in a product, it is automatically subtracted from your total inventory. Returning a material restores the stock.
- **⏱️ Labor & Markup Integration**: Calculate final prices by factoring in hourly rates, fixed markups, and percentage-based margins.
- **💾 Local Storage**: Automatically saves all data locally in a structured JSON file (`craftcalc_data.json`), ensuring no data is lost between sessions.

## 🏗️ Architecture

The project is built using a clean, multi-layered architecture following the Single Responsibility Principle:

- `Model/`: Contains data structures (`Material`, `Product`, `UsedMaterial`).
- `Storage/`: Manages serialization and deserialization using `System.Text.Json`.
- `Calculator/`: Encapsulates the mathematical logic for pricing and costs.
- `UI/`: Handles user interaction, menus, and input validation.

## 🚀 How to Run

1. Make sure you have the [.NET SDK](https://dotnet.microsoft.com/) installed on your computer.
2. Clone this repository to your local machine.
3. Open your terminal or command prompt in the project folder.
4. Run the following command:

```bash
dotnet run
```
