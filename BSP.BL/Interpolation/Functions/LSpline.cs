using BSP.BL.Extensions;
using System;

namespace BSP.BL.Interpolation.Functions
{
    public class LSpline : IInterpolator
    {
        public double[] Interpolate(double[] x, double[] y, double[] newX, AxisLogScale interpolationScaleType = AxisLogScale.None)
        {
            if (x == null || y == null)
                throw new ArgumentNullException("Input arrays are NULL");

            if (newX == null)
                throw new ArgumentNullException("New X values array is NULL");

            if (x.Length < 2 || y.Length < 2)
                throw new ArgumentException("XYPoints array size is too small! It has to be at least 2 values.");

            int source_length = x.Length;
            int new_data_length = newX.Length;

            if (interpolationScaleType == AxisLogScale.BothXY || interpolationScaleType == AxisLogScale.OnlyX)
            {
                x = x.ToLog10();
                newX = newX.ToLog10();
            }

            if (interpolationScaleType == AxisLogScale.BothXY || interpolationScaleType == AxisLogScale.OnlyY)
            {
                y = y.ToLog10();
            }

            //Новые значения Y
            double[] newY = new double[new_data_length];
            //Массив коэффициентов наклона промежуточных участков (интервалов) массива исходных точек
            double[] slopes = new double[source_length - 1];
            //Массив коэффициентов смещения для промежуточных участков (интервалов) массива исходных точек
            double[] intercepts = new double[source_length - 1];

            for (int i = 0; i < source_length - 1; i++)
            {
                //Рассчитываем предварительно параметры slope и intercept уравнения прямой
                slopes[i] = (y[i + 1] - y[i]) / (x[i + 1] - x[i]);
                intercepts[i] = y[i] - slopes[i] * x[i];
            }

            //Количество итераций равно количеству интервалов
            for (int i = 0; i < source_length - 1; i++)
                for (int j = 0; j < new_data_length; j++)
                {
                    //Если новое значение в пределах текущего интервала массива исходной функции, то рассчитываем новое значение Y по новому значению X
                    if (x[i] < newX[j] && newX[j] <= x[i + 1])
                    {
                        if (AreEqual(x[i], x[i + 1]))
                        {
                            newY[j] = Math.Max(y[i], y[i + 1]);
                        }
                        else
                            newY[j] = slopes[i] * newX[j] + intercepts[i];
                    }

                    //Если новое значение точки левее нижней границы значений X исходной функции, то экстраполируем по последнему интервалу
                    else if (newX[j] < x[0])
                        newY[j] = slopes[0] * newX[j] + intercepts[0];
                    //Если новое значение точки правее верхней границы значений X исходной функции, то экстраполируем по параметрам крайнего интервала
                    else if (newX[j] > x[^1])
                        //TODO: Код может быть медленным, из-за использования Индексов!!!
                        newY[j] = slopes[^1] * newX[j] + intercepts[^1];

                }
            return interpolationScaleType == AxisLogScale.BothXY || interpolationScaleType == AxisLogScale.OnlyY ? newY.ToLinear() : newY;
        }

        private bool AreEqual(double a, double b, double TOL = 1e-5)
        {
            return Math.Abs(a - b) <= TOL;
        }
    }
}
