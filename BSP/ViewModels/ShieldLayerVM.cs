using BSP.BL.Analysis.Sensitivity;
using BSP.Common;

namespace BSP.ViewModels;

public class ShieldLayerVM : BaseViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }

    [VariableParameter(MinValue = 1, MaxValue = 200, Step = 1)]
    public float Z { get; set; }

    [VariableParameter(MinValue = 1, Step = 0.5)]
    public float Weight { get; set; }

    [VariableParameter(MinValue = 0, Step = 0.1)]
    public float Density { get; set; }

    private float d = 1;

    /// <summary>
    /// Толщина [см]
    /// </summary
    [VariableParameter(MinValue = 0, Step = 0.1)]
    public float D { get => d; set { d = value; OnChanged(); } }

    /// <summary>
    /// Массовая толщина [г/см2]
    /// </summary>
    public float Dm
    {
        get { return D * Density; }
    }
}