using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;

namespace BSP
{
	/// <summary>
	/// Описывает набор слоев защиты
	/// </summary>
	public class ShieldLayers: ICloneable
	{
		private ObservableCollection<MaterialLayer> _Layers;

		/// <summary>
		/// Доступ к слою защиты по индексу
		/// </summary>
		/// <param name="Index">Индекс элемента</param>
		/// <returns></returns>
		public MaterialLayer this[int Index]
		{
			get 
			{
				if (Index > _Layers.Count || Index < 0) throw new IndexOutOfRangeException("Индекс выбираемого слоя вне диапазона. Index =  " + Index + ". Length = " + _Layers.Count);
				return _Layers[Index];
			}
			/*
			set
			{
				if (Index > _Layers.Length || Index < 0) throw new IndexOutOfRangeException("Индекс выбираемого слоя вне диапазона. Index =  " + Index + ". Length = " + _Layers.Length);
				if (value == null) throw new NullReferenceException("Ссылка на присваиваемый объект отсутствует!");
				_Layers[Index] = value;
			}
			*/
		}

		public ObservableCollection<MaterialLayer> Collection
		{
			get
			{
				return _Layers;
			}
		}

		/// <summary>
		/// Возвращает количество слоев защиты, включая слой источника
		/// </summary>
		public int Count
		{
			get { return _Layers?.Count ?? 0; }
		}

		/// <summary>
		/// Возвращает суммарную толщину всех слоев защиты, исключая слой источника [см]
		/// </summary>
		public double D
		{
			get
			{
				double bufValue = 0F;
				for (int i = 1; i < _Layers.Count; i++)
					bufValue += _Layers[i].d;
				return bufValue;
			}
		}

		/// <summary>
		/// Возвращает суммарную массовую толщину всех слоев защиты, исключая слой источника [г/см2]
		/// </summary>
		public double Dm
		{
			get
			{
				double bufValue = 0F;
				for (int i = 1; i < _Layers.Count; i++)
					bufValue += _Layers[i].dm;
				return bufValue;
			}
		}

		public ShieldLayers()
		{
			_Layers = new ObservableCollection<MaterialLayer>();
		}

		public void AddLayer(MaterialLayer Layer)
		{
			if (Layer == null) throw new ArgumentNullException("Переданный в качестве аргумента слой был без ссылки");
			_Layers.Add(Layer);
		}
		public void InsertLayer(MaterialLayer layer, int Index)
		{
			if (layer != null)
			{
				_Layers.Insert(Index, layer);
			}
		}
		public void RemoveLayer(MaterialLayer Layer)
		{
			if (Layer == null)
			{
				throw new Exception("Ссылка на удаляемый слой отсутствует");
			}
			_Layers.Remove(Layer);
		}
		public void ReplaceLayer(int Index, MaterialLayer LayerMaterial)
		{
			if (Index > -1 && Index < _Layers.Count)
			{
				_Layers[Index] = LayerMaterial;
			}
			else
			{
				throw new Exception("Error during replacing the layer. Layers index was out of range.");
			}
		}

		public object Clone()
		{
			ShieldLayers cloneClass = new ShieldLayers();
			for (int i = 0; i < _Layers.Count; i++)
			{
				cloneClass.AddLayer(_Layers[i]);
			}
			return cloneClass;
		}
	}
}
