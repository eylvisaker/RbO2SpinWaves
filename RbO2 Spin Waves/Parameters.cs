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

namespace RbO2_Spin_Waves
{
	public class Parameters
	{
		public double Jxy { get; set; }
		public double Jxx { get; set; }
		public double Jsigma { get; set; }
		public double Jpi { get; set; }

		public double Js { get { return Jsigma + Jpi; } }
		public double Jbar { get { return 0.5 * (Jxy + Jxx); } }
		public double JI { get { return Math.Sqrt(Jsigma * Jpi); } }
	}

}
