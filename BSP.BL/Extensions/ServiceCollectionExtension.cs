using BSP.BL.Calculation;
using BSP.BL.Services;
using BSP.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSP.BL.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddServices(this IServiceCollection collection)
        {
            var connectionString = "Data Source=Database.mdb";
            collection.AddDbContext<DataContext>(options => options.UseSqlite(connectionString))
                .AddTransient<BuildupService>()
                .AddTransient<DoseFactorsService>()
                .AddTransient<GeometryService>()
                .AddTransient<MaterialsService>()
                .AddTransient<RadionuclidesService>()
                .AddTransient<InputDataBuilder>()
                .BuildServiceProvider();
        }
    }
}
