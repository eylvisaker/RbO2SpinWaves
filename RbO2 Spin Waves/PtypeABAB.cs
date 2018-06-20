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
	class PtypeABAB : Model
	{
		private double G_m(Vector3 k)
		{
			return Math.Cos(0.5 * (k.X + k.Y)) * Math.Cos(0.5 * k.Z);
		}
		private double G_n(Vector3 k)
		{
			return Math.Cos(0.5 * (k.X - k.Y)) * Math.Cos(0.5 * k.Z);
		}

		double Gam_2(Vector3 k)
		{
			return 0.5 * (Math.Cos(k.X) + Math.Cos(k.Y));
		}

		protected override double Spin_A(Parameters p, ERY.EMath.Vector3 k)
		{
			return 8 * p.Jbar;
		}

		protected override double Spin_B(Parameters p, ERY.EMath.Vector3 k)
		{
			return 4 * (p.Jxy * G_m(k) + p.Jxx * G_n(k));
		}

		protected override double Spin_Ad(Parameters p, ERY.EMath.Vector3 k)
		{
			return 0;
		}


		protected override double Orb_A(Parameters p, Vector3 k)
		{
			return p.Js;
		}

		protected override double Orb_B(Parameters p, Vector3 k)
		{
			return p.Js;
		}

		protected override double Orb_C(Parameters p, Vector3 k)
		{
			return 0.5 * p.JI * Gam_2(k);
		}

		protected override double Orb_D(Parameters p, Vector3 k)
		{
			return 0;
		}

		protected override double Orb_E(Parameters p, Vector3 k)
		{
			return 0;
		}


		public override double E0(Parameters p)
		{
			return -4 * p.Jbar - 0.5 * p.Js;
		}
	}
}
