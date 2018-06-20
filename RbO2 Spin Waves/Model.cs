//  RbO2 Spin Wave Calculator
//  Copyright (C) 2007-2017  Erik Ylvisaker
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//    
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <https://www.gnu.org/licenses/>.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERY.EMath;

namespace RbO2_Spin_Waves
{
	public abstract class Model
	{
		double e1s, e1o;

		public Model()
		{
			SpinWave = new DoublePair[1];
			OrbitalWave = new DoublePair[1];
			KPath = KPath.CreateTetragonal();
		}

		public double E1S { get { return e1s; } }
		public double E1O { get { return e1o; } }

		public string Filename { get; set; }

		public void Run(Parameters p)
		{
			SpinWave = new DoublePair[KPath.Path.Length];
			OrbitalWave = new DoublePair[KPath.Path.Length];
			OrbParam = new DoublePair[KPath.Path.Length];
			var grid = KPath.CreateGrid(80);

			//using (var w = new System.IO.StreamWriter(Filename + ".k"))
			//{
			//    //w.WriteLine("index\tcontrib\ttotal\tkx\tky\tkz");

				for (int i = 0; i < grid.Length; i++)
				{
					Vector3 k = grid[i];
					double a = Math.Abs(Spin_A(p, k));
					double b = Spin_B(p, k);
					double orb_a = Math.Abs(Orb_A(p, k));
					double orb_c = Orb_C(p, k);

					double scontrib = a - Math.Sqrt(a * a - b * b);
					e1s -= scontrib;

					e1o -= orb_a - Math.Sqrt(orb_a * orb_a - orb_c * orb_c);

					//w.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", i, scontrib, e1s, k.X, k.Y, k.Z);
			    }
			//}

			e1s = e1s / grid.Length;
			e1o = e1o / grid.Length;

			for (int i = 0; i < KPath.Path.Length; i++)
			{
				Vector3 k = KPath.Path[i];

				double a = Spin_A(p, k);
				double b = Spin_B(p, k);
				double a_d = Spin_Ad(p, k);

				double omega;
				if (a >= b)
					omega = Math.Sqrt(a*a - b*b);
				else
					omega = - Math.Sqrt(b*b - a*a);

				SpinWave[i].A = omega - a_d;
				SpinWave[i].B = omega + a_d;

				double A = Orb_A(p, k);
				double B = Orb_B(p, k);
				double C = Orb_C(p, k);
				double D = Orb_D(p, k);
				double E = Orb_E(p, k);

				double c = (B * (A - E) - Math.Pow(C - D, 2)) * (B * (A + E) - Math.Pow(C + D, 2));
				b = 0.5 * (A * A + B * B - 2 * C * C + 2 * D * D - E * E);
				double ratio = c / (b * b);

				OrbParam[i].A = Math.Sign(b) * b*b;
				OrbParam[i].B = c;

				if (b >= 0)
				{
					if (0 <= ratio && ratio <= 1)
					{
						// yay real frequencies
						OrbitalWave[i].A = Math.Sqrt(b * (1 - Math.Sqrt(1 - ratio)));
						OrbitalWave[i].B = Math.Sqrt(b * (1 + Math.Sqrt(1 - ratio)));
					}
					else if (ratio > 1)
					{
						OrbitalWave[i].A = -Math.Pow(c, 0.25);
						OrbitalWave[i].B = -Math.Pow(c, 0.25);
					}
					else if (ratio < 0)
					{
						double g = Math.Sqrt(1 - ratio);

						OrbitalWave[i].A = -Math.Sqrt(b * (g - 1));
						OrbitalWave[i].B = Math.Sqrt(b * (1 + g));
					}
				}
				else
				{
					if (0 <= ratio && ratio <= 1)
					{
						double g = Math.Sqrt(b * b - c);

						if (g >= -b)
						{
							OrbitalWave[i].A = -Math.Sqrt(g + b);
							OrbitalWave[i].B = Math.Sqrt(g - b);
						}
						else
						{
							OrbitalWave[i].A = -Math.Sqrt(-(b + g));
							OrbitalWave[i].B = -Math.Sqrt(g - b);
						}
					}
					else if (ratio < 0)
					{
						double t = Math.Sqrt(1 - ratio);

						OrbitalWave[i].A = -Math.Sqrt(-b) * Math.Sqrt(t + 1);
						OrbitalWave[i].B = Math.Sqrt(-b) * Math.Sqrt(t - 1);
					}
					else if (ratio > 1)
					{
						OrbitalWave[i].A = -Math.Sqrt(-b) * Math.Pow(ratio, 0.25);
						OrbitalWave[i].B = -Math.Sqrt(-b) * Math.Pow(ratio, 0.25);
					}
				}

				if (double.IsNaN(OrbitalWave[i].A) ||
					double.IsNaN(OrbitalWave[i].B))
				{
					throw new Exception();
				}
			}
		}

		protected abstract double Spin_A(Parameters p, Vector3 k);
		protected abstract double Spin_B(Parameters p, Vector3 k);
		protected abstract double Spin_Ad(Parameters p, Vector3 k);

		protected abstract double Orb_A(Parameters p, Vector3 k);
		protected abstract double Orb_B(Parameters p, Vector3 k);
		protected abstract double Orb_C(Parameters p, Vector3 k);
		protected abstract double Orb_D(Parameters p, Vector3 k);
		protected abstract double Orb_E(Parameters p, Vector3 k);

		public abstract double E0(Parameters p);
		
		public KPath KPath { get; protected set; }

		public DoublePair[] SpinWave { get; protected set; }
		public DoublePair[] OrbitalWave { get; protected set; }
		public DoublePair[] OrbParam { get; protected set; }

		public double GraphMax
		{
			get
			{
				double maxSpin = Math.Max(
					SpinWave.Max(x => x.A), SpinWave.Max(x => x.B));
				double maxOrb = Math.Max(
					OrbitalWave.Max(x => x.A), OrbitalWave.Max(x => x.B));

				double maxEnergy = Math.Max(maxSpin, maxOrb);

				return Math.Ceiling(maxEnergy / 10) * 10;
			}
		}

	}

}
