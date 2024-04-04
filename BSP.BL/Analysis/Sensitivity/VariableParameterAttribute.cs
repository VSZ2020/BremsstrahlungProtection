namespace BSP.BL.Analysis.Sensitivity
{
    /// <summary>
    /// Аттбирут, показывающий, что данное свойство можно варьировать
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class VariableParameterAttribute: Attribute
    {
        public string Name;
        public double MinValue;
        public double MaxValue;
        public double Step;
        public bool IsChangableStep = true;
    }
}
