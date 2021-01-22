using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace TechShare.Utility.Tools.Export
{
    public static class KMLExportHelper
    {
        private static readonly int DEFAULT_POINTLIMIT = 2000;
        private static readonly string EXTENSION = ".kml";
        public static string SaveKMLFile(string savePath, string fileName, KMLDocument document)
        {
            if (string.IsNullOrEmpty(savePath))
                throw new ArgumentException(typeof(KMLExportHelper).Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - Path is required");
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException(typeof(KMLExportHelper).Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - Filename is required");

            string savedFileName = string.Empty;
            if (document != null && document.HasData)
            {
                if (!savePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    savePath += Path.DirectorySeparatorChar;

                if (!Directory.Exists(savePath))
                    Directory.CreateDirectory(savePath);

                if (!fileName.EndsWith(EXTENSION))
                    fileName += EXTENSION;

                savedFileName = savePath + fileName;
                File.WriteAllText(savedFileName, document.ToString());
            }
            return savedFileName;
        }
        public static IEnumerable<string> SaveKMLFiles(string savePath, string baseFileName, IEnumerable<KMLDocument> documents)
        {
            if (string.IsNullOrEmpty(savePath))
                throw new ArgumentException(typeof(KMLExportHelper).Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - Path is required");
            if (string.IsNullOrEmpty(baseFileName))
                throw new ArgumentException(typeof(KMLExportHelper).Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - Filename is required");

            List<string> savedFileNames = new List<string>();
            if (documents != null && documents.Any(x => x.HasData))
            {
                string filenameNoExt = baseFileName.Replace(EXTENSION, "");

                if (documents.Count() == 1)
                {
                    savedFileNames.Add(SaveKMLFile(savePath, baseFileName, documents.First()));
                }
                else
                {
                    int count = 1;
                    foreach (KMLDocument document in documents)
                    {
                        savedFileNames.Add(SaveKMLFile(savePath, filenameNoExt + "_" + count.ToString() + EXTENSION, document));
                        count++;
                    }
                }
            }
            return savedFileNames;
        }
        public static IEnumerable<string> SaveKMLFiles(string savePath, string baseFileName, DataTable data, string nameCol = null,
                string descriptionCol = null, string latitudeCol = null, string longitudeCol = null, string altitudeCol = null, int? pointLimit = null)
        {
            pointLimit = pointLimit ?? DEFAULT_POINTLIMIT;
            IEnumerable<KMLDocument> documents = GenerateKMLDocumentObjects(data, nameCol, descriptionCol, latitudeCol, longitudeCol, altitudeCol, pointLimit);
            if (documents != null && documents.Any(x => x.HasData))
            {
                return SaveKMLFiles(savePath, baseFileName, documents);
            }
            return null;
        }
        public static IEnumerable<string> SaveKMLFiles(string savePath, string baseFileName, IEnumerable<IKMLPlacemark> data, int? pointLimit = null)
        {
            pointLimit = pointLimit ?? DEFAULT_POINTLIMIT;
            IEnumerable<KMLDocument> documents = GenerateKMLDocumentObjects(data, pointLimit);
            if (documents != null && documents.Any(x => x.HasData))
            {
                return SaveKMLFiles(savePath, baseFileName, documents);
            }
            return null;
        }
        public static IEnumerable<KMLDocument> GenerateKMLDocumentObjects(DataTable data, string nameCol, string descriptionCol,
            string latitudeCol, string longitudeCol, string altitudeCol = null, int? pointLimit = null)
        {
            pointLimit = pointLimit ?? DEFAULT_POINTLIMIT;
            if (data == null)
                throw new ArgumentException(typeof(KMLExportHelper).Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - Table is null");
            if (string.IsNullOrEmpty(nameCol))
                throw new ArgumentException(typeof(KMLExportHelper).Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - Name column is required");
            if (!data.Columns.Contains(nameCol))
                throw new ArgumentException(typeof(KMLExportHelper).Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - Name column does not exist in table");

            if (string.IsNullOrEmpty(latitudeCol))
                throw new ArgumentException(typeof(KMLExportHelper).Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - Latitude column is required");
            if (!data.Columns.Contains(latitudeCol))
                throw new ArgumentException(typeof(KMLExportHelper).Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - Latitude column does not exist in table");

            if (string.IsNullOrEmpty(longitudeCol))
                throw new ArgumentException(typeof(KMLExportHelper).Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - Longitude column is required");
            if (!data.Columns.Contains(longitudeCol))
                throw new ArgumentException(typeof(KMLExportHelper).Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - Longitude column does not exist in table");

            List<KMLDocument> retVal = new List<KMLDocument>();
            KMLDocument document = null;
            if (data != null && data.Rows.Count > 0)
            {
                document = new KMLDocument();
                List<KMLPlacemark> placemarks = new List<KMLPlacemark>();
                foreach (DataRow row in data.Rows)
                {
                    document.AddPlacemark(
                        row[nameCol].ToString(),
                        (!string.IsNullOrEmpty(descriptionCol) && data.Columns.Contains(descriptionCol) ? row[descriptionCol].ToString() : null),
                        Double.TryParse(row[latitudeCol].ToString(), out double tempLatitude) ? tempLatitude : (double?)null,
                        Double.TryParse(row[longitudeCol].ToString(), out double tempLongitude) ? tempLongitude : (double?)null,
                        (!string.IsNullOrEmpty(altitudeCol) && data.Columns.Contains(altitudeCol) &&
                        Double.TryParse(row[altitudeCol].ToString(), out double tempAltitude)) ? tempAltitude : (double?)null);

                    if (document.HasPlacemarks && document.Placemarks.Count() >= pointLimit)
                    {
                        retVal.Add(document);
                        document = new KMLDocument();
                    }
                }
                if (document.HasData)
                    retVal.Add(document);
            }
            return retVal;
        }
        public static IEnumerable<KMLDocument> GenerateKMLDocumentObjects(IEnumerable<IKMLPlacemark> data, int? pointLimit = null)
        {
            pointLimit = pointLimit ?? DEFAULT_POINTLIMIT;
            if (data == null || !data.Any(x => x.HasData))
                throw new ArgumentException(typeof(KMLExportHelper).Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - Data is null");

            List<KMLDocument> retVal = new List<KMLDocument>();
            KMLDocument document = new KMLDocument();
            List<KMLPlacemark> placemarks = new List<KMLPlacemark>();
            foreach (IKMLPlacemark pm in data)
            {
                document.AddPlacemark(pm.Name, pm.Description, pm.Latitude, pm.Longitude, pm.Altitude);

                if (document.HasPlacemarks && document.Placemarks.Count() >= pointLimit)
                {
                    retVal.Add(document);
                    document = new KMLDocument();
                }
            }
            if (document.HasData)
                retVal.Add(document);
            return retVal;
        }
    }
}
