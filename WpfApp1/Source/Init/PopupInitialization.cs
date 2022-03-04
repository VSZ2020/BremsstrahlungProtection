using System;
using System.Windows;
using System.Windows.Controls;

namespace BSP
{
	public partial class MainWindow: Window
	{
		public enum PopupMode
		{
			Add, Change
		}

		/// <summary>
		/// Инициирует всплывающее меню добавления нуклида
		/// </summary>
		public void InitializePopupMenu()
		{
			//Popup ADD NUCLIDE
			popupComboBox.ItemsSource = CalcParams.TableNuclides.Collection;
			if (popupComboBox.Items.Count > 0) popupComboBox.SelectedIndex = 0;

			btnPopupAcceptNuclide.Click += BtnPopupAcceptNuclide_Click;
			btnPopupCancelNuclide.Click += (object sender, RoutedEventArgs e) =>
			{
				popupAddNuclide.IsOpen = false;
				//popupComboBox.SelectedIndex = 0;
				//popupTextBox.Text = "0.0E+000";
			};


			//Popup ADD LAYER
			cbPopupAddLayer.ItemsSource = CalcParams.TableMaterials.Collection;
			cbPopupAddLayer.SelectionChanged += (object sender, SelectionChangedEventArgs e) =>
			{
				var mat = cbPopupAddLayer.SelectedItem as Material;
				//tbPopupD.Text = "1.0";
				tbPopupDensity.Text = mat.Density.ToString();
			};

			if (cbPopupAddLayer.Items.Count > 0) cbPopupAddLayer.SelectedIndex = 0;

			btnPopupAcceptLayer.Click += BtnPopupAcceptLayer_Click;
			btnPopupCancelLayer.Click += (object sender, RoutedEventArgs e) =>
			{
				popupAddLayer.IsOpen = false;
				cbPopupAddLayer.SelectedIndex = 0;

			};
		}

		private void BtnPopupAcceptLayer_Click(object sender, RoutedEventArgs e)
		{
			var mat = (cbPopupAddLayer.SelectedItem as Material);
			if (mat == null) return;

			try
			{
				var layer = new MaterialLayer(mat);
				double bufValue = layer.d;
				if (!double.TryParse(tbPopupD.Text, out bufValue)) throw new Exception((string)Application.Current.Resources["msgError_IncorrectValueFormat"]);
				layer.d = bufValue;

				bufValue = layer.Density;
				if (!double.TryParse(tbPopupDensity.Text, out bufValue)) throw new Exception((string)Application.Current.Resources["msgError_IncorrectValueFormat"]);
				layer.Density = bufValue;

				if (((OperationType)popupAddLayer.Tag) == OperationType.Add)
				{
					CalcParams.SelectedLayersList.AddLayer(layer);
				}
				if (((OperationType)popupAddLayer.Tag) == OperationType.Edit)
				{
					CalcParams.SelectedLayersList.ReplaceLayer(lbShielLayers.SelectedIndex, layer);
				}
				
				popupAddLayer.IsOpen = false;
			}
			catch (FormatException ex)
			{
				popupAddLayer.IsOpen = false;
				MessageBox.Show(
					ex.Message,
					(string)Application.Current.Resources["msgError_Title"],
					MessageBoxButton.OK, MessageBoxImage.Error);
				popupAddLayer.IsOpen = true;
				// TODO: Add writing additional "ex" info to log
			}
		}

		private void BtnPopupAcceptNuclide_Click(object sender, RoutedEventArgs e)
		{
			var nuc = (popupComboBox.SelectedItem as Nuclide);

			try
			{
				double bufActivity = 0;
				if (!double.TryParse(popupTextBox.Text, out bufActivity)) throw new Exception((string)Application.Current.Resources["msgError_IncorrectValueFormat"]);
				nuc.Activity = bufActivity;

				if ((OperationType)popupAddNuclide.Tag == OperationType.Add)
				{
					if (CalcParams.Source.Radionuclides.Collection.Contains(nuc))
						throw new Exception(string.Format((string)Application.Current.Resources["msgError_NuclideAlreadyExists"], nuc.Name));

					CalcParams.Source.Radionuclides.AddNuclide(nuc);
				}
				if ((OperationType)popupAddNuclide.Tag == OperationType.Edit)
				{
					CalcParams.Source.Radionuclides.ReplaceNuclide(lbNuclides.SelectedIndex, nuc);
					lbNuclides.Items.Refresh();
				}

				RecalcSumActivity();
				popupAddNuclide.IsOpen = false;
			}

			catch (Exception ex)
			{
				popupAddNuclide.IsOpen = false;
				MessageBox.Show(
					ex.Message,
					(string)Application.Current.Resources["msgWarning_Title"],
					MessageBoxButton.OK, MessageBoxImage.Warning);
				popupAddNuclide.IsOpen = true;
				// TODO: Add writing additional "ex" info to log
			}
		}
	}
}
