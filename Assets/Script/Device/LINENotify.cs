using System.Collections;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LINENotify : MonoBehaviour
{
    public static void SendMessage(string message, string token)
    {
        var result = sendMessage(message, token);
        Debug.LogWarning(result);
    }

    private static async Task sendMessage(string message, string token)
    {
        using (var client = new HttpClient())
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "message", message },
                });

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var result = await client.PostAsync("https://notify-api.line.me/api/notify", content);
        }
    }
}
