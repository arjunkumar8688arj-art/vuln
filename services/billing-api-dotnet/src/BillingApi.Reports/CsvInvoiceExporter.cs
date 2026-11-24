using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;

namespace BillingApi.Reports
{
    public class InvoiceRow
    {
        public string Number { get; set; }
        public string CustomerId { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime DueAt { get; set; }
        public string Status { get; set; }
    }

    /// <summary>
    /// Writes invoice rows to CSV and bundles finished reports into a zip archive
    /// for the finance team's monthly close.
    /// </summary>
    public class CsvInvoiceExporter
    {
        public void Export(IEnumerable<InvoiceRow> rows, string path)
        {
            var sb = new StringBuilder();
            sb.AppendLine("number,customer_id,issued_at,due_at,status");
            foreach (var row in rows)
            {
                sb.Append(Escape(row.Number)).Append(',')
                  .Append(Escape(row.CustomerId)).Append(',')
                  .Append(row.IssuedAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)).Append(',')
                  .Append(row.DueAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)).Append(',')
                  .AppendLine(Escape(row.Status));
            }
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }

        public void Bundle(IEnumerable<string> files, string zipPath)
        {
            using (var fs = File.Create(zipPath))
            using (var zip = new ZipOutputStream(fs))
            {
                foreach (var file in files)
                {
                    zip.PutNextEntry(new ZipEntry(Path.GetFileName(file)));
                    var bytes = File.ReadAllBytes(file);
                    zip.Write(bytes, 0, bytes.Length);
                    zip.CloseEntry();
                }
            }
        }

        private static string Escape(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (value.IndexOfAny(new[] { ',', '"', '\n' }) < 0) return value;
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
    }
}
