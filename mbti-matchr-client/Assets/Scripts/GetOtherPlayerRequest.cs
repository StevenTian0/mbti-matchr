using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class GetOtherPlayerRequest : MonoBehaviour
{
    BigPlayerController bigPlayerController;
    PostPlayerMBTI postPlayerMBTI;
    public static readonly HttpClient client = new HttpClient();
    PlayerActionDto playerActionDtoReponse;
    public async void FixedUpdate()
    {
        string playerActionJsonResponse = await SendPlayerActionRequest();
        playerActionDtoReponse = PlayerActionDto.CreateFromJSON(playerActionJsonResponse);

    }

    private async Task<string> SendPlayerActionRequest()
    {
        PlayerActionDto playerActionDto = new PlayerActionDto
        (
            bigPlayerController.positionX,
            bigPlayerController.positionY,
            bigPlayerController.positionZ,
            (int)bigPlayerController.state
        );

        string playerActionDtoJson = playerActionDto.SaveToString();

        var values = new Dictionary<string, string>
        {
            {"data", playerActionDtoJson}
        };
     
        var content = new FormUrlEncodedContent(values);
        var response = await client.PostAsync(System.String.Format("http://{0}:{1}/api/join", postPlayerMBTI.res.server_host, postPlayerMBTI.res.server_port),content);
        var responseString = await response.Content.ReadAsStringAsync();
        return responseString;
    }
}
