using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using Ionic.Zip;

namespace backend.Models
{
    public class CheckBoxItem
    {
        public int Code { get; set; }
        public bool IsChecked { get; set; }
        public string Label { get; set; }

        public static List<int> GetSelectedIds(List<CheckBoxItem> list)
        {
            return list.Where(l => l.IsChecked).Select(l => l.Code).ToList();
        }
    }
    /** Implemented Bootstrap style in EditorTemplates **/
    public class CheckBoxItem2
    {
        public int Code { get; set; }
        public bool IsChecked { get; set; }
        public string Label { get; set; }

        public static List<int> GetSelectedIds(List<CheckBoxItem2> list)
        {
            return list.Where(l => l.IsChecked).Select(l => l.Code).ToList();
        }
    }

    public class ZipResult : ActionResult
    {
        public List<System.IO.FileInfo> Files { get; set; }
        public string Filename { get; set; }
        public List<FileEntry> FileEntries { get; set; }
        public CompressionMethod Method { get; set; }

        public ZipResult(IEnumerable<System.IO.FileInfo> files, string fileName, CompressionMethod method = CompressionMethod.Deflate)
        {
            Files = new List<System.IO.FileInfo>(files);
            Filename = fileName;
            Method = method;
        }

        public ZipResult(IEnumerable<FileEntry> entries, string fileName)
        {
            FileEntries = new List<FileEntry>(entries);
            Filename = fileName;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var response = context.HttpContext.Response;
            response.ContentType = "application/gzip";
            using (var zip = new ZipFile())
            {
                zip.CompressionMethod = Method;
                
                if (Files != null)
                {
                    foreach (var file in Files)
                    {
                        zip.AddFile(file.FullName, "");
                    }
                }

                if (FileEntries != null)
                {
                    foreach (var fe in FileEntries)
                    {
                        zip.AddEntry(fe.Name, fe.Stream);
                    }
                }
                
                zip.Save(response.OutputStream);
                var cd = new ContentDisposition
                {
                    FileName = Filename,
                    Inline = false
                };
                response.AddHeader("Content-Disposition", cd.ToString());
            }
        }
    }

    public class FileEntry
    {
        public Stream Stream { get; set; }
        public string Name { get; set; }
    }



    public class ConvertToPdfModel
    {
        public string Url { get; set; }
        public string Options { get; set; }
    }
}