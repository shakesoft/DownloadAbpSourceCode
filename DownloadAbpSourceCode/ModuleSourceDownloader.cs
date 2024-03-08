#region Header

// + ++- --+ + -+ + +-+-+- ++++
// 
// Radio Cabs of Wollongong Co-Operative Society Ltd
// 
// Author: Chuyang Hu
// 
// File Name: ModuleSourceDownloader.cs
// 
// Modification Time: 2:50
// Modification Date: 28/12/2020
// 
// Create Time: 2:50
// Create Date: 28/12/2020
// 
// --+ -+ --- + +-+-+- -+-+

#endregion

using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace DownloadSource
{
    public static class ModuleSourceDownloader
    {
        public static void InitDirectories()
        {
            if (!Directory.Exists(".\\Pro"))
            {
                Directory.CreateDirectory(".\\Pro");
            }

            if (!Directory.Exists(".\\Free"))
            {
                Directory.CreateDirectory(".\\Free");
            }
        }

        public static async Task Download(string name, string version, bool isPro = false, string token = null)
        {
            var path = ".\\" + (isPro ? "Pro\\" : "Free\\") + name + "-" + version + ".zip";

            if (File.Exists(path))
            {
                Console.WriteLine("文件已存在，跳过下载。");
                return;
            }
 
            var bytes = await DownloadSourceCodeContentAsync(new SourceCodeDownloadInputDto()
            {
                Name = name,
                Version = version,
                Type = "module",
                TemplateSource = null,
                IncludePreReleases = false
            }, isPro, token);

            if(bytes==null)
            {
                Console.WriteLine($"模块{name}下截失败，请使用abp get-source {name}手动下载源码");
                return;
            }
            
            await using var bw =
                new BinaryWriter(new FileStream(path, FileMode.Create));
            bw.Write(bytes);
            bw.Flush();
            bw.Close();
            await bw.DisposeAsync();
        }

        private static async Task<byte[]> DownloadSourceCodeContentAsync(SourceCodeDownloadInputDto input,
            bool isPro = false, string token = null)
        {
            var url = "https://abp.io/api/download/" + input.Type + "/";
            try
            {
                using var client = new HttpClient();

                if (isPro)
                {
                    client.SetBearerToken(token);
                }

                var responseMessage = await client
                    .PostAsync(url,
                        new StringContent(input.ToJson(), Encoding.UTF8, "application/json"));

                if (responseMessage.IsSuccessStatusCode)
                {
                    return await responseMessage.Content.ReadAsByteArrayAsync();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured while downloading source-code from {0} : {1}", url, ex.Message);
                throw;
            }
        }
    }
}