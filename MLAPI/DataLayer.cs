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

        //internal List<List<string>> GetData(string paneId)
        internal String GetData(string paneId)
        {
            //var data = new List<List<string>>();
            String headerStr = "SKU_NUMBER,MASKED_SKU,ORDER_DATE,QUANTITY";
            String data = "";
            String filePath = @"C:\Users\Nicole.jackson\source\repos\MLAPI\Data\input_to_ADAR.csv";
            //String filePath = @"\\dev-ph5\share\interns\";
            StringBuilder dataStr = new StringBuilder();

            dataStr.AppendLine(headerStr);

            var dt = GetDataTable(paneId);

            foreach (DataRow r in dt.Rows)
            {
                //String nameCol0 = r["name"] + ", " + r["col0"];
                //List<string> rowData;
                //rowData.Add(nameCol0);
                //data.Add(rowData);

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
            var viewId = _context["view"]["id"].ToString(); // from context
            //var sessionId = _context["plugin"]["sessionId"].ToString(); // from context
            var sessionId = HttpContext.Current.Session.SessionID;
            var cacheKey = $"{sessionId}-{viewId}-{paneId}";
            return cacheKey;
        }

        /// <summary>
		/// Return MDX query results as a DataSet
		/// </summary>
		/// <param name="mdx"></param>
		/// <returns></returns>
		//internal DataSet GetData(string mdx)
		//{
		//	var builder = new SqlConnectionStringBuilder();
		//	builder.DataSource = Server;
		//	builder.InitialCatalog = Catalog;
		//	DataSet ds = null;

		//	using (var connection = new AdomdConnection(builder.ToString()))
		//	{
		//		connection.Open();
		//		ds = new DataSet();
		//		using (var adaptor = new AdomdDataAdapter(mdx, connection))
		//		{
		//			adaptor.Fill(ds);
		//		}
		//	}
			
		//	return ds;
		//}
	}
}