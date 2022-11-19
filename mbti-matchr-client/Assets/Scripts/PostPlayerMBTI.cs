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
        Debug.Log(res.uuid + ", " + res.matched_uuid + ", " + res.matched_mbti + ", " + res.server_host + ", " + res.server_port);
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
}
