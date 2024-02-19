using System.IO;
using System.Linq;
using System;

namespace Max.Reporting.Infrastructure.MySqlStorage
{
    public static class ReportDocumentUtils
    {
        private const int zipLeadBytes = 67324752;

        internal const string DefaultExtension = ".trdx";

        private static readonly string[] supportedExt = new string[3] { ".trdx", ".trdp", ".trbp" };

        public static bool IsSupportedReportDocument(string path)
        {
            return supportedExt.Any((string s) => path.EndsWith(s, StringComparison.OrdinalIgnoreCase));
        }

        internal static string ResoveExtension(Stream stream)
        {
            string empty = string.Empty;
            if (stream.CanSeek)
            {
                long position = stream.Position;
                stream.Seek(0L, SeekOrigin.Begin);
                byte[] array = new byte[4];
                stream.Read(array, 0, array.Length);
                stream.Seek(position, SeekOrigin.Begin);
                if (BitConverter.ToInt32(array, 0) == 67324752)
                {
                    return ".trdp";
                }
            }

            return empty;
        }
    }
}
