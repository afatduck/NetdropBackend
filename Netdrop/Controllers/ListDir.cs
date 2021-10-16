using Limilabs.FTP.Client;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Netdrop.Controllers
{
    public partial class NetdropController : ControllerBase
    {
        [HttpPost("listdir")]
        public string PostListdir([FromBody] Dictionary<string, string> data)
        {

            try
            {
                using (Ftp client = new Ftp())
                {
                    client.Connect(data["host"]);
                    client.Login(data["user"], data["pword"]);

                    List<FtpItem> items = client.GetList(data.ContainsKey("path") ? data["path"] : "");
                    List<Dictionary<string, string>> ordered = new List<Dictionary<string, string>>();

                    foreach (FtpItem item in items)
                    {
                        if (item.IsCurrentFolder || item.IsParentFolder) { continue; }
                        Dictionary<string, string> toAdd = new Dictionary<string, string>();
                        toAdd.Add("name", item.Name);
                        toAdd.Add("modify", item.ModifyDate.ToString("MM/dd/yyyy H:mm"));
                        toAdd.Add("size", item.Size.ToString());
                        toAdd.Add("type", item.IsFolder ? "dir" : "file");
                        ordered.Add(toAdd);
                    }

                    return JsonSerializer.Serialize(ordered);
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(ex.Message);
            }
        }
    }
}
