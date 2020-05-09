using System;
using System.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

namespace NotifyTaskCompletion
{
    public class GetDataAsync
    {
        /// <summary>
        /// Метод загрузки контента.
        /// </summary>
        /// <param name="client">Экземпляр HttpClient</param>
        /// <param name="uri">Ссылка для загрузки</param>
        /// <param name="times">Время в сек. потраченных на попытки.По умолчанию равно 3.</param>
        /// <returns>Возвращяет downloadTask если попытка удачная,timeOutTask если нет.</returns>
        async Task<string> DownloadStringWithTimeOutAsync(HttpClient client , string uri,int times = 3)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(times));
            Task<string> downloadTask = client.GetStringAsync(uri);
            Task timeOutTask = Task.Delay(Timeout.InfiniteTimeSpan, cts.Token);
            
            Task completedTask = await Task.WhenAny(downloadTask, timeOutTask); 
            
            if (completedTask == timeOutTask)
                return null;
            
            return await downloadTask;
        }

        /// <summary>
        /// Метод возпращает Json объект.
        /// </summary>
        /// <param name="client">Экземпляр HttpClient</param>
        /// <param name="uri">Ссылка для загрузки</param>
        /// <param name="times">Время в сек. потраченных на попытки.По умолчанию равно 3.</param>
        /// <returns>Возвращяет downloadTask если попытка удачная,timeOutTask если нет.</returns>
        async Task<JsonObject> GetJsonObjectFromStringWithTimeOutAsync(HttpClient client , string uri,int times = 3)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(times));
            Task<HttpResponseMessage> downloadTask =  client.GetAsync(uri);
            Task timeOutTask = Task.Delay(Timeout.InfiniteTimeSpan, cts.Token);

            Task completedTask = await Task.WhenAny(downloadTask, timeOutTask);

            string content = await downloadTask.Result.Content.ReadAsStringAsync();

            return (JsonObject) await Task.Run(() => JsonObject.Parse(content));
        }
    }
}