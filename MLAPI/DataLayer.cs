using Microsoft.AnalysisServices.AdomdClient;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Caching;
using Newtonsoft.Json.Linq;
using System;
using System.Web;
using System.Collections.Generic;
using CsvHelper;
using System.IO;
using System.Text;

namespace HaloBI.Prism.Plugin
{
    public class DataLayer

    {
        private JObject _context;

        internal DataLayer(JObject context)
        {
            _context = context;
        }

        /// <summary>
        /// Get data cache for specified paneId and write to a file
        /// </summary>
        /// <param name="paneId"></param>
        internal void WriteTimeSeriesToFile(DataTable dt, string filePath)
        {
            String headerStr = "SKU_NUMBER,MASKED_SKU,ORDER_DATE,QUANTITY";
            String data = "";
            StringBuilder dataStr = new StringBuilder();
            dataStr.AppendLine(headerStr);
            
            foreach (DataRow r in dt.Rows)
            {
                data += "1,1," + r["name"] + "," + r["col0"] + "\n";
            }

            dataStr.Append(data);
            File.WriteAllText(filePath, dataStr.ToString());
        }
        
        internal void WriteADARInput(DataTable dt, string filePath)
        {
            String headerStr = "SKU_NUMBER,MASKED_SKU,ORDER_DATE,QUANTITY";
            String data = "";
            StringBuilder dataStr = new StringBuilder();

            dataStr.AppendLine(headerStr);

            var maxADAR = dt.Rows.Count - 60;
            for (var i = 0; i < maxADAR; i++)
            {
                DataRow r = dt.Rows[i];
                data += "1,1," + r["name"] + "," + r["col0"] + "\n";
            }
            dataStr.Append(data);
            File.WriteAllText(filePath, dataStr.ToString());

        }

        public DataTable GetDataTable(string paneId)
        {
            var cacheKey = GetCacheKey(paneId);
            var dt = new DataTable();

            try
            { 
                dt = (DataTable)MemoryCache.Default.Get(cacheKey);
            }
            catch(Exception ex)
            {
                throw ex;
                // log the exeption
            }

            if (dt == null)
            {
                throw new Exception($"dt is null. cachekey:{cacheKey}");
            }

            return dt;
        }


		private string GetCacheKey(string paneId)
        {
            var viewId = _context["view"]["id"].ToString(); 
            var sessionId = HttpContext.Current.Session.SessionID;
            var cacheKey = $"{sessionId}-{viewId}-{paneId}";
            return cacheKey;
        }

    }
}