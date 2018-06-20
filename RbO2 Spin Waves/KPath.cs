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
	public class KPath
	{
		private KPath()
		{ }
		public static KPath CreateTetragonal()
		{
			KPath retval = new KPath();
			return retval;
		}
		public static KPath CreateOrthonormal()
		{
			KPath retval = new KPath();

			retval.Names = new string[]
			{
				"\\xG",
				"X",
				"S",
				"Y",
				"\\xG",
				"Z",
				"U",
				"R",
				"T",
				"\\xG",
			};
			retval.Points = new Vector3[] 
			{
				new Vector3(0,0,0),
				new Vector3(1,0,0),
				new Vector3(1,1,0),
				new Vector3(0,1,0),
				new Vector3(0,0,0),
				new Vector3(0,0,1),
				new Vector3(1,0,1),
				new Vector3(1,1,1),
				new Vector3(0,1,1),
				new Vector3(0,0,0),
			};

			return retval;
		}
		public string[] Names = new string[]
		{
			"\\xG",
			"X",
			"M",
			"\\xG",
			"Z",
			"R",
			"A",
			"\\xG",
		};
		Vector3[] Points = new Vector3[] 
		{
			new Vector3(0,0,0),
			new Vector3(1,0,0),
			new Vector3(1,1,0),
			new Vector3(0,0,0),
			new Vector3(0,0,1),
			new Vector3(1,0,1),
			new Vector3(1,1,1),
			new Vector3(0,0,0),
		};
		const double cOverA = 1.7167259;
		double ZRatio { get { return 1 / cOverA; } }

		Vector3[] mPath;
		int[] mLocations;

		public int[] Locations
		{
			get
			{
				if (mLocations != null)
					return mLocations;

				const int points = 100;
				mLocations = new int[Points.Length];

				for (int i = 1; i < mLocations.Length; i++)
				{
					Vector3 delta = Points[i] - Points[i - 1];
					double dist = Math.Sqrt(
						delta.X * delta.X + delta.Y * delta.Y + Math.Pow(ZRatio * delta.Z, 2));

					int len = (int)(dist * points);

					mLocations[i] = mLocations[i - 1] + len;
				}

				return mLocations;
			}
		}
		public Vector3[] Path
		{
			get
			{
				if (mPath != null)
					return mPath;

				List<Vector3> path = new List<Vector3>();
				path.Capacity = Locations[Locations.Length - 1] + 1;

				for (int i = 1; i < Points.Length; i++)
				{
					Vector3 start = Points[i - 1];
					int len = Locations[i] - Locations[i - 1];

					Vector3 step = Points[i] - Points[i - 1];
					step /= len;

					for (int j = 0; j < len; j++)
					{
						Vector3 value = start + j * step;
						value *= Math.PI;
						path.Add(value);
					}
				}
				path.Add(Points[Points.Length - 1] * Math.PI);

				mPath = path.ToArray();
				return mPath;
			}
		}
		public Vector3[] CreateGrid(int n)
		{
			List<Vector3> retval = new List<Vector3>();
			Vector3 g1 = new Vector3(0, 1, 1);
			Vector3 g2 = new Vector3(1, 0, 1);
			Vector3 g3 = new Vector3(1, 1, 0);

			g1 *= 2 * Math.PI; g2 *= 2 * Math.PI; g3 *= 2 * Math.PI;

			for (int k = 0; k < n; k++)
			{
				double dz = k / (double)n;

				for (int j = 0; j < n; j++)
				{
					double dy = j / (double)n;

					for (int i = 0; i < n; i++)
					{
						double dx = i / (double)n;

						Vector3 value = dx * g1 + dy * g2 + dz * g3;

						retval.Add(value);
					}
				}
			}

			return retval.ToArray();
		}
	}
}
