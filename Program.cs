using System;
using System.IO;
using System.Net.Http;
using System.Text;

namespace ConsoleApplication1
{
    class Program
    {
        static HttpRequestMessage CreateRequest(string url)
        {
            var id = "user_id";
            var pass = "user_pass";

            // リクエストの生成
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };

            // Basic認証ヘッダを付与する
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", id, pass))));
            return request;
        }

        static void Main(string[] args)
        {
            // 情報一覧取得
            var url = "https://GET_LIST_URL";
            var request = CreateRequest(url);

            // リクエストの送信
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.SendAsync(request);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                Console.WriteLine(res);
            }

            //TODO 一覧情報のJSONをパースしてtarget_idを引っ張り出す処理を入れる
            //仮で入れておく
            var target_id = 20180803100000;

            // 取得したデータに合わせてAPIでファイルをDLする
            var file_url = "https://GET_INFO_URL?target_id=" + target_id;
            DownloadFileAsync(file_url, @"C:\temp\"+target_id + ".zip");
        }

        static async void DownloadFileAsync(string url, string outputPath)
        {
            var request = CreateRequest(url);
            var client = new HttpClient();

            HttpResponseMessage res;
            // リクエストの送信
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.SendAsync(request);
                res = response.Result;
            }

            // ファイルのDL
            using (var fileStream = File.Create(outputPath))
            using (var httpStream = await res.Content.ReadAsStreamAsync())
            {
                httpStream.CopyTo(fileStream);
                fileStream.Flush();
            }

        }
    }
}