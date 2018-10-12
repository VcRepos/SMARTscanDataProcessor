using System;
using System.Collections.Generic;
using System.IO;
using OSGeo.OGR;
using OSGeo.OSR;

namespace SMARTscan_DataProcessor
{
    public static class VboxFileController
    {
        public static double[] TransformWGS83ToOSGB(double latitude, double longitude)
        {
            SpatialReference src = new SpatialReference("");
            src.ImportFromEPSG(4326);
            SpatialReference dst = new SpatialReference("");
            dst.ImportFromEPSG(27700);
            CoordinateTransformation ct = new CoordinateTransformation(src, dst);
            double[] point = new double[3];
            point[0] = longitude;
            point[1] = latitude;
            point[2] = 0;
            ct.TransformPoint(point);
            return point;
        }

        public static void ProcessVboxData(string directPath)
        {
            //Define the required module and file path
            Ogr.RegisterAll();
            Dictionary<string, string[]> IntegratedData = new Dictionary<string, string[]>();
            Dictionary<string, string[]> ConvertedDict = new Dictionary<string, string[]>();
            List<string[]> ConvertedData = new List<string[]>();

            //Get contents from the folder
            DirectoryInfo directory = new DirectoryInfo(directPath);
            FileInfo[] vboFiles = directory.GetFiles("*vbo");

            if (vboFiles.Length == 0)
            {
                throw new Exception("Note that there is no vbox file in the folder");
            }

            // Read each file to get all data from the file
            foreach (FileInfo file in vboFiles)
            {
                FileStream readfile = File.Open(file.FullName, FileMode.Open);
                string fileName = Path.GetFileNameWithoutExtension(file.Name);
                using (StreamReader reader = new StreamReader(readfile))
                {
                    while (reader.Peek() > -1)
                    {
                        string line = reader.ReadLine();
                        if (line.Contains("[data]"))
                        {
                            string[] textFile = reader.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.None);
                            IntegratedData.Add(fileName, textFile);
                        }
                    }
                }
            }

            if (IntegratedData.Count > 0)
            {
                int couter = 1; // this would be populated as ID
                foreach (var block in IntegratedData)
                {
                    string name = block.Key;
                    string[] values = block.Value;

                    foreach (var str in values)
                    {
                        if (str != "")
                        {
                            // Splite string by space and get latitude value
                            double latitude = Convert.ToDouble(str.Split(' ')[2]);
                            double longtude = Convert.ToDouble(str.Split(' ')[3]);

                            // See RacelLogic user mannual - VBOX Latitude and Longitude Calculations
                            double degreeLat = Math.Round((latitude / 60), 8);
                            double degreeLon = Math.Round((longtude / 60), 8);

                            // Check the prefix
                            if (degreeLon > 0)
                            {
                                degreeLon = degreeLon * (-1);
                            }
                            else
                            {
                                degreeLon = Math.Abs(degreeLon);
                            }

                            string id = couter.ToString();
                            couter++;

                            // Implement coordinate transformation
                            double X = TransformWGS83ToOSGB(degreeLat, degreeLon)[0];
                            double Y = TransformWGS83ToOSGB(degreeLat, degreeLon)[1];

                            string[] strLocation = new string[] { id, name, X.ToString(), Y.ToString() };

                            ConvertedData.Add(strLocation);
                        }
                    }
                }
            }

            //Write Vbox data to file
            string csvName = directPath + "\\" + "Vbox.csv";
            if (File.Exists(csvName))
            {
                File.Delete(csvName);
            }

            using (StreamWriter writer = new StreamWriter(csvName))
            {
                string[] header = new string[4] { "ID", "Scan_num", "Latitude", "Longitude", };
                writer.WriteLine($"{header[0]},{header[1]},{header[2]},{header[3]}");
                ConvertedData.ForEach(item => writer.WriteLine($"{item[0]},{item[1]},{item[2]},{item[3]}"));
            }
        }
    }
}
