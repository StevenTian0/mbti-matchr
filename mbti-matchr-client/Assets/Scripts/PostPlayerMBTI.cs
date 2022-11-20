using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

public class PostPlayerMBTI : MonoBehaviour
{
    public TMPro.TMP_Dropdown dropdown;
    private string m_Text;
    public QueueResponseDto res;

    public string pid;

    private string host = "192.168.137.1";
    private string port = "8080";

    private static readonly HttpClient client = new HttpClient();

    private void Start()
    {
        //SceneManager.UnloadScene("WaitingRoom");
        //SceneManager.UnloadScene("InitialLevel");
    }
    public async void OnMatchClick()
    {
        m_Text = dropdown.captionText.GetParsedText();
        SceneManager.LoadScene("WaitingRoom");
        string response = await SendJoinRequest();
        res = QueueResponseDto.CreateFromJSON(response);
        pid = await SendReadyRequest();
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
        var response = await client.PostAsync(System.String.Format("http://{0}:{1}/game/ready", res.server_host, res.server_port), content);
        var responseString = await response.Content.ReadAsStringAsync();
        return responseString;
    }
}
