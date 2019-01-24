
using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Diagnostics;
using System.Web.Services;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Security;

namespace HaloBI.Prism.Plugin
{
    public partial class MLAPI : System.Web.UI.Page
    {
#region Plugin Config

        string _contextId = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            _contextId = Request.QueryString["sessionId"];

            if (!Page.IsPostBack)
            {
                var context = GetContext(_contextId);

                // assume iFrame embed mode
                SetContent(context);
            }
        }

        /// <summary>
        /// Initialize plugin on call from client
        /// Set any plugin specific properties here
        /// </summary>
        /// <param name="plugin"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public static object Initialize(object plugin)
        {
            // pull context from session
            var s = JsonConvert.SerializeObject(plugin);
            var w = JObject.FromObject(plugin);
            var sessionId = w["sessionId"].ToString();
            var context = GetContext(sessionId);

            // set plugin properties and store in session
            context["plugin"]["name"] = "Template";
            context["plugin"]["physicalPath"] = Path.GetDirectoryName(
                HttpContext.Current.Request.PhysicalPath);
            context["plugin"]["embedMode"] = "iFrame";
            context["plugin"]["height"] = 800;
            context["plugin"]["width"] = 600;

            // since 15.2.0408 and 15.1 SR1 plugin config information is stored in the Prism View
            // override plugin config by loading from a file if desired. 
            context["plugin"]["config"] = GetConfig(
                     Path.Combine(context["plugin"]["physicalPath"].ToString(),
                     "config.json")
            );

            // save context to session
            SetContextToSession(context, sessionId);

            // return details to client 
            return new
            {
                embedMode = context["plugin"]["embedMode"].ToString()
            };
        }

        /// <summary>
        /// Read config from disk
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static JObject GetConfig(string fileName)
        {
            var text = "";

            using (StreamReader file = File.OpenText(fileName))
            {
                text = file.ReadToEnd();
            }

            return JObject.Parse(text); 
        }

        /// <summary>
        /// Set the content
        /// </summary>
        /// <param name="context["plugin"]["config"]></param>
        /// <returns></returns>
        private void SetContent(JObject context)
        {

            // now the all important client side hook to update the Prism view
            var viewId = context["view"]["id"].ToString();
            var paneId = context["view"]["paneId"].ToString();
			var clientsideUpdateFunction = string.Format("window.parent.halobi.plugin.prismUpdate('{0}','{1}','{2}', true);",
                    viewId,
                    paneId,
                    ""
			);

            //debg.Text = GetContext(_contextId).ToString();
        }
        
        /// <summary>
        /// Load Plugin Context from Session
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        private static JObject GetContext(string sessionId)
        {
            return JObject.Parse(
                HttpContext.Current.Session[sessionId].ToString()
            );
        }

        /// <summary>
        /// Store Plugin Context to Session
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sessionId"></param>
        private static void SetContextToSession(JObject context, string sessionId)
        {
            HttpContext.Current.Session[sessionId] = JsonConvert.SerializeObject(context);
        }

        #endregion

        #region  Working code

        /// <summary>
        /// Cleans the TS of quotes and makes it csv. To do(Subject to Change with input)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        protected String TransformParams(String param)
        {
            param = param.Replace(",", ";");
            return param = param.Replace("\"", "");
        }

        /// <summary>
        /// Extracts the date from TS data To do(Subject to Change with input)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        protected String TransformData(String date)
        {
            return date.Substring(0,7);
        }

        /// <summary>
        /// Triggers Forecasting R script and returns TS forecast in text form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Request_click(object sender, EventArgs e)
        {
            // Test getting data from cache

            var context = GetContext(_contextId);
            var dataLayer = new DataLayer(context);
            var timeSeries = dataLayer.GetData("002");



            if (timeSeries != null)
            {
                debg.Text = timeSeries;
            }
            else
            {
                debg.Text = "Timeseries is null";
            }



            //Function controlling cmd input to Rscript and output from SQL Server
            //FileIO();

            //Database checking timeout
            var endTime = DateTime.Now.AddSeconds(120);

            while (DateTime.Now.CompareTo(endTime) < 0)
            {
                object result = "";
                object status = DBWait(result);                         
                
                //Connects to SQL Server and extracts output data from R forecasting script
                if (status != null)
                {
                    break;
                }
                else
                {
                    //If data isn't ready in database, Main thread sleeps and tries again
                    Thread.Sleep(15000);
                }
            }
        }

        /// <summary>
        /// Function controlling cmd input to Rscript and output from SQL Server
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected object DBWait(object result)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                String localhostName = getLegalLocalName(server.Text);
                
