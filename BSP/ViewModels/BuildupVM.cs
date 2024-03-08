using BSP.BL.Buildups;
using BSP.BL.Buildups.Common;
using BSP.BL.Services;
using System.Windows;

namespace BSP.ViewModels
{
    public class BuildupVM
    {
        public string Name { get; set; }

        public Type BuildupType { get; set; }

        

        public static List<BuildupVM> Load()
        {
            return BuildupService.GetHomogeneousBuildups
                .Select(b => new BuildupVM()
                {
                    BuildupType = b.Key,
                    Name = (string)Application.Current.Resources[BuildupService.GetTranslationKeyHomogeneousBuildup(b.Key)] ?? b.Value,
                })
                .ToList();
        }
    }

    public class HeterogeneousBuildupVM
    {
        public string Name { get; set; }

        public Type BuildupType { get; set; }

        public string Description { get; set; }

        public static List<HeterogeneousBuildupVM> Load()
        {
            return BuildupService.GetHeterogeneousBuildups
                .Select(b => new HeterogeneousBuildupVM()
                {
                    BuildupType = b.Key,
                    Name = BuildupService.GetTranslationKeyHeterogeneousBuildup(b.Key) != null ? (string)Application.Current.Resources[BuildupService.GetTranslationKeyHeterogeneousBuildup(b.Key)] : b.Value,
                    Description = BuildupService.GetHeterogeneousBuildupInstance(b.Key, typeof(BuildupGeometricProgression)).Description,
                })
                .ToList();
        }
    }
}
