using System.IO;
using SMARTscan_DataProcessor.Data;
using SMARTscan_DataProcessor.Controller;
using System;

namespace SMARTscan_DataProcessor
{
    public class SmscanBuilder
    {
        #region Public Properties

        public string SmscanSchemePath { get; set; }
        public string SmscanSourceFolder { get; set; }
        public string SmscanBackUpFolder { get; set; }
        public string SmscanGeodatabase { get; set; }
        public FileInfo ZipFileSource { get; set; }
        public SmscanScheme WorkingScheme { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor with zipfile and default folder settings
        /// </summary>
        /// <param name="zipFileSource"></param>
        public SmscanBuilder(string zipFileSource)
        {
            ZipFileSource = new FileInfo(zipFileSource);
            SmscanSourceFolder = Global.SMsourceFolder;
            SmscanBackUpFolder = Global.SMbackupFolder;
            SmscanGeodatabase = Global.SMGeodatabase;
        }

        /// <summary>
        /// Constructors with zipfile and self-defined settings
        /// </summary>
        /// <param name="zipFileSource"></param>
        /// <param name="sourceFolder"></param>
        /// <param name="backupFolder"></param>
        /// <param name="geodatabase"></param>
        public SmscanBuilder(string zipFileSource, string sourceFolder, string backupFolder, string geodatabase)
        {
            ZipFileSource = new FileInfo(zipFileSource);
            SmscanSourceFolder = sourceFolder;
            SmscanBackUpFolder = backupFolder;
            SmscanGeodatabase = geodatabase;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Builder method to process data from the zipped file to geodatabase
        /// </summary>
        /// <remarks>
        /// The folder structure in a zip file are predefined as follows
        /// 1-GIS: include consolidate sheet excel files only
        /// 2-IMAGE: include ground penetration radar images
        /// 3-PHOTO: high resolution pictures taken by handheld camera
        /// 4-REPORT: pdf format based report reflecting survey situation
        /// 5-VBOX: vbox data with GPS tracking data, excluding video data
        /// 6-OTHER: any other information excluded from above data types
        /// </remarks>
        public void BuildService()
        {
            //Validate all parameters before data processing
            ValidateData();

            //Unzip the zip file with folder structure 
            UnzipData();

            //Build a scheme with structure information
            ConstructScheme();

            //Resize the images in Image folder and Photo folder
            ResizeImages();

            //Parse data from Vbox folder to get GPS trace
            ProcessVbox();

            //Copy empty geodatabase to store data processe
            CopyGeodatabase();

            //Implement a python script to manipulate fields to required formats
            PopulateGIS();

            //Copy data from local machine to network drive
            CopyFileFolder();

            //Send Notification
            SendNotificationWithPath();
        }

        private void CopyFileFolder()
        {
            string localFd = WorkingScheme.SchemePath;
            string remoteFd = Path.Combine(Global.SMNetdriveFolder, WorkingScheme.SchemeName);

            if (Directory.Exists(remoteFd))
            {
                Directory.Delete(remoteFd);
            }

            DirectoryHelper.Copy(localFd, remoteFd);
            AppLogger.LogInformation("Drainage File database is sucessfully created");


        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Validate all data, files, path,etc before processing data
        /// </summary>
        private void ValidateData()
        {
            if (!ZipFileSource.Exists)
            {
                throw new Exception("The zipfile not exists, please check if file exists!");
            }

            if (SmscanGeodatabase == null)
            {
                throw new Exception("You must specify geodatabase to process Smartscan data!");
            }

            if ((SmscanSourceFolder == null)||(SmscanBackUpFolder == null))
            {
                throw new Exception("Either Source Folder or Backup Folder does not exist, please check!");
            }
        }

        /// <summary>
        /// Unzip the downloaded zip file from WINSCP file server
        /// </summary>
        private void UnzipData()
        {
            if (ZipFileSource.Exists)
            {
                SmscanSchemePath = DirectoryHelper.UnzipFile(ZipFileSource);
            }
        }

        /// <summary>
        /// Check the unzipped file to get file structure and file contents
        /// </summary>
        private void ConstructScheme()
        {
            if (SmscanSchemePath != null)
            {
                //Build scheme information
                WorkingScheme = new SmscanScheme(SmscanSchemePath);
                WorkingScheme.BuildScheme();
            }
            else
            {
                throw new Exception("Folder structure is invalided and scheme cannot be produced");
            }
        }

        /// <summary>
        /// Copy an empty geodatabase to perserve smartscan data
        /// </summary>
        private void CopyGeodatabase()
        {
            string localMdb = Path.Combine(WorkingScheme.SchemePath, "DRAINAGE_REPORTS.mdb");

            if (File.Exists(localMdb))
            {
                File.Delete(localMdb);
            }
            else
            {
                File.Copy(Global.SMGeodatabase, localMdb);
                AppLogger.LogInformation("Drainage File database is sucessfully created");
            }
        }

        /// <summary>
        /// Resize all images at Photo folder and Image folder for file size requirements
        /// </summary>
        private void ResizeImages()
        {
            foreach (DirectoryInfo item in WorkingScheme.SubSchemeDirects)
            {
                if (item.Name.Contains("IMAGE") || item.Name.Contains("PHOTO"))
                {
                    // Picture resolution is restricted to 200 pixel * 200 pixel to reduce the size
                    DirectoryHelper.ResizeGroupImages(item.FullName,200,200);
                }
            }
        }

        /// <summary>
        /// Find the vbox data folder to process GPS data to trace
        /// </summary>
        private void ProcessVbox()
        {
            if (WorkingScheme.SubSchemeDirects.Length > 0)
            {
                foreach (DirectoryInfo folder in WorkingScheme.SubSchemeDirects)
                {
                    if (folder.Name.Contains("VBOX"))
                    {
                        VboxFileController.ProcessVboxData(folder.FullName);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Parse data from consildate sheet in GIS folder
        /// </summary>
        /// <remarks>
        /// Note that this part of data processing is implemented in an Python script
        /// </remarks>
        private void PopulateGIS()
        {
            string myPythonApp = Global.PyscriptPath;
            string projectPath = WorkingScheme.SchemeDirect.FullName;

            if (!File.Exists(myPythonApp))
            {
                throw new Exception($"Python script do not exist");
            }

            AppLogger.LogInformation($"{WorkingScheme.SchemeName}: Python script starts to process");

            //Initialise a python command line to implement python script
            PythonController pyController = new PythonController();
            pyController.Start(myPythonApp, projectPath);

            AppLogger.LogInformation($"Python Processing is completed");
        }

        /// <summary>
        /// Send emails to the registered stakeholders
        /// </summary>
        private void SendNotificationWithPath()
        {
            //MailSender.SendNotification(WorkingScheme.SchemeName);
            string remoteFd = Path.Combine(Global.SMNetdriveFolder, WorkingScheme.SchemeName);
            MailSender.SendNotificationWithPath(WorkingScheme.SchemeName, remoteFd);
            AppLogger.LogInformation($"Smartscan email notification has been sent!");
        }

        #endregion

    }
}
