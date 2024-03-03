using BSP.Common;

namespace BSP.ViewModels
{
    /// <summary>
    /// Модель, описывающая размер источника
    /// </summary>
    public class DimensionVM : BaseViewModel
    {
        private string name;
        private float value = 10;
        private int discreteness = 10;

        public string Name { get { return name; } set { name = value; OnChanged(); } }
        public float Value { get { return this.value; } set { this.value = value; OnChanged(); } }
        public int Discreteness { get { return discreteness; } set { discreteness = value; OnChanged(); } }

        /// <summary>
        /// Подсказка, отображаемая при наведении на объект
        /// </summary>
        public string Tooltip { get; set; }
    }
}
