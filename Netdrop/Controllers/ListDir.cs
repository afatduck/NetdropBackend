using FluentFTP;
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
        public async Task<IActionResult> PostListdir([FromBody] BaseFtpRequest data)
        {

            try
            {
                    FtpClient client = GetFtpClient(data);
                    await client.ConnectAsync();

                    FtpListItem[] items = await client.GetListingAsync(data.Path);
                    List<DirList> dirList = new List<DirList>();

                    foreach (FtpListItem item in items)
                    {
                        if (item.Type == FtpFileSystemObjectType.Link || item.SubType == FtpFileSystemObjectSubType.SelfDirectory || item.SubType == FtpFileSystemObjectSubType.ParentDirectory) { continue; }
                        DirList toAdd = new DirList() {
                            Name = item.Name,
                            Modify = item.Modified.ToString("MM/dd/yyyy H:mm"),
                            Size = item.Size.ToString(),
                            Type = item.Type == FtpFileSystemObjectType.Directory ? "dir" : "file",
                            Mime = MimeMapping.MimeUtility.GetMimeMapping(item.Name)
                        };
                        dirList.Add(toAdd);
                    }

                    client.Dispose();

                     return Ok(new ListDirResponse() {
                        Result = true,
                        DirList = dirList
                    });
            }
            catch (Exception ex)
            {
                return Ok(new ListDirResponse() { 
                    Result = false,
                    Errors = new List<string>() { ex.Message }
                });
            }
        }

    }
}
