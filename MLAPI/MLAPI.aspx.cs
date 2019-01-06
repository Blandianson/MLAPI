﻿using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.Data.SqlClient;

namespace HaloBI.Prism.Plugin
{
    public partial class MLAPI : System.Web.UI.Page
    {
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
            var w = Newtonsoft.Json.Linq.JObject.FromObject(plugin);
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

            // Test add timestamp
            //context["plugin"]["config"]["time"] = System.DateTime.Now.ToShortTimeString() + "</></>";

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
            // get Header info from config
			//uiHeader.Text = context["plugin"]["config"]["headerText"].ToString();
            var members = GetHierarchyMembersFromContext(context);
            uiSelectedMembers.Text = string.Join(",", members.ToArray());

			// set members list dropdown
			SetMembersList(context, uiMembersList);
			if (members.Count > 0)
			{
				var item = uiMembersList.Items.FindByText(members[0].ToString());

				if (item != null)
				{
					item.Selected = true;
				}
		}

            // now the all important client side hook to update the Prism view
            var viewId = context["view"]["id"].ToString();
            var paneId = context["view"]["paneId"].ToString();
            
            //// this example adds an inline anonymous function to the client side click event 
            //// and prevents the default action of the event (no postback)
            //var clientsideUpdateFunction = string.Format("(function(e) {{ e.preventDefault(); window.parent.halobi.plugin.prismUpdate('{0}','{1}','{2}');}})(event)",
            //        viewId,
            //        paneId,
            //        "" // in iFrame mode context is updated server-side by the plugin
            //);

            // simpler case without preventDefault if you want the asp.net server side event to fire
			//var updateAll = true;
			var clientsideUpdateFunction = string.Format("window.parent.halobi.plugin.prismUpdate('{0}','{1}','{2}', true);",
                    viewId,
                    paneId,
                    ""
			);

            //uiUpdatePrism.Attributes.Add("onclick", clientsideUpdateFunction);

            //SetDebugInfo(context);
        }

        private void SetMembersList(JObject context, DropDownList ddl)
        {
			var server = context["cube"]["server"].ToString();
			var catalog = context["cube"]["catalog"].ToString();
			var cube = context["cube"]["cube"].ToString();

			// get hierarchy details from config
			var h = context["plugin"]["config"]["hierarchy"];
			var hierarchy = h["name"].ToString();
			var level = h["level"].ToString();
			var allMember = h["allMember"].ToString();
 
			// Build the MDX
			var mdx = "WITH "; 
			mdx += String.Format("MEMBER [Time].[Time].[MemberUniqueName] AS '{0}.CurrentMember.UniqueName' ", 
				hierarchy
			);
			mdx += String.Format("MEMBER [Time].[Time].[MemberName] AS '{0}.CurrentMember.Name' ",
				hierarchy
			);
			mdx += String.Format("SET [Rows] AS '{{Descendants({0}, {1})}}' ",
				allMember,
				level
			);
			mdx += "SELECT{[Time].[Time].[MemberUniqueName], [Time].[Time].[MemberName]} ON COLUMNS, ";
			mdx += "{[Rows]} ON ROWS ";
			mdx += String.Format("FROM [{0}]", 
				cube
			);

			var data = new CubeData(server, catalog, cube);
			var dataSet = data.GetData(mdx);

			// expecting only a single dataTable in the set
			var dataTable = dataSet.Tables[0];

            ddl.Items.Clear();

			foreach (DataRow r in dataTable.Rows)
			{
				ddl.Items.Add(new ListItem(
					r["[Time].[Time].[MemberName]"].ToString(),
					r["[Time].[Time].[MemberUniqueName]"].ToString()
				));
			}
        }

        //private void SetDebugInfo(JObject context)
        //{
        //    if (Debug(context))
        //    {
        //        uiContext.Visible = true;
        //        uiContext.Text = context.ToString(Formatting.Indented);
        //    }
        //}

        /// <summary>
        /// Check config debug status
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool Debug(JObject context)
        {
            if (context["plugin"]["config"]["debug"] != null &&
                context["plugin"]["config"]["debug"].ToString().ToLower() == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieve the values in the context object associated with the config key
        /// </summary>
        /// <param name="context"></param>
        /// <param name=configKey"></param>
        /// <returns></returns>
        private List<string> GetHierarchyMembersFromContext(JObject context)
        {
            var requriedHierarchy = context["plugin"]["config"]["hierarchy"]["name"].ToString();
            var hierarchies = context["hierarchies"];
            var list = new List<string>();

            foreach (var h in hierarchies)
            {
                if (h["uniqueName"].ToString() == requriedHierarchy)
                {
                    list = JsonConvert.DeserializeObject<List<string>>(h["memberNames"].ToString());
                }
            }

            if (list == null)
            {
                list = new List<string>();
            }

            return list;
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

        protected void uiMembersList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var memberUniqueName = uiMembersList.SelectedItem.Value;
            var memberName = uiMembersList.SelectedItem.Text;

            // update the context object
            var context = GetContext(_contextId);
            var requiredHierarchy = context["plugin"]["config"]["hierarchy"]["name"].ToString();
            var hierarchies = context["hierarchies"];
            var list = new List<string>();

            foreach (var h in hierarchies)
            {
                if (h["uniqueName"].ToString() == requiredHierarchy)
                {
                    h["memberUniqueNames"][0] = memberUniqueName;
                    h["memberNames"][0] = memberName;
                }
            }

            //SetDebugInfo(context);
            SetContextToSession(context, _contextId);
        }

        //Working code

        protected void request_click(object sender, EventArgs e) {
            using (SqlConnection conn = new SqlConnection())
            {
                //conn.ConnectionString = "Server=DESKTOP-SK1K62B;Database=HaloMessageClient;Trusted_Connection=true";
                conn.ConnectionString = "Server=DESKTOP-SK1K62B;Database=HaloMessageClient;Trusted_Connection=true";
                conn.Open();
                //SqlCommand command = new SqlCommand("SELECT TOP (1) FROM 'Queue.Message' ORDER BY DateCreated DESC", conn);
                // SqlCommand command = new SqlCommand("SELECT * FROM Queue.Message", conn); WORKS!!
                SqlCommand command = new SqlCommand("SELECT TOP 3 * FROM Queue.Message ORDER BY DateCreated DESC", conn);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        outputText.Text += String.Format("{0}", reader[8]);
                    }
                }

                conn.Close();
            }
        }

