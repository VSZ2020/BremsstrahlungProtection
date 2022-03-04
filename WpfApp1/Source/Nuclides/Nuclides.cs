using System;
using System.Collections.ObjectModel;

namespace BSP
{
	public class Nuclides
    {
        private ObservableCollection<Nuclide> nuclides;
        public delegate void NuclideAddEvent();

        /// <summary>
        /// Event evokes after nuclide was added
        /// </summary>
        public event NuclideAddEvent OnNuclideAdded;

        public Nuclide this[string Name]
        {
            get
            {
                foreach (Nuclide nucl in nuclides)
				{
                    if (nucl.Name.Equals(Name))
					{
                        return nucl;
					}
				}
                
                throw new ArgumentException("There is no nuclide with name " + Name);
            }
        }

        public ObservableCollection<Nuclide> Collection
		{
			get
			{
                return nuclides;
			}
		}

        public string[] Names
		{
			get
			{
                int count = nuclides.Count;
                string[] names = new string[count];
                for (int i = 0; i < count; i++)
				{
                    names[i] = nuclides[i].Name;
				}
                return names;
			}
		}
        
        public int Length
        { get { return nuclides?.Count ?? 0; } }

        public Nuclides()
        {
            nuclides = new ObservableCollection<Nuclide>();
        }

        public void AddNuclide(Nuclide NewNuclide)
        {
            if (NewNuclide == null) throw new Exception("The reference for adding nuclide is null");
            nuclides.Add(NewNuclide);

            OnNuclideAdded?.Invoke();
        }

        public void RemoveNuclide(string Name)
        {
            if (this[Name] != null)
                nuclides.Remove(this[Name]);
            /*
            Nuclide[] copy = (Nuclide[])nuclides.Clone();

            int copyCounter = 0;
            for (int i = 0; i < nuclides.Length; i++)
            {
                if (i == Index) copyCounter++;
                if (copyCounter == copy.Length) break;

                nuclides[i] = copy[copyCounter];
                copyCounter++;
            }

            Array.Resize(ref nuclides, nuclides.Length - 1);
            */
        }
        public void RemoveNuclide(Nuclide nuclide)
        {
            if (nuclide == null) throw new Exception("The removing nuclide reference is null.");
            nuclides.Remove(nuclide);
        }

        public void ReplaceNuclide(int Index, Nuclide NewNuclide)
		{
            if (Index > -1 && Index < nuclides.Count)
            {
                nuclides[Index] = NewNuclide;
            }
            else
            {
                throw new Exception("Error during replacing the nuclide. Nuclide index was out of range.");
            }
        }

        /// <summary>
        /// Получить список нуклидов по их названиям
        /// </summary>
        /// <returns>Массив названий нуклидов</returns>
        public string[] GetNuclideNamesList()
        {
            if (nuclides == null) return new string[] { "" };

            string[] namesList = new string[nuclides.Count + 1];

            namesList[0] = "";                                      //Пустая строка для случая, когда ничего не выбрано
            int i = 1;                                              //Счетчик списка
            foreach (Nuclide nucl in nuclides)
            {
                namesList[i++] = nucl.Name;
            }
            return namesList;
        }

        public int IndexOf(string Name)
		{
            for (int i = 0; i < nuclides.Count; i++)
            {
                if (nuclides[i].Name.Equals(Name))
                {
                    return i;
                }
            }
            return 0;
        }
    }
}
