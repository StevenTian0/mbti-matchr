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

public class PostPlayerMBTI : MonoBehaviour
{
    public GameObject animation;
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

    public void Awake()
    {
        Instantiate(animation, new Vector3(0, 0, 0), Quaternion.identity);
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(GameObject.Find("Animation(Clone)"));
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
            Debug.Log("Sending Join Request");
            string response = await SendJoinRequest();
            res = QueueResponseDto.CreateFromJSON(response);
            Debug.Log("Room id: " + res.gameroom_index);
            Debug.Log("Sending Ready Request");
            pid = await SendReadyRequest();
        } catch (Exception e) {
            Debug.Log(e.Message);
            SceneManager.LoadScene("Menu");
            return;
        }
        if (pid.Equals("-1"))
        {
            Debug.Log("Invalid pid. Returning to menu...");
            SceneManager.LoadScene("Menu");
            return;
        }
        Debug.Log("In 5 seconds, start level 1...");
        StartCoroutine(GameStart());
    }

    IEnumerator GameStart()
    {
        yield return new WaitForSecondsRealtime(3f);
        Debug.Log("Starting level 1");
        Destroy(GameObject.Find("Animation(Clone)"));
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
}
