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
using System.IO;
using System.Linq;
using System.Text;
using ERY.EMath;

namespace RbO2_Spin_Waves
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().Run(args);
        }

        private void Run(string[] args)
        {
            List<Type> models = new List<Type>();
            models.Add(typeof(OrbitalXY));
            models.Add(typeof(PtypeABAB));
            models.Add(typeof(PtypeABCD));
            models.Add(typeof(OrbitalXX));

            bool done = false;
            while (done == false)
            {
                Console.Write("Enter filename: ");
                string filename = Console.ReadLine();

                if (string.IsNullOrEmpty(filename))
                    break;

                Console.Write("Enter Jxy Jxx Jsigma Jpi (nothing for defaults): ");
                string line = Console.ReadLine();

                Console.WriteLine();

                Parameters param;
                if (line != "")
                {
                    string[] entries = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    double[] p = new double[4];
                    for (int i = 0; i < 4; i++)
                        p[i] = double.Parse(entries[i]);

                    param = new Parameters { Jxy = p[0], Jxx = p[1], Jsigma = p[2], Jpi = p[3] };
                }
                else
                {
                    param = new Parameters { Jxy = 4.8, Jxx = 1.54, Jsigma = 3.89, Jpi = 0.34 };
                }


                for (int i = 0; i < models.Count; i++)
                {
                    Console.WriteLine("\t{0}. {1}", i + 1, models[i].Name);
                }

                chooseModel:
                Console.Write("Choose model: ");

                int sel;

                try
                {
                    sel = int.Parse(Console.ReadLine());
                    sel--;
                }
                catch
                {
                    goto chooseModel;
                }

                Model m = (Model)Activator.CreateInstance(models[sel]);
                m.Filename = filename;
                m.Run(param);

                Console.WriteLine("E0:   {0}", m.E0(param));
                Console.WriteLine("E1-s: {0}", m.E1S);
                Console.WriteLine("E1-o: {0}", m.E1O);

                using (var file = new StreamWriter(filename))
                {
                    WriteGraceHeader(file, param, m, "");
                    WriteGraceSetLineColor(file, 1, 1, 1, 2, 2);

                    WriteGraceDottedFirstSet(file);

                    WriteGraceLegend(file, 1, "Spin Waves");
                    WriteGraceLegend(file, 3, "Orbital Waves");

                    WriteGraceBaseline(file, m.SpinWave.Length);
                    WriteGraceDataset(file, m.SpinWave.Length, x => m.SpinWave[x].A);
                    WriteGraceDataset(file, m.SpinWave.Length, x => m.SpinWave[x].B);
                    WriteGraceDataset(file, m.OrbitalWave.Length, x => m.OrbitalWave[x].A);
                    WriteGraceDataset(file, m.OrbitalWave.Length, x => m.OrbitalWave[x].B);
                }

                Console.WriteLine("Done.  Data written to {0}. (AGR/XMGrace format", filename);

                using (var file = new StreamWriter(filename + "-bc"))
                {
                    WriteGraceHeader(file, param, m, "Diagonalization Parameters");
                    WriteGraceSetLineColor(file, 1, 1, 2);

                    WriteGraceDottedFirstSet(file);

                    WriteGraceLegend(file, 1, @"b\S2");
                    WriteGraceLegend(file, 2, "c");

                    WriteGraceBaseline(file, m.OrbParam.Length);
                    WriteGraceDataset(file, m.OrbParam.Length, x => m.OrbParam[x].A);
                    WriteGraceDataset(file, m.OrbParam.Length, x => m.OrbParam[x].B);
                }

                Console.WriteLine();
            }
        }

        private static void WriteGraceDottedFirstSet(StreamWriter file)
        {
            file.WriteLine("@    s0 line linestyle 2");
        }

        private void WriteGraceLegend(StreamWriter file, int dataset, string text)
        {
            file.WriteLine("@    s{0} legend \"{1}\"", dataset, text);
        }
        private static void WriteGraceSetLineColor(StreamWriter file, int start, params int[] linestyle)
        {
            for (int i = 0; i < linestyle.Length; i++)
            {
                file.WriteLine("@    s{0} line linewidth 2.0", i + start);
                file.WriteLine("@    s{0} line color {1}", i + start, linestyle[i]);
            }
        }

        private static void WriteGraceHeader(StreamWriter file, Parameters param, Model m, string title)
        {
            file.WriteLine("# Model {0}", m.GetType().Name);
            file.WriteLine("#");
            file.WriteLine("# Jxy = {0}", param.Jxy);
            file.WriteLine("# Jxx = {0}", param.Jxx);
            file.WriteLine("# Jsigma = {0}", param.Jsigma);
            file.WriteLine("# Jpi = {0}", param.Jpi);
            file.WriteLine("#");
            file.WriteLine("# K-path: ");

            for (int i = 0; i < m.KPath.Locations.Length; i++)
            {
                file.WriteLine("#    {0}    {1}", m.KPath.Names[i], m.KPath.Locations[i]);
            }


            file.WriteLine("@with g0");
            file.WriteLine("@    world 0, 0, {0}, {1}", m.KPath.Path.Length - 1, m.GraphMax);

            if (string.IsNullOrEmpty(title) == false)
            {
                file.WriteLine("@    title \"{0}\"", title);
                file.WriteLine("@    subtitle \"{0}\"", m.GetType().Name);
            }

            file.WriteLine("@    xaxis  tick spec type both");
            file.WriteLine("@    xaxis  tick spec {0}", m.KPath.Locations.Length);
            for (int i = 0; i < m.KPath.Locations.Length; i++)
            {
                file.WriteLine("@    xaxis  tick major {0}, {1}", i, m.KPath.Locations[i]);
                file.WriteLine("@    xaxis  ticklabel {0}, \"{1}\"", i, m.KPath.Names[i]);
            }

        }

        private void WriteGraceBaseline(StreamWriter file, int length)
        {
            file.WriteLine("@type xy");
            file.WriteLine("0 0");
            file.WriteLine("{0} 0", length);
            file.WriteLine("&");
        }

        private static void WriteGraceDataset(StreamWriter file, int length, Func<int, double> data)
        {
            file.WriteLine("@type xy");
            for (int i = 0; i < length; i++)
            {
                file.WriteLine("{0}   {1}", i, data(i));
            }
            file.WriteLine("&");
        }


    }
}
