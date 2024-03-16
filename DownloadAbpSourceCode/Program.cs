using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;

namespace DownloadSource
{
    class Program
    {
        private const string ModuleListUrl = "https://abp.io/api/download/modules";

        private static List<string> listErrorModule = new List<string>();

        static async Task Main(string[] args)
        {
            Console.WriteLine("Abp.io 批量源码下载器");
            ModuleSourceDownloader.InitDirectories();
            var list = await GetModulesList();
            var proCount = list.Count(m => m.IsPro);
            var freeCount = list.Count(m => !m.IsPro);
            Console.WriteLine("成功获取 " + list.Count + " 个模块信息，" + freeCount + " 个免费模块，" + proCount + " 个收费模块");
            Console.WriteLine("开始下载模块...");

            string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string abpCliPath = Path.Combine(userProfilePath, ".abp", "cli");
            string tokenFilename = Path.Combine(abpCliPath, "access-token.bin");
            string token = string.Empty;

            //读取本地目录token
            if (File.Exists(tokenFilename))
            {
                Console.WriteLine("尝试读取收费模块下载令牌");
                using var sr = new StreamReader(tokenFilename);
                token = await sr.ReadToEndAsync();
                Console.WriteLine("已成功读取Abp用户令牌。");
            }

            Console.WriteLine("请输入Abp版本号，例如：8.0.4");
            var version = Console.ReadLine();

            //下载免费模块
            await ExecuteDownLoad(list.Where(m=>!m.IsPro).ToList(),version,token);
            
            //下载收费模块
            await ExecuteDownLoad(list.Where(m=>m.IsPro).ToList(),version,token);

            Console.WriteLine("源码下载完成");
        }

        private static async Task ExecuteDownLoad(List<Module> list,string version,string token)
        {
            foreach (var module in list)
            {
                Console.WriteLine((module.IsPro ? "[收费模块]" : "[免费模块]") + " " + module.Name);

                if (string.IsNullOrWhiteSpace(module.Namespace))
                {
                    Console.WriteLine("模块不正确，跳过。");
                    continue;
                }

                Console.WriteLine("正在下载源码...");
                try
                {
                    await ModuleSourceDownloader.Download(module.Name, version, module.IsPro, token);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private static async Task<List<Module>> GetModulesList()
        {
            Console.WriteLine("正在获取可下载模块清单...");

            const string moduleListFilename = "module-list.json";

            var content = string.Empty;

            if (File.Exists(moduleListFilename))
            {
                Console.WriteLine("找到本地下载缓存，正在尝试读取清单。");
                using var sr = new StreamReader(moduleListFilename);
                content = await sr.ReadToEndAsync();
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                Console.WriteLine("本地读取失败，尝试联网下载清单。");

                var client = new HttpClient();
                using var responseMessage = await client.GetAsync(ModuleListUrl);

                if (responseMessage.IsSuccessStatusCode)
                {
                    content = await responseMessage.Content.ReadAsStringAsync();

                    Console.WriteLine("将清单保存到本地，降低联网请求数。");
                    await using var sw = new StreamWriter(new FileStream(moduleListFilename, FileMode.Create));
                    await sw.WriteAsync(content);
                    await sw.FlushAsync();
                    sw.Close();
                }
                else
                {
                    throw new Exception("联网下载清单失败失败，错误代码：" + responseMessage.StatusCode);
                }
            }

            var list = JsonConvert.DeserializeObject<List<Module>>(content);
            return list;
        }
    }
}