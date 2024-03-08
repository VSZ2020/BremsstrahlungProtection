/*
 * Создано в SharpDevelop.
 * Пользователь: Slava Izgagin
 * Дата: 06-Mar-20
 * Время: 19:21
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using System;

namespace BSP.BL.Interpolation.Functions
{
    /// <summary>
    /// Cubic spline class
    /// </summary>
    public class CSpline : IInterpolator
    {
        struct CubicSpline
        {
            public double A;
            public double B;
            public double C;
            public double D;
            public double X;
        }

        public double[] Interpolate(double[] x, double[] y, double[] new_x)
        {
            if (x.Length < 2)
                throw new ArgumentException("No sufficient points count for spline construction");

            int n = x.Length;
            var splines = new CubicSpline[n];

            for (int i = 0; i < n; i++)                 //i = [0,n)
            {
                splines[i] = new CubicSpline();
                splines[i].A = y[i];
                splines[i].X = x[i];
            }

            var h = new double[n - 1];
            var alpha = new double[n - 1];

            for (int i = 0; i < n - 1; i++)             //i = [0,n-2]. Прим. от 0 до 8, при n = 10			
            {
                h[i] = x[i + 1] - x[i];
            }
            for (int i = 1; i < n - 1; i++)             //i = [0,n-2]. Прим. от 0 до 8, при n = 10			
            {
                alpha[i] = 3.0f / h[i] * (splines[i + 1].A - splines[i].A) - 3.0f / h[i - 1] * (splines[i].A - splines[i - 1].A);
            }
            var l = new double[n];
            var u = new double[n];
            var z = new double[n];

            l[0] = 1.0f; u[0] = z[0] = 0;
            l[n - 1] = 1; z[n - 1] = 0; splines[n - 1].C = 0;
            for (int i = 1; i < n - 1; i++)
            {
                l[i] = 2.0f * (x[i + 1] - x[i - 1]) - h[i - 1] * u[i - 1];
                u[i] = h[i] / l[i];
                z[i] = (alpha[i] - h[i - 1] * z[i - 1]) / l[i];
            }

            for (int j = n - 2; j >= 0; j--)
            {
                splines[j].C = z[j] - u[j] * splines[j + 1].C;
                splines[j].B = (splines[j + 1].A - splines[j].A) / h[j] - h[j] * (splines[j + 1].C + 2.0f * splines[j].C) / 3.0f;
                splines[j].D = (splines[j + 1].C - splines[j].C) / (3.0f * h[j]);
            }

            double[] interpolatedValues = new double[new_x.Length];
            for (int i = 0; i < new_x.Length; i++)
            {
                CubicSpline sp = new CubicSpline();

                if (new_x[i] <= splines[0].X) { sp = splines[0]; }
                else
                    if (new_x[i] >= splines[n - 1].X) { sp = splines[n - 2]; }
                else
                {
                    for (int j = 0; j < n - 1; j++)
                    {
                        if (new_x[i] > splines[j].X && new_x[i] <= splines[j + 1].X)
                        {
                            sp = splines[j];
                            break;
                        }
                    }
                }
                interpolatedValues[i] = sp.A + sp.B * (new_x[i] - sp.X) + sp.C * Math.Pow(new_x[i] - sp.X, 2) + sp.D * Math.Pow(new_x[i] - sp.X, 3);
            }
            return interpolatedValues;
        }

    }
}
