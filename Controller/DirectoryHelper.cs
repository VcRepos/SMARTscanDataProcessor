using System;
using System.Drawing;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ImageProcessor;
using ImageProcessor.Imaging.Formats;


namespace SMARTscan_DataProcessor.Controller
{
    class DirectoryHelper
    {
        public static DirectoryInfo[] GetDirectories(string path)
        {
            DirectoryInfo ParentDirectory = new DirectoryInfo(path);
            DirectoryInfo[] Dicts = ParentDirectory.GetDirectories();
            return Dicts;
        }

        public static void CopyFiles(string sourceFolder, string targetFolder)
        {
            foreach (var file in Directory.GetFiles(sourceFolder))
            {
                string fName = file.Substring(sourceFolder.Length + 1);
                File.Copy(Path.Combine(sourceFolder, fName), Path.Combine(targetFolder, fName));
            }
        }

        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        public static void CopyALLDirt(string sourceFolder, string targetFolder)
        {
            foreach (var direct in Directory.GetFiles(sourceFolder))
            {

            }
        }

        public static bool IsInDirectoryList(string dict, DirectoryInfo[] dictList)
        {
            bool inDirectory = false;

            foreach (DirectoryInfo directFile in dictList)
            {
                if (dict == directFile.Name)
                {
                    inDirectory = true;
                    break;
                }
            }

            return inDirectory;
        }

        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }

        /// <summary>
        /// Zip the file from a specific zip file
        /// </summary>
        /// <param name="zipfile"></param>
        /// <returns></returns>
        public static string UnzipFile(FileInfo zipfile)
        {
            string zipfileStoreFolder = Path.Combine(zipfile.DirectoryName, Path.GetFileNameWithoutExtension(zipfile.FullName));
            //string zipfileStoreFolder = zipfile.DirectoryName + "\\" + Path.GetFileNameWithoutExtension(zipfile.FullName);
            bool isFolderExist = Directory.Exists(zipfileStoreFolder);

            // Check if the project is already existed
            if (isFolderExist)
            {
                DeleteDirectory(zipfileStoreFolder);
            }

            try
            {
                FastZip fasterzip = new FastZip();
                fasterzip.ExtractZip(zipfile.FullName, zipfile.DirectoryName, null);
                return zipfileStoreFolder;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Resize all image to specific size for requirements
        /// </summary>
        /// <param name="directPath"></param>
        public static void ResizeGroupImages(string directPath, int width, int height)
        {
            DirectoryInfo directory = new DirectoryInfo(directPath);
            foreach (FileInfo file in directory.GetFiles("*jpg"))
            {
                byte[] source = File.ReadAllBytes(file.FullName);
                using (MemoryStream inStream = new MemoryStream(source))
                using (MemoryStream outStream = new MemoryStream())
                using (ImageFactory imageFactory = new ImageFactory())
                {
                    imageFactory.Load(inStream)
                                .Resize(new Size(width,height))
                                .Format(new JpegFormat())
                                .Quality(70)
                                .Save(outStream);

                    byte[] resizedImage = outStream.ToArray();
                    string outpath = Path.Combine(directory + "\\" + file.Name);
                    //string outpath = Path.Combine(outputPath +"\\"+file.Name);
                    using (FileStream fileStream = new FileStream(outpath, FileMode.Open))
                    {
                        outStream.CopyTo(fileStream);
                    }
                }
            }
        }


    }
}
