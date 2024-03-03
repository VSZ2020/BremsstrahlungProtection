/*
 * Created by SharpDevelop.
 * User: Slava Izgagin
 * Date: 29-Feb-20
 * Time: 01:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using BSP.BL.DTO;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BSP.BL.Materials
{
    /// <summary>
    /// Слой защиты
    /// </summary>
    public class ShieldLayer : MaterialDto, INotifyPropertyChanged
    {
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

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
