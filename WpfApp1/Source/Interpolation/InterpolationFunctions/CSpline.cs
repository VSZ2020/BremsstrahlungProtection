/*
 * Создано в SharpDevelop.
 * Пользователь: Slava Izgagin
 * Дата: 06-Mar-20
 * Время: 19:21
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using System;

namespace BSP
{
	/// <summary>
	/// Cubic spline class
	/// </summary>
	public class CSpline: ASpline
	{
		struct CubicSpline
		{
			public double A;
			public double B;
			public double C;
			public double D;
			public double X;
		}
		
		CubicSpline[] splines;
		int n = 0;
		
		public CSpline(double[] x, double[] y)
		{
			if (x.Length != y.Length) throw new Exception("Размеры массивов X и Y различны");
			n = x.Length;
			splines = new CubicSpline[n];
			
			for (int i = 0; i < n; i++)					//i = [0,n)
			{
				splines[i] = new CubicSpline();
				splines[i].A = y[i];
				splines[i].X = x[i];
			}
			
			double[] h = new double[n - 1];
			double[] alpha = new double[n - 1];
			
			for (int i = 0; i < n - 1; i++)				//i = [0,n-2]. Прим. от 0 до 8, при n = 10			
			{
				h[i] = x[i+1] - x[i];

			}
			for (int i = 1; i < n - 1; i++)				//i = [0,n-2]. Прим. от 0 до 8, при n = 10			
			{
				alpha[i] = 3.0/h[i]*(splines[i+1].A - splines[i].A) - 3.0/h[i-1]*(splines[i].A - splines[i-1].A);
			}
			double[] l = new double[n];
			double[] u = new double[n];
			double[] z = new double[n];
			
			l[0] = 1.0;	u[0] = z[0] = 0.0;
			l[n - 1] = 1.0;	z[n - 1] = 0.0; splines[n - 1].C = 0.0;
			for (int i = 1; i < n - 1; i++)
			{
				l[i] = 2.0*(x[i+1] - x[i-1]) - h[i-1]*u[i-1];
				u[i] = h[i]/l[i];
				z[i] = (alpha[i] - h[i-1]*z[i-1])/l[i];
			}
			
			for (int j = n - 2; j >= 0; j--)
			{
				splines[j].C = z[j] - u[j]*splines[j+1].C;
				splines[j].B = (splines[j+1].A - splines[j].A)/h[j] - h[j]*(splines[j+1].C + 2.0*splines[j].C)/3.0;
				splines[j].D = (splines[j+1].C - splines[j].C)/(3.0*h[j]);
			}
		}
	
		/*
		public double[] A {get;set;}
		public double[] B {get;set;}
		public double[] C {get;set;}
		public double[] D {get;set;}
		
		private double[] xs {get;set;}
		private double[] ys {get;set;}
		
		private double[] h {get;set;}
		
		public int Length 
		{
			get 
			{
				if (ys.Length > 1) return ys.Length - 1;
				else return 0;
			}
		}
		
		public double this[int index]
		{
			get 
			{
				if (index < 0 || index > B.Length) 
				{
					throw new IndexOutOfRangeException("Spline index out of range!");
				}
				return A[index] + B[index]*xs[index] + C[index]*Math.Pow(xs[index],2) + D[index]*Math.Pow(xs[index],3);}
		}
		
		public CSpline(double[] x, double[] y)
		{
			if (x.Length != y.Length) throw new Exception("The \"x\" dimension isn't equal \"y\" dimension!");
			this.xs = x;
			this.ys = y;
			
			int n = y.Length - 1;
			
			A = new double[n + 1];
			B = new double[n];
			C = new double[n + 1];
			D = new double[n];
			
			for (int i = 0; i <= n; i++)					//i = [0,n-1]
				A[i] = y[i];
			
			h = new double[n];
			double[] alpha = new double[n];
			
			for (int i = 0; i <= n - 1; i++)				//i = [0,n-2]. Прим. от 0 до 8, при n = 10			
			{
				h[i] = x[i+1] - x[i];
				
				if (i >= 1) alpha[i] = 3.0/h[i]*(A[i+1] - A[i]) - 3.0/h[i-1]*(A[i] - A[i-1]);
			}
			
			double[] l = new double[n + 1];
			double[] u = new double[n + 1];
			double[] z = new double[n + 1];
			
			l[0] = 1.0;	u[0] = z[0] = 0.0;
			l[n] = 1.0;	z[n] = 0.0; C[n] = 0.0;
			
			for (int i = 1; i <= n - 1; i++)
			{
				l[i] = 2.0*(x[i+1] - x[i-1]) - h[i-1]*u[i-1];
				u[i] = h[i]/l[i];
				z[i] = (alpha[i] - h[i-1]*z[i-1])/l[i];
			}
			
			for (int j = n - 1; j >= 0; j--)
			{
				C[j] = z[j] - u[j]*C[j+1];
				B[j] = (A[j+1] - A[j])/h[j] - h[j]*(C[j+1] + 2.0*C[j])/3.0;
				D[j] = (C[j+1] - C[j])/(3.0*h[j]);
			}
		}
		*/
		public override double GetValue(double x, float ENERGY_EDGE)
		{
			if (n < 2) throw new Exception("Слишком мало точек данных для построения сплайна. Нужно как минимум 2 точки");
			CubicSpline sp = new CubicSpline();
			

			if (x <= splines[0].X) {sp = splines[0];}
			else
				if (x >= splines[n-1].X) {sp = splines[n - 2];}
			else
			{
				for (int i = 0; i < n - 1; i++)
				{
					if (x > splines[i].X && x <= splines[i+1].X) {sp = splines[i]; break;}
				}
			}
			
			return sp.A + sp.B*(x - sp.X) + sp.C*Math.Pow(x-sp.X,2) + sp.D*Math.Pow(x-sp.X,3);
		}

		public override double[] GetArray(double[] x, ref Material material)
		{
			double[] y = new double[x.Length];
			for (int i = 0; i < x.Length; i++)
				y[i] = GetValue(x[i], material.EnergyLimit);
			return y;
		}
		
		
	}
}
