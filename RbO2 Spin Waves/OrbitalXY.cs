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
	class OrbitalXY : Model
	{
		double Gamma(Vector3 k)
		{
			return Math.Cos(k[0] / 2) * Math.Cos(k[1] / 2) * Math.Cos(k[2] / 2);
		}
		double Gam_X(Vector3 k)
		{
			return Math.Cos(k[0]);
		}
		double Gam_Y(Vector3 k)
		{
			return Math.Cos(k[1]);
		}
		double Gam_2(Vector3 k)
		{
			return 0.5 * (Math.Cos(k.X) + Math.Cos(k.Y));
		}

		protected override double Spin_A(Parameters p, Vector3 k)
		{
			return 8 * p.Jxy + 0.5 * p.Js * (Gam_X(k) + Gam_Y(k) - 2);
		}
		protected override double Spin_B(Parameters p, Vector3 k)
		{
			return 8 * p.Jxy * Gamma(k);
		}
		protected override double Spin_Ad(Parameters p, Vector3 k)
		{
			return 0.5 * (p.Jsigma - p.Jpi) * (Gam_Y(k) - Gam_X(k));
		}

		protected override double Orb_A(Parameters p, Vector3 k)
		{
			return -p.Js;
		}
		protected override double Orb_B(Parameters p, Vector3 k)
		{
			return Orb_A(p, k);
		}
		protected override double Orb_C(Parameters p, Vector3 k)
		{
			return 0;
		}
		protected override double Orb_D(Parameters p, Vector3 k)
		{
			return 0.5 * p.JI * Gam_2(k);
		}
		protected override double Orb_E(Parameters p, Vector3 k)
		{
			return 0;
		}

		public override double E0(Parameters p)
		{
			return -4 * p.Jbar;
		}
	}
}
