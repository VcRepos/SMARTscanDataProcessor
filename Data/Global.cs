using System.Configuration;

namespace SMARTscan_DataProcessor.Data
{
    public static class Global
    {
        #region Constants

        public static string SMsourceFolder = ConfigurationManager.AppSettings["SourceFolder"];
        public static string SMbackupFolder = ConfigurationManager.AppSettings["BackupFolder"];
        public static string SMGeodatabase = ConfigurationManager.AppSettings["MdbDatabase"];

        public static string PyscriptPath = ConfigurationManager.AppSettings["Pyscript"];
        public static string Pyenvironment = ConfigurationManager.AppSettings["Pyenvironment"];

        public static string SmtpServer = ConfigurationManager.AppSettings["Smtp_server"];
        public static string SmtpPort = ConfigurationManager.AppSettings["Smtp_port"];
        public static string SmtpFromTo = ConfigurationManager.AppSettings["Smtp_fromto"];

        public static string Hao = ConfigurationManager.AppSettings["Hao"];

        #endregion


    }
}
