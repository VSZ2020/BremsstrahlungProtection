// See https://aka.ms/new-console-template for more information
using BSP.DatabaseFiller;

var path = Path.Combine("Data", "DoseFactors");
var filler = new DatabaseFiller();

Console.WriteLine("Writing dose factors to database");
filler.FillDatabaseWithDoseFactors(path);

Console.WriteLine("Writing materials data to database");
path = Path.Combine("Data", "Materials");
filler.FillDatabaseWithMaterialsData(path);

Console.WriteLine("Writing radionuclides data to database");
path = Path.Combine("Data", "Radionuclides");
filler.FillDatabaseWithRadionuclidesData(path);

Console.WriteLine("Finished!");
Console.ReadKey();