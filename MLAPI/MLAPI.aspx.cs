
using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.Services;
using Newtonsoft.Json;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Data;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace HaloBI.Prism.Plugin
{
    public partial class MLAPI : System.Web.UI.Page
    {
        #region Plugin Config

        string _contextId = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            _contextId = Request.QueryString["sessionId"];

            if (_contextId == null)
            {
                throw new Exception("_contextId is null");
            }

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
            if (forscastType.SelectedIndex == 1) // Holiday Oversampling
            {
                inputData.Text = readOutput(@"demo\Holiday\full_base_data.csv");
                cleanedData.Text = readOutput(@"demo\Holiday\full_cleaned_data.csv");
                cleanForecast.Text = readOutput(@"demo\Holiday\output.csv");
                actualForecast.Text = readOutput(@"demo\Holiday\output_actual.csv");
                outputText.Text += readOutput(@"demo\Holiday\full_base_data.csv") + readOutput(@"demo\Holiday\output.csv");
            }
            else if (forscastType.SelectedIndex == 2) // Demo Mode
            {
                inputData.Text = readOutput(@"demo\2019-02-05-1144\full_base_data.csv");
                cleanedData.Text = readOutput(@"demo\2019-02-05-1144\full_cleaned_data.csv");
                cleanForecast.Text = readOutput(@"demo\2019-02-05-1144\output.csv");
                actualForecast.Text = readOutput(@"demo\2019-02-05-1144\output_actual.csv");
                outputText.Text += readOutput(@"demo\2019-02-05-1144\full_base_data.csv") + readOutput(@"demo\2019-02-05-1144\output.csv");
            }
            else if (forscastType.SelectedIndex == 3) // Demo Mode
            {
                inputData.Text = readOutput(@"demo\twoMonthADAR\full_base_data.csv");
                cleanedData.Text = readOutput(@"demo\twoMonthADAR\full_cleaned_data.csv");
                cleanForecast.Text = readOutput(@"demo\twoMonthADAR\output.csv");
                actualForecast.Text = readOutput(@"demo\twoMonthADAR\output_actual.csv");
                outputText.Text += readOutput(@"demo\twoMonthADAR\full_base_data.csv") + readOutput(@"demo\twoMonthADAR\output.csv");
            }
            else if (forscastType.SelectedIndex == 4) // Demo Mode
            {
                outputText.Text += "Holiday Data:";
                inputData.Text = readOutput(@"demo\twoMonthHoliday\full_base_data.csv");
                cleanedData.Text = readOutput(@"demo\twoMonthHoliday\full_cleaned_data.csv");
                cleanForecast.Text = readOutput(@"demo\twoMonthHoliday\output.csv");
                actualForecast.Text = readOutput(@"demo\twoMonthHoliday\output_holiday.csv");
                outputText.Text += readOutput(@"demo\twoMonthHoliday\full_base_data.csv") + readOutput(@"demo\twoMonthHoliday\output.csv");
            }
            else
            {
                var context = GetContext(_contextId);

                var dataLayer = new DataLayer(context);
                var timeSeriesDataTable = dataLayer.GetDataTable("002");


                dataLayer.WriteTimeSeriesToFile(timeSeriesDataTable, @"C:\Halo\ADAR\inputs and outputs\full_base_data.csv");
                dataLayer.WriteADARInput(timeSeriesDataTable, @"C:\Halo\ADAR\inputs and outputs\input_to_ADAR.csv");

                triggerADAR();

                inputData.Text = readOutput("full_base_data.csv");
                cleanedData.Text = readOutput("full_cleaned_data.csv");
                cleanForecast.Text = readOutput("output.csv");
                actualForecast.Text = readOutput("output_actual.csv");
                outputText.Text += readOutput("full_base_data.csv") + readOutput("output.csv");

                List<List<string>> newConfig = readConfig();
                writeConfig(newConfig);
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


            while (!File.Exists(path)) ///This was the error, file was being written to another file.
            {
                outputText.Text += "Data is loading";
                Thread.Sleep(5000);
            }

            StreamReader sr = File.OpenText(path);
            string s;
            while((s = sr.ReadLine()) != null)
            {
                outputLines += s + "\n";
            }
            sr.Close();
            return outputLines;
        }

        protected void triggerADAR()
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

            //Pass the parameters to the R Script via cmd argument
            cmdStartInfo.Arguments = "send -s 38CE52DA-BE16-45C5-A0C8-D90EE9A07ED6 -f 100 -i DESKTOP-SK1K62B -d xxx -t 0 -l C:\\Halo\\ADAR\\AnomalyDetectionAndRemoval.R -p \"inputs and outputs,config,input_to_ADAR\"";
            
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

        #region Change Config

        /// <summary>
        /// Reads in the config data from file
        /// </summary>
        /// <returns></returns>
        protected List<List<string>> readConfig()
        {
            string path = @"C:\Halo\ADAR\inputs and outputs\demo\Holiday\config.csv";
            List<List<string>> outputLines = new List<List<string>>();

            StreamReader sr = File.OpenText(path);
            string s;
            while ((s = sr.ReadLine()) != null)
            {
                var elements = s.Split(',');
                List<string> innerList = new List<string>
                {
                    elements[0], elements[1]
                };
                outputLines.Add(innerList);
            }
            sr.Close();

            //Output Each Line of the inner Array
            //foreach (var sublist in outputLines)
            //{
            //    outputText.Text += String.Join(", ", sublist) + "\n";
            //}

            //Output Each element of the inner Array
            //foreach (var sublist in outputLines)
            //{
            //    foreach (var element in sublist)
            //    {
            //        outputText.Text += element + "\n";

            //    }
            //}
            List<List<string>> configData = configStrToDatTab(outputLines);
            return outputLines;
        }

        /// <summary>
        /// Takes config data and alters Date and ADAR options
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        protected List<List<string>> configStrToDatTab(List<List<string>> config)
        {

            int firstPeriodsCount = 0;

            //Getting the first occurence of the PeriodsCount
            for (var i = 0; i < config.Count(); i++)
            {
                if ( config[i][0] == "PeriodsCount")
                {
                    firstPeriodsCount = i;
                }
                
            }
            //Before Config change
            //foreach (var sublist in config)
            //{
            //    outputText.Text += "Config (Original): " + String.Join(", ", sublist) + "\n"; //Changed Array
            //}


            for (var i = 0; i < config.Count(); i++)
            {
                List<string> atrribData = new List<string>();
                var confArray = config[i][0];

                if (confArray.Contains("DateEnd") | confArray.Contains("Period_End"))
                {
                    var dataEnd = GetConfigFirstDataPt(false);
                    var formattedDate = DateFormat(dataEnd);
                    atrribData.Add(confArray);
                    atrribData.Add(formattedDate);
                    config[i] = atrribData;
                }
                if (confArray.Contains("DateStart") | confArray.Contains("Period_Start") | confArray.Contains("DsDate"))
                {
                    var dataStart = GetConfigFirstDataPt(true);
                    var formattedDate = DateFormat(dataStart);
                    atrribData.Add(confArray);
                    atrribData.Add(formattedDate);
                    config[i] = atrribData;
                }
                if (i == firstPeriodsCount)  //Changes an additional setting with the same name ((config).IndexOf("PeriodsCount"))
                {
                    atrribData.Add("PeriodsCount");
                    atrribData.Add("60");
                    config[i] = atrribData;
                }
                if (confArray.Contains("RunAnomalyDetectionOnly"))
                {
                    atrribData.Add("RunAnomalyDetectionOnly");
                    atrribData.Add("1");
                    config[i] = atrribData;
                }
                if (confArray.Contains("RunAnomalyDetectionWithHolidayOversampling"))
                {
                    atrribData.Add("RunAnomalyDetectionWithHolidayOversampling");
                    atrribData.Add("0");
                    config[i] = atrribData;
                }
                if (confArray.Contains("ValidationStart"))
                {
                    var dataEnd = GetConfigFirstDataPt(false);
                    var formattedDate = new DateTime(int.Parse(dataEnd.Substring(0, 4)), int.Parse(dataEnd.Substring(4, 2)), int.Parse(dataEnd.Substring(6)));
                    atrribData.Add("ValidationStart");
                    atrribData.Add(formattedDate.AddDays(-1).ToString("MM/dd/yyyy"));
                    config[i] = atrribData;
                }
            }

            //After Config Change
            //foreach (var sublist in config)
            //{
            //    outputText.Text += "Config (Changed): " + String.Join(", ", sublist) + "\n"; //Changed Array
            //}
            return config;
        }

        /// <summary>
        /// changes date format from yyyymmdd to mm/dd/yyyy
        /// </summary>
        /// <param name="ddmmyyy"></param>
        /// <returns></returns>
        protected String DateFormat(String ddmmyyy)
        {
            if (ddmmyyy.Substring(4, 1) != "0") // two digit month number
            {
                return ddmmyyy.Substring(4, 2) + "/" + ddmmyyy.Substring(6) + "/" + ddmmyyy.Substring(0, 4);
            }
            else //one digit month number
            {
                return ddmmyyy.Substring(5, 1) + "/" + ddmmyyy.Substring(6) + "/" + ddmmyyy.Substring(0, 4);
            }
        }

        /// <summary>
        /// reads in the input_to_ADAR and gets the first and last date of the data
        /// </summary>
        /// <param name="startEnd"></param>
        /// <returns></returns>
        protected String GetConfigFirstDataPt(Boolean startEnd) //true = startDate Returned, False = endDate Returned
        {
            string path = @"C:\Halo\ADAR\inputs and outputs\input_to_ADAR.csv";
            String outputLines = "";
            var startDate = "";
            var endDate = "";

            StreamReader sr = File.OpenText(path);
            string s;
            while ((s = sr.ReadLine()) != null)
            {
                outputLines += s + "\n";
            }
            sr.Close();

            endDate += outputLines;

            String[] inputArray = Regex.Split(outputLines, "\n");
            for (var i = 0; i < inputArray.Length; i++)
            {
                var inputRow = Regex.Split(inputArray[i], ",");
                if (i == 1)
                {
                    startDate = inputRow[2];
                };
                if (i == inputArray.Length-2)
                {
                    endDate = inputRow[2];
                }
            }

            if (startEnd)
            {
                return startDate;
            }
            else
            {
                return endDate;//String.Join("\n ", inputArray[inputArray.Length-2].ToArray());//[inputArray.Length -1]; String.Join("\n ", inputArray.ToArray())
            }
        }

        /// <summary>
        /// overwrites the config
        /// </summary>
        private void writeConfig(List<List<string>> newConfig)
        {
            var filePath = @"C:\Halo\ADAR\inputs and outputs\config.csv";
            String configLine = "";
            StringBuilder sb = new StringBuilder();
            
            for (var i = 0; i < newConfig.Count(); i++)
            {
                configLine += String.Join(",", newConfig[i]) + "\n";
            }
            sb.Append(configLine);
            File.WriteAllText(filePath, sb.ToString());
        }

        #endregion

        #endregion
    }
}