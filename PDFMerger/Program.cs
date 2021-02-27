using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PDFMerger {
  class Program {
    private static string workingDir = Environment.CurrentDirectory;

    public static void Main(string[] args) {
      var mergeFolders = CreateMergeFolders(workingDir);
      if (mergeFolders.Count > 0) {
        foreach (var folder in mergeFolders) {
          var folderName = Path.GetFileName(folder);
          if (folderName.StartsWith("PDFMerger")) {
            MergeFiles(folder);
            CleanUpFiles(folder);
          }
        }
      }
    }

    private static List<string> CreateMergeFolders(string directory) {
      List<string> mergeFolders = new List<string>();
      var files = Directory.GetFiles(directory, "*.pdf");
      foreach (var file in files) {
        var fileName = Path.GetFileName(file);
        var fileNameClean = Path.GetFileNameWithoutExtension(file);
        string fileType = "PDFMerger_" + fileNameClean.Split('_')[1];

        string typeDir = Path.Combine(directory, fileType);
        if (!Directory.Exists(typeDir)) {
          mergeFolders.Add(typeDir);
          Directory.CreateDirectory(typeDir);
        }
        string copyFile = Path.Combine(typeDir, fileName);
        if (!File.Exists(copyFile)) {
          File.Copy(file, copyFile);
        }
      }
      return mergeFolders;
    }

    private static void MergeFiles(string directory) {
      var fileType = Path.GetFileNameWithoutExtension(directory).Split('_')[1];
      var files = Directory.EnumerateFiles(directory, "*.pdf").OrderBy(filename => filename).ToArray();
      string targetFile = Path.Combine(directory, "Merged_" + fileType + ".pdf");
      MergePDFs(targetFile, files);
    }

    public static void MergePDFs(string targetPath, params string[] pdfs) {
      using (PdfDocument targetDoc = new PdfDocument()) {
        foreach (string pdf in pdfs) {
          using (PdfDocument pdfDoc = PdfReader.Open(pdf, PdfDocumentOpenMode.Import)) {
            for (int i = 0; i < pdfDoc.PageCount; i++) {
              targetDoc.AddPage(pdfDoc.Pages[i]);
            }
          }
        }
        targetDoc.Save(targetPath);
      }
    }

    public static void CleanUpFiles(string directory) {
      var files = Directory.GetFiles(directory);
      foreach (var file in files) {
        string fileName = Path.GetFileName(file);
        if (!fileName.StartsWith("Merged")) {
          File.Delete(file);
        }
      }
    }
  }
}
