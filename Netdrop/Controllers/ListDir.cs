using FluentFTP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Netdrop.Interfaces;
using Netdrop.Interfaces.Requests;
using Netdrop.Interfaces.Responses;
using Netdrop.Models;
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
        public async Task<IActionResult> PostListdir([FromBody] ListDirRequest data)
        {

            try
            {
                    FtpClient client = GetFtpClient(data, data.New, data.Connection);
                    if (!client.IsConnected) client.Noop();

                    FtpListItem[] items = await client.GetListingAsync(data.Path);
                    List<DirList> dirList = new List<DirList>();

                    foreach (FtpListItem item in items)
                    {
                        if (item.Type == FtpFileSystemObjectType.Link || item.SubType == FtpFileSystemObjectSubType.SelfDirectory || item.SubType == FtpFileSystemObjectSubType.ParentDirectory) { continue; }
                        DirList toAdd = new DirList() {
                            Name = item.Name,
                            Modify = item.RawModified.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds,
                            Size = item.Size.ToString(),
                            Type = item.Type == FtpFileSystemObjectType.Directory ? "dir" : "file",
                            Mime = item.Type == FtpFileSystemObjectType.Directory ? "directory" : MimeMapping.MimeUtility.GetMimeMapping(item.Name)
                        };
                        dirList.Add(toAdd);
                    }
                if (!data.Save) client.Dispose();

                return Ok(new ListDirResponse() {
                        Result = true,
                        DirList = dirList,
                        Connection = (string)HttpContext.Items[0]
                    });
            }
            catch (Exception ex)
            {
                return Ok(new ListDirResponse() { 
                    Result = false,
                    Errors = new List<string>() { ex.InnerException != null ? ex.InnerException.Message : ex.Message }
                });
            }
        }

    }
}
