using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Net.Sockets;
using System.Diagnostics;

public class PostPlayerMBTI : MonoBehaviour
{
    public TMPro.TMP_Dropdown dropdown;
    private string m_Text;
    public QueueResponseDto res;

    public string pid;

    private string host = "192.168.137.1";
    private string port = "8080";

    private static readonly HttpClient client = new HttpClient();

    // TCP client
    public static TcpClient gsClient;
    public static string gsHost;
    public static int gsPort;
    public static string gsPid = "0";
    public const string BZREADY = "BZREADY";
    public const string BZUPDATE = "BZUPDATE";
    public const string MAGIC = "MAGIC";

    // Get data from gameserver
    private void GSReady()
    {
        Stopwatch s = new Stopwatch();
        while (s.Elapsed < TimeSpan.FromSeconds(10))
        {
            byte[] buf = System.Text.Encoding.UTF8.GetBytes(BZREADY + gsPid);
            NetworkStream stream = gsClient.GetStream();
            stream.Write(buf, 0, buf.Length);

            buf = new byte[32];
            string responseData = string.Empty;
            int bytes = stream.Read(buf, 0, buf.Length);
            responseData = System.Text.Encoding.UTF8.GetString(buf, 0, bytes);
            string[] response = responseData.Split(',');
            gsPid = response[0];
            int isReady = int.Parse(response[1]);
            if (isReady == 1)
            {
                s.Stop();
                return;
            }
        }
        s.Stop();
        pid = "-1";
    }

    // Connect to TCP gameserver
    private bool GSConnect()
    {
        bool res = false;
        UnityEngine.Debug.Log($"Trying to connect to gameserver {gsHost}:{gsPort}");
        try
        {
            gsClient = new TcpClient(gsHost, gsPort);
            UnityEngine.Debug.Log("Gameserver connection success");
            res = true;
        }
        catch (SocketException e)
        {
            UnityEngine.Debug.LogError($"Could not connect to {gsHost}:{gsPort}: {e.Message}");
        }
        return res;
    }

    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public async void OnMatchClick()
    {
        
        m_Text = dropdown.captionText.GetParsedText();
        SceneManager.LoadScene("WaitingRoom");
        try {
            UnityEngine.Debug.Log("Sending Join Request");
            string response = await SendJoinRequest();
            res = QueueResponseDto.CreateFromJSON(response);
            UnityEngine.Debug.Log("Room id: " + res.gameroom_index);
            // set gsHost and gsPort
            gsHost = res.server_host;
            gsPort = res.server_port;
        } catch (Exception e) {
            UnityEngine.Debug.Log(e.Message);
            SceneManager.LoadScene("Menu");
            return;
        }

        UnityEngine.Debug.Log("Sending Ready Request");
        GSConnect();
        GSReady();

        if (pid.Equals("-1"))
        {
            UnityEngine.Debug.Log("Invalid pid. Returning to menu...");
            SceneManager.LoadScene("Menu");
            return;
        }
        UnityEngine.Debug.Log("In 3 seconds, start level 1...");
        StartCoroutine(GameStart());
        SceneManager.LoadScene("Level1");
    }

    IEnumerator GameStart()
    {
        yield return new WaitForSecondsRealtime(3f);
        UnityEngine.Debug.Log("Starting level 1");
        SceneManager.LoadScene("Level1");
    }
    private async Task<string> SendJoinRequest()
    {
        var values = new Dictionary<string, string>
        {
            {"mbti", m_Text }
        };
        var content = new FormUrlEncodedContent(values);
        var response = await client.PostAsync(System.String.Format("http://{0}:{1}/api/join", host, port), content);
        var responseString = await response.Content.ReadAsStringAsync();
        return responseString;
    }

    private async Task<string> SendReadyRequest()
    {
        var values = new Dictionary<string, string>
        {
            {"roomId", res.gameroom_index.ToString() }
        };
        var content = new FormUrlEncodedContent(values);
        var response = await client.PostAsync(System.String.Format("http://{0}:{1}/game/ready", res.server_host.ToString(), res.server_port.ToString()), content);
        var responseString = await response.Content.ReadAsStringAsync();
        return responseString;
    }

    // Clean up
    private void OnApplicationQuit()
    {
        gsClient?.Close();
        UnityEngine.Debug.Log("Closed gameserver connection");
    }
}
