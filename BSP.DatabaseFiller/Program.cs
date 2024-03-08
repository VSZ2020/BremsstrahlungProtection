// See https://aka.ms/new-console-template for more information
using BSP.DatabaseFiller;
using System.Reflection;

var root = 
    Directory.GetParent(
        Directory.GetParent(
            Directory.GetParent(
                Directory.GetParent(
                    Environment.CurrentDirectory).FullName).FullName).FullName).FullName;

var path = Path.Combine(root, "Data", "DoseFactors");
var filler = new DatabaseFiller(root);

Console.WriteLine("Writing dose factors to database");
filler.FillDatabaseWithDoseFactors(path);

Console.WriteLine("Writing materials data to database");
path = Path.Combine(root, "Data", "Materials");
filler.FillDatabaseWithMaterialsData(path);

Console.WriteLine("Writing radionuclides data to database");
path = Path.Combine(root, "Data", "Radionuclides");
filler.FillDatabaseWithRadionuclidesData(path);

Console.WriteLine("Finished!");
Console.ReadKey();