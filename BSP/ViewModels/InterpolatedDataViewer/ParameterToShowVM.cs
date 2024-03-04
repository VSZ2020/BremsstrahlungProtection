namespace BSP.ViewModels.InterpolatedDataViewer
{
    public class ParameterToShowVM
    {
        public string Name { get; set; }

        public InterpolatedParameterType Type { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
