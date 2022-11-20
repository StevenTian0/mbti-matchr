using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using UnityEngine.UI;
using TMPro;
using System.Net.Sockets;

public class PostPlayerMBTI : MonoBehaviour
{
    public TMP_InputField inputField;
    private string m_Text;
    public QueueResponseDto res;
    public string pid;

    private string host = "192.168.137.1";
    private string port = "8080";
    private List<string> valdiInput = new List<string>()
    {
       "INTJ", "INTP", "ENTJ", "ENTP","INFJ", "INFP", "ENFJ", "ENFP","ISTJ", "ISFJ", "ESTJ", "ESFJ","ISTP", "ISFP", "ESTP", "ESFP",
       "intj","intp","entj","entp","infj","infp","enfj", "enfp","istj","isfj","estj","esfj","istp","isfp","estp","esfp"
    };
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
        int isReady = 0;
        while (isReady == 0)
        {
            byte[] buf = System.Text.Encoding.UTF8.GetBytes(BZREADY + gsPid);

            GSConnect();
            NetworkStream stream = gsClient.GetStream();
            stream.Write(buf, 0, buf.Length);

            buf = new byte[32];
            string responseData = string.Empty;
            int bytes = stream.Read(buf, 0, buf.Length);
            gsClient?.Close();

            responseData = System.Text.Encoding.UTF8.GetString(buf, 0, bytes);
            string[] response = responseData.Split(',');
            gsPid = response[0];
            isReady = int.Parse(response[1]);
        }
    }

    // Connect to TCP gameserver
    public static bool GSConnect()
    {
        bool res = false;
        //UnityEngine.Debug.Log($"Trying to connect to gameserver {gsHost}:{gsPort}");
        try
        {
            gsClient = new TcpClient(gsHost, gsPort);
            //UnityEngine.Debug.Log("Gameserver connection success");
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
        DontDestroyOnLoad(GameObject.Find("Animation"));
    }

    public async void OnMatchClick()
    {
        m_Text = inputField.text;
        if(!valdiInput.Contains(m_Text) || m_Text.Length != 4)
        {
            inputField.text = "";
            return;
        }
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
        GSReady();

        if (pid.Equals("-1"))
        {
            UnityEngine.Debug.Log("Invalid pid. Returning to menu...");
            SceneManager.LoadScene("Menu");
            return;
        }
        UnityEngine.Debug.Log("In 3 seconds, start level 1...");
        StartCoroutine(GameStart());
    }

    IEnumerator GameStart()
    {
        yield return new WaitForSecondsRealtime(3f);
        Debug.Log("Starting level 1");
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

    // Clean up
    private void OnApplicationQuit()
    {
        byte[] buf = System.Text.Encoding.UTF8.GetBytes(MAGIC);
        GSConnect();
        NetworkStream stream = gsClient.GetStream();
        stream.Write(buf, 0, buf.Length);
        gsClient?.Close();
        UnityEngine.Debug.Log("Reset gameserver");
    }
}
