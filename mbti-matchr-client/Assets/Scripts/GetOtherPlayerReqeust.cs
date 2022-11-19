using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GetOtherPlayerReqeust : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MakeRequests());
    }


    private IEnumerator MakeRequests()
    {
        var dataToPost = new PostData() { state = 0, positionX = 0, positionY = 0, positionZ = 0 };
        var postRequest = CreateRequest("https://reqbin.com/echo/post/json", RequestType.POST, dataToPost);
        yield return postRequest.SendWebRequest();
        var deserializedPostData = JsonUtility.FromJson<PostResult>(postRequest.downloadHandler.text);
    }

    private UnityWebRequest CreateRequest(string path, RequestType type = RequestType.GET, object data = null)
    {
        var request = new UnityWebRequest(path, type.ToString());

        if (data != null)
        {
            var bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        }

        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        return request;
    }

    [Serializable]
    public class PostData
    {
        public int state;
        public int positionX;
        public int positionY;
        public int positionZ;
    }

    public enum RequestType
    {
        GET = 0,
        POST = 1,
        PUT = 2
    }

    public class PostResult
    {
        public string success { get; set; }
    }


}