                conn.ConnectionString = "Server=" + localhostName + "; Database=HaloMessageClient;Trusted_Connection=true";
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT TOP 1 * FROM Queue.Message ORDER BY DateCreated DESC", conn);
                
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.IsDBNull(4))
                        {
                            //Data is not ready - only partially stored in the database.
                            result = null;
                        }
                        else
                        {
                            //Data is present. Outputted to screen
                            result = reader[4];                         
                            outputText.Text += String.Format("DateCreated: {0}, Output: {1}\n", reader[5], reader[8]);
                        }
                        
                    }
                }
                conn.Close();
            }
            return result;
        }

        /// <summary>
        /// Main function controlling the I/O to and from R Script
        /// </summary>
        protected void FileIO()
        {

            //Triggers the background Process to recieve cmd output
            Process receiveProcess = ReceiveCmd();

            //Triggers the background Process to recieve R script output
            Process receiveResponseProcess = ReceiveResponseCmd();

            //Triggers the Process to execute R script
            ProcessStartInfo cmdStartInfo = new ProcessStartInfo
            {
                //Subject to Change? To do
                FileName = @"C:\Program Files\Halo\HaloMW\MessageConsole\Halo.ML.MessageConsole.exe",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = false,
                Verb = "runas"
            };

            //Parameters to send to R Script via cmd

            //Is customisable though users won't understand it hence is hard-coded in backend
            String sessionID = "38CE52DA-BE16-45C5-A0C8-D90EE9A07ED6";
            // "  "
            String executionID = "100";
            //Gets legally qualified name of database server
            String serverText = getLegalLocalName(server.Text);
            String stagingText = escapeEmptyStr(staging.Text);
            String rScriptText = escapeEmptyStr(rScript.Text);
            //Tidies into R friendly TS data
            String param = TransformParams(parameters.Text);
            String startDate = TransformData(start.Text.ToString());
            String endDate = TransformData(end.Text.ToString());

            //Pass the parameters to the R Script via cmd argument

            cmdStartInfo.Arguments = "send -s " + sessionID + " -f " + executionID + " -i " + serverText + " -d " + stagingText + " -t " + fileType.SelectedValue + " -l " + rScript.Text + " -p " + startDate + "," + endDate + "," + column.Text + "," + forecast.Text + "," + param;

            Process cmdProcess = new Process
            {
                StartInfo = cmdStartInfo
            };
            cmdProcess.EnableRaisingEvents = true;
            cmdProcess.Start();
            cmdProcess.BeginOutputReadLine();
            cmdProcess.BeginErrorReadLine();

            //Execute exit on Sending Process
            cmdProcess.StandardInput.WriteLine("exit");

            //Wait for gracefull exit
            cmdProcess.WaitForExit();

            //Closes the background Process to recieve cmd output
            receiveProcess.Close();

            //Triggers the background Proess to recieve R script output
            receiveResponseProcess.Close();

            return;
        }

        /// <summary>
        /// Triggers the background Process to recieve cmd output
        /// </summary>
        /// <returns></returns>
        protected Process ReceiveCmd()
        {
            ProcessStartInfo cmdStartInfoRec = new ProcessStartInfo
            {
                //Subject to change
                FileName = @"C:\Program Files\Halo\HaloMW\MessageConsole\Halo.ML.MessageConsole.exe",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = false,
                Verb = "runas",
                Arguments = "receive"
            };

            Process cmdProcessRec = new Process
            {
                StartInfo = cmdStartInfoRec
            };
            cmdProcessRec.EnableRaisingEvents = true;
            cmdProcessRec.Start();
            cmdProcessRec.BeginOutputReadLine();
            cmdProcessRec.BeginErrorReadLine();

            cmdProcessRec.StandardInput.Flush();

            return cmdProcessRec;
        }

        /// <summary>
        /// Triggers the background Process to recieve R script output
        /// </summary>
        /// <returns></returns>
        protected Process ReceiveResponseCmd()
        {
            ProcessStartInfo cmdStartInfoRecResp = new ProcessStartInfo
            {
                //Subject to change
                FileName = @"C:\Program Files\Halo\HaloMW\MessageConsole\Halo.ML.MessageConsole.exe",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = false,
                Verb = "runas",
                Arguments = "receiveresponse"
            };

            Process cmdProcessResp = new Process
            {
                StartInfo = cmdStartInfoRecResp
            };
            cmdProcessResp.EnableRaisingEvents = true;
            cmdProcessResp.Start();
            cmdProcessResp.BeginOutputReadLine();
            cmdProcessResp.BeginErrorReadLine();

            cmdProcessResp.StandardInput.Flush();

            return cmdProcessResp;
        }

        /// <summary>
        /// Gets legally qualified name of database server
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        protected String getLegalLocalName(String serverName)
        {
            if (server.Text == "localhost /gmi")
            {
                return System.Environment.MachineName;
            }
            else if (serverName == "")
            {
                return "null";
            }
            else
            {
                return SecurityElement.Escape(server.Text);
            }
        }

        /// <summary>
        /// No empty parameter is passed into the cmd. Cmd will mistake empty param as EOL.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        protected String escapeEmptyStr(String field)
        {
            if (field == "")
            {
                return "null";
            }
            else
            {
                return SecurityElement.Escape(field);
            }
        }

        #endregion 
        
    }
}