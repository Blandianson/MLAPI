
using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Diagnostics;
using System.Web.Services;
using Newtonsoft.Json;
using System.Threading;
using Newtonsoft.Json.Linq;

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
        /// Triggers Forecasting R script and returns TS forecast in text form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        protected void Request_click(object sender, EventArgs e)
        {
            // Test getting data from cache

            var context = GetContext(_contextId);
            var dataLayer = new DataLayer(context);
            var timeSeries = dataLayer.GetData("002");

            if (timeSeries != null)
            {
                callForecast();
                Thread.Sleep(15000);
                inputData.Text = readOutput("input_to_ADAR.csv");
                cleanedData.Text = readOutput("cleaned_data.csv");
                forecastData.Text = readOutput("output.csv");
                outputText.Text += readOutput("input_to_ADAR.csv") + readOutput("output.csv");
            }
            else
            {
                inputData.Text = "Timeseries is null";
            }
        }

        /// <summary>
        /// Reads in the base, forecast, and cleaned Data.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        protected String readOutput(String filename)
        {
            string path = @"C:\Halo\ADAR\inputs and outputs\" + filename;
            String outputLines = "";

            while(!File.Exists(path))
            {
                outputText.Text += "Data is loading";
                Thread.Sleep(15000);
            }

            StreamReader sr = File.OpenText(path);
            string s;
            while((s = sr.ReadLine()) != null)
            {
                outputLines += s + "\n";
            }
            return outputLines;
        }

        protected void callForecast()
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

            //Is customisable though users won't understand it hence is hard-coded in backend
            String sessionID = "38CE52DA-BE16-45C5-A0C8-D90EE9A07ED6";

            // "  "
            String executionID = "100";

            //Pass the parameters to the R Script via cmd argument
            cmdStartInfo.Arguments = "send -s " + sessionID + " -f " + executionID + " -i localhost -d xxx -t 0 -l \"C:\\Halo\\ADAR\\AnomalyDetectionAndRemoval.R\" -p \"inputs and outputs\" \"config\" \"input_to_ADAR\"";

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
    }

    #endregion
}