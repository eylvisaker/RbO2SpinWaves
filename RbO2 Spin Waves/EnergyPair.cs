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
	public struct DoublePair
	{
		public double A { get; set; }
		public double B { get; set; }

		public double this[int index]
		{
			get
			{
				if (index == 0) return A;
				else if (index == 1) return B;
				else
					throw new IndexOutOfRangeException();
			}
			set
			{
				if (index == 0) A = value;
				else if (index == 1) B = value;
				else
					throw new IndexOutOfRangeException();
			}
		}
	}

}
