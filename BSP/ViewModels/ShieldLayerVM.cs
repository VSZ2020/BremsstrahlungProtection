using BSP.Common;

namespace BSP.ViewModels;

public class ShieldLayerVM: BaseViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }

    public float Z { get; set; }

    public float Weight { get; set; }
    public float Density { get; set; }
    
    private float d = 1;

    /// <summary>
    /// Толщина [см]
    /// </summary>
    public float D { get => d; set { d = value; OnChanged(); } }

    /// <summary>
    /// Массовая толщина [г/см2]
    /// </summary>
    public float Dm
    {
        get { return D * Density; }
    }
}