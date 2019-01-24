using Microsoft.AnalysisServices.AdomdClient;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Caching;
using Newtonsoft.Json.Linq;
using System;
using System.Web;

namespace HaloBI.Prism.Plugin
{
    public class DataLayer

	{
        private JObject _context;

		internal DataLayer(JObject context)
		{
            _context = context;
		}

        internal DataTable GetDataTable(string paneId)
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