        protected void rabbitMessaging_click(object sender, EventArgs e)
        {

            Process receiveProcess = receiveCmd();
            Process receiveResponseProcess = receiveResponseCmd();

            ProcessStartInfo cmdStartInfo = new ProcessStartInfo();
            cmdStartInfo.FileName = @"C:\Program Files\Halo\HaloMW\MessageConsole\Halo.ML.MessageConsole.exe";
            cmdStartInfo.RedirectStandardOutput = true;
            cmdStartInfo.RedirectStandardError = true;
            cmdStartInfo.RedirectStandardInput = true;
            cmdStartInfo.UseShellExecute = false;
            cmdStartInfo.CreateNoWindow = false;
            cmdStartInfo.Verb = "runas";
            cmdStartInfo.Arguments = "send -s " + sessionID.Text + " -f " + executionID.Text + " -i " + server.Text + " -d " + staging.Text + " -t  0 -l " + rScript.Text + " -p " + parameters.Text;


            Process cmdProcess = new Process();
            cmdProcess.StartInfo = cmdStartInfo;
            cmdProcess.ErrorDataReceived += cmd_Error;
            cmdProcess.OutputDataReceived += cmd_DataReceived;
            cmdProcess.EnableRaisingEvents = true;
            cmdProcess.Start();
            cmdProcess.BeginOutputReadLine();
            cmdProcess.BeginErrorReadLine();

            cmdProcess.StandardInput.WriteLine("exit");                  //Execute exit. 
            cmdProcess.WaitForExit();

            
            receiveProcess.Close();

            receiveResponseProcess.Close();

            return;
        }

        protected Process receiveCmd()
        {
            ProcessStartInfo cmdStartInfoRec = new ProcessStartInfo();
            cmdStartInfoRec.FileName = @"C:\Program Files\Halo\HaloMW\MessageConsole\Halo.ML.MessageConsole.exe";
            cmdStartInfoRec.RedirectStandardOutput = true;
            cmdStartInfoRec.RedirectStandardError = true;
            cmdStartInfoRec.RedirectStandardInput = true;
            cmdStartInfoRec.UseShellExecute = false;
            cmdStartInfoRec.CreateNoWindow = false;
            cmdStartInfoRec.Verb = "runas";
            cmdStartInfoRec.Arguments = "receive";

            Process cmdProcessRec = new Process();
            cmdProcessRec.StartInfo = cmdStartInfoRec;
            cmdProcessRec.ErrorDataReceived += cmd_Error;
            cmdProcessRec.OutputDataReceived += cmd_DataReceived;
            cmdProcessRec.EnableRaisingEvents = true;
            cmdProcessRec.Start();
            cmdProcessRec.BeginOutputReadLine();
            cmdProcessRec.BeginErrorReadLine();
            
            cmdProcessRec.StandardInput.Flush();

            return cmdProcessRec;
        }

        protected Process receiveResponseCmd()
        {
            ProcessStartInfo cmdStartInfoRecResp = new ProcessStartInfo();
            cmdStartInfoRecResp.FileName = @"C:\Program Files\Halo\HaloMW\MessageConsole\Halo.ML.MessageConsole.exe";
            cmdStartInfoRecResp.RedirectStandardOutput = true;
            cmdStartInfoRecResp.RedirectStandardError = true;
            cmdStartInfoRecResp.RedirectStandardInput = true;
            cmdStartInfoRecResp.UseShellExecute = false;
            cmdStartInfoRecResp.CreateNoWindow = false;
            cmdStartInfoRecResp.Verb = "runas";
            cmdStartInfoRecResp.Arguments = "receiveresponse";
            
            Process cmdProcessResp = new Process();
            cmdProcessResp.StartInfo = cmdStartInfoRecResp;
            cmdProcessResp.ErrorDataReceived += cmd_Error;
            cmdProcessResp.OutputDataReceived += cmd_DataReceived;
            cmdProcessResp.EnableRaisingEvents = true;
            cmdProcessResp.Start();
            cmdProcessResp.BeginOutputReadLine();
            cmdProcessResp.BeginErrorReadLine();
            
            cmdProcessResp.StandardInput.Flush();

            return cmdProcessResp;
        }

        protected void cmd_DataReceived(object sender, DataReceivedEventArgs e)
        {
            postData.Text += "Data: " + (e.Data) + "\n\n";
        }

        protected void cmd_Error(object sender, DataReceivedEventArgs e)
        {
            postData.Text += "Error: " + (e.Data) + "\n\n";
        }

        //End of Working Code

        protected void uiUpdatePrism_Click(object sender, EventArgs e)
        {
            //uiSelectedMembers.Text = uiMembersList.SelectedItem.Text;
        }
    }
}