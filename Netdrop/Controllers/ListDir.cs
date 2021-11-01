using Limilabs.FTP.Client;
using Microsoft.AspNetCore.Mvc;
using Netdrop.Interfaces.Requests;
using Netdrop.Interfaces.Responses;
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
        public async Task<IActionResult> PostListdir([FromBody] ListDirRequest data)
        {

            try
            {
                using (Ftp client = new Ftp())
                {
                    client.SendTimeout = TimeSpan.FromSeconds(10);

                    Task task = Task.Run(() =>
                    {
                        
                        try
                        {
                            if (data.Secure) { client.ConnectSSL(data.Host); }
                            else { client.Connect(data.Host); }
                        }
                        catch (FtpException)
                        {}
                    });
                    if (!task.Wait(TimeSpan.FromSeconds(10)))
                    {
                        return Ok(new ListDirResponse()
                        {
                            Result = false,
                            Errors = new List<string>() { "Timed out." }
                        });
                    }

                    client.Login(data.Username, data.Password);

                    List<FtpItem> items = client.GetList(data.Path);
                    List<DirList> dirList = new List<DirList>();

                    foreach (FtpItem item in items)
                    {
                        if (item.IsCurrentFolder || item.IsParentFolder) { continue; }
                        DirList toAdd = new DirList() {
                            Name = item.Name,
                            Modify = item.ModifyDate.ToString("MM/dd/yyyy H:mm"),
                            Size = item.Size.ToString(),
                            Type = item.IsFolder ? "dir" : "file"
                        };
                        dirList.Add(toAdd);
                    }

                    return Ok(new ListDirResponse() {
                        Result = true,
                        DirList = dirList
                    });
                }
            }
            catch (FtpException ex)
            {
                return Ok(new ListDirResponse() { 
                    Result = false,
                    Errors = new List<string>() { ex.Message == "Please connect first." ? "Can't connect." : ex.Message }
                });
            }
        }
    }
}
