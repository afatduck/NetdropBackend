using FluentFTP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Netdrop.Interfaces;
using Netdrop.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Netdrop.Controllers
{
    public partial class NetdropController : ControllerBase
    {

        private static Dictionary<string, FtpClient> SavedClients = new Dictionary<string, FtpClient>();

        private FtpClient GetFtpClient(BaseFtpRequest data, bool isNew = false, string con = "")
        {
            string clientKey = con != "" ? con : HttpContext.Request.Cookies["connection"];
            if (!string.IsNullOrEmpty(clientKey))
            {
                FtpClient cli = SavedClients.ContainsKey(clientKey) ? SavedClients[clientKey] : null;

                if (cli != null && !cli.IsDisposed && !isNew)
                {

                        HttpContext.Items[0] = clientKey;
                        if (!cli.IsConnected) cli.Connect();
                        return cli;
                }
            }

            FtpClient client;
            if (data.Username + data.Password == string.Empty)
            {
                client = new(data.Host, data.Port, new NetworkCredential("anonymous", "janeDoe@contoso.com"));
            }
            else { client = new(data.Host, data.Port, data.Username, data.Password); }
            client.DataConnectionEncryption = true;
            client.SslProtocols = System.Security.Authentication.SslProtocols.None;
            client.EncryptionMode = data.Secure ? FtpEncryptionMode.Implicit : FtpEncryptionMode.None;
            client.ValidateAnyCertificate = true;
            client.SocketPollInterval = 0;
            client.ConnectTimeout = 10000;
            client.ReadTimeout = 10000;
            client.DataConnectionConnectTimeout = 10000;
            client.DataConnectionReadTimeout = 10000;
            client.DataConnectionType = FtpDataConnectionType.AutoPassive;
            client.RetryAttempts = 5;
            client.UngracefullDisconnection = true;
            client.EnableThreadSafeDataConnections = false;
            client.SocketKeepAlive = false;
            client.Connect();

            if (data.Save)
            {
                clientKey = DateTime.Now.ToString();
                clientKey = EncryptField.Encrypt(clientKey);
                SavedClients[clientKey] = client;
                HttpContext.Items[0] = clientKey;
                Response.Cookies.Append("connection", clientKey, new CookieOptions()
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    Secure = true
                });
            }

            return client;
        }
    }
}
