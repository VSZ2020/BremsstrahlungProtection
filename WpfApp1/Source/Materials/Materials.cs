using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.WindowsRuntime;

namespace BSP
{
    public class Materials
    {
        private ObservableCollection<Material> materials;

        /// <summary>
        /// Возвращает материал по его названию
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Material this[string name]
        {
            get
            {
                for (int i = 0; i < materials.Count; i++)
				{
                    if (materials[i].Name.Equals(name))
					{
                        return materials[i];
					}
				}
                throw new ArgumentException("Материала " + name + " в списке нет!");
            }
        }
        public Material this[int index]
        {
            get
            {
                return materials[index];
            }
        }

        public ObservableCollection<Material> Collection
		{
            get
			{
                return materials;
			}
		}

        public int Length
        {
            get 
            {
                return (materials != null) ? materials.Count : 0;
            }
        }

        /// <summary>
        /// Конструктор класса Materials
        /// </summary>
        public Materials()
        {
            materials = new ObservableCollection<Material>();
        }

        /// <summary>
        /// Добавление материала в список
        /// </summary>
        /// <param name="NewMaterial"></param>
        public void AddMaterial(Material NewMaterial)
        {
            if (NewMaterial == null) throw new NullReferenceException("Пустая ссылка на новый создаваемый материал");
            if (materials.Contains(NewMaterial)) throw new ArgumentException("Нуклид в именем " + NewMaterial.Name + " уже существует!");
            materials.Add(NewMaterial);
        }

        /// <summary>
        /// Удаление материала по его имени
        /// </summary>
        /// <param name="Name"></param>
        public void RemoveMaterial(Material material)
        {
            if (!materials.Contains(material)) throw new ArgumentException("Материала с именем " + material.Name + " не существует в списке");

            materials.Remove(material);
        }

        /// <summary>
        /// Возвращает список названий материалов в виде массива
        /// </summary>
        /// <returns>Массив названий</returns>
        public string[] GetMaterialNamesList()
        {
            if (materials == null) return new string[] { "" };

            string[] namesList = new string[materials.Count + 1];

            namesList[0] = "";                                      //Пустая строка для случая, когда ничего не выбрано
            int i = 1;                                              //Счетчик списка
            foreach (Material mat in materials)
            {
                namesList[i++] = mat.Name;
            }
            return namesList;
        }

        public int IndexOf(string Name)
		{
            for (int i = 0; i < materials.Count; i++)
            {
                if (materials[i].Name.Equals(Name))
                {
                    return i;
                }
            }
            return 0;
        }
    }
}
