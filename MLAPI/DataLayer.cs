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
        
        internal String GetData(string paneId)
        {
            String headerStr = "SKU_NUMBER,MASKED_SKU,ORDER_DATE,QUANTITY";
            String data = "";
            var filePath = @"C:\Halo\ADAR\input_to_ADAR.csv";
            StringBuilder dataStr = new StringBuilder();

            dataStr.AppendLine(headerStr);

            var dt = GetDataTable(paneId);

            foreach (DataRow r in dt.Rows)
            {
                data += "1,1," + r["name"] + "," + r["col0"] + "\n";
            }

            dataStr.Append(data);
            dataStr.Append(data);
            File.WriteAllText(filePath, dataStr.ToString());

            return data;
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