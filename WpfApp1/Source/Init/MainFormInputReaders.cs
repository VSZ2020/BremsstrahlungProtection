using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BSP
{
	public partial class MainWindow: Window
	{
		public bool ReadInputDataFields(ref InputData data)
		{
			if (!CheckValidation()) return false;
			try
			{
				data.IncludeScattering = cbIncludeScattering.IsChecked == true;
				Calculation.IsPointSource = cbIsPointSource.IsChecked.Value;

				/*
				 * Radiation source have already been created in Initialization procedure
				 * and its Density and Z have been binded
				*/

				CalcParams.Source.Geometry = (ASourceForm)cbSourceGeometry.SelectedItem;
				CalcParams.Source.Geometry.X = bufferSourceSizeX;
				CalcParams.Source.Geometry.Y = bufferSourceSizeY;
				CalcParams.Source.Geometry.Z = bufferSourceSizeZ;

				//Calculation distance is passed

				//Source material Zeff and Density are passed

				//Read and add source material at first position
				CalcParams.Source.Substances = (cbSourceMaterial.SelectedItem as Material);

				data.Layers = (ShieldLayers)CalcParams.SelectedLayersList.Clone();
				data.Layers.InsertLayer(new MaterialLayer(1F, CalcParams.Source.Substances), 0);

				//Getting the dose factor
				FactorValue doseFactor = GetSelectedDoseFactor();

				CalcParams.Source.UpdateIb(ref IbetaMaxLabel);

				if (string.IsNullOrEmpty(CalcParams.generalNuclideKey))
				{
					throw new Exception("There is no general nuclide key!");
				}
				data.interpData = Interpolator.GetInterpolatedData(ref data, CalcParams.TableNuclides[CalcParams.generalNuclideKey].BSEnergySpectrum.MeanEnergies, doseFactor);
				data.RecalcUD();

				//Get calculation direction
				data.CalcDirection = (BSP.Source.Calculation.CalcDirections.ADirection)cbRadiationDirection.SelectedItem;

			}
			catch(Exception ex)
			{
				SendMessage(string.Format((string)Application.Current.Resources["msgError_TheLaunchError"], ex.Message), MessageLevel.Error);
				statusbarLabel.Text = (string)Application.Current.Resources["msgStatus_ValidationProcessError"];
				return false;
			}

			return true;
		}

		public FactorValue GetSelectedDoseFactor()
		{
			int Index = cbOutDoseType.SelectedIndex;

			if (Index == 0)	//Effective, Hp10, Exp dose items
			{
				var dose = (DoseFactor.DoseFactorData)cbDoseFactorGeometry.SelectedItem;
				return dose.Value[dose.Value.Count - 1].Factor ?? new FactorValue();
			}
			if (Index == 1)	//Equivalent dose item
			{
				var dose = cbEquivalentDoseOrgan.SelectedItem as DoseFactorWithOrganName;
				return dose.Factor ?? new FactorValue();
			}
			if (Index == 2 || Index == 3)
			{
				var dose = (DoseFactor)cbOutDoseType.SelectedItem;
				return dose.FactorData[0].Value[dose.FactorData[0].Value.Count - 1].Factor ?? new FactorValue();
			}
			else
			{
				//default air kerma with "1" coefficients
			}
				return new FactorValue();
		}


		public bool CheckValidation()
		{
			statusbarLabel.Text = (string)Application.Current.Resources["msgStatus_ValidationProcess"];

			bool flagValidationTrue = true;
			string msg = "";

			foreach (TextBox item in Descendants<TextBox>(this))
			{
				if (Validation.GetHasError(item) && item.Tag != null)
				{
					SendMessage(string.Format((string)Application.Current.Resources["msgOutputBox_ErrorIn"], (string)item.Tag), MessageLevel.Error);
					flagValidationTrue = false;
				}
			}

			return flagValidationTrue;
		}

		public static IEnumerable<T> Descendants<T>(DependencyObject dependencyItem) where T : DependencyObject
		{
			if (dependencyItem != null)
			{
				for (var index = 0; index < VisualTreeHelper.GetChildrenCount(dependencyItem); index++)
				{
					var child = VisualTreeHelper.GetChild(dependencyItem, index);
					if (child is T dependencyObject)
					{
						yield return dependencyObject;
					}

					foreach (var childOfChild in Descendants<T>(child))
					{
						yield return childOfChild;
					}
				}
			}
		}
	}
}
