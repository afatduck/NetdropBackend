using Limilabs.FTP.Client;
using Microsoft.AspNetCore.Mvc;
using Netdrop.Interfaces;
using Netdrop.Interfaces.Requests;
using Netdrop.Interfaces.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text.Json;
using System.Threading.Tasks;

namespace Netdrop.Controllers
{
    public partial class NetdropController : ControllerBase
    {
        [HttpPost("listdir")]
        public IActionResult PostListdir([FromBody] BaseFtpRequest data)
        {

            try
            {
                using (Ftp client = new Ftp())
                {

                    Task task = Task.Run(() =>
                    {

                        client.ServerCertificateValidate += ValidateCertificate;

                        try
                        {
                            if (data.Secure) { client.ConnectSSL(data.Host, data.Port); }
                            else { client.Connect(data.Host, data.Port); }
                            client.AuthTLS();
                        }
                        catch (Exception)
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
            catch (Exception ex)
            {
                return Ok(new ListDirResponse() { 
                    Result = false,
                    Errors = new List<string>() { ex.Message == "Please connect first." ? "Can't connect." : ex.Message }
                });
            }
        }

    }
}
