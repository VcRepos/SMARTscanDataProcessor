using System;
using System.IO;

namespace SMARTscan_DataProcessor.Data
{
    public class SmscanScheme
    {
        public string SchemeName { get; set; }
        public string SchemePath { get; set; }
        public DirectoryInfo SchemeDirect { get; set; }
        public DirectoryInfo[] SubSchemeDirects { get; set; }

        public SmscanScheme(string schemePath)
        {
            SchemePath = schemePath;
        }

        public void BuildScheme()
        {
            SchemeDirect = new DirectoryInfo(SchemePath);
            SchemeName = SchemeDirect.Name;
            SubSchemeDirects = SchemeDirect.GetDirectories();
        }
    }
}
