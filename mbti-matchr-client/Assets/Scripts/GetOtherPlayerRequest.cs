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
    GameObject otherPlayer;
    PlayerController playerController;
    PostPlayerMBTI postPlayerMBTI;
    public static readonly HttpClient client = new HttpClient();
    PlayerActionDto playerActionDtoReponse;

    private void Start()
    {
    }

    public async void FixedUpdate()
    {
        string playerActionJsonResponse = await SendPlayerActionRequest();
        playerActionDtoReponse = PlayerActionDto.CreateFromJSON(playerActionJsonResponse);
        otherPlayer.transform.position = new Vector3(playerActionDtoReponse.positionX, playerActionDtoReponse.positionY,
            playerActionDtoReponse.positionZ);
    }

    private async Task<string> SendPlayerActionRequest()
    {
        PlayerActionDto playerActionDto = new PlayerActionDto
        (
            playerController.positionX,
            playerController.positionY,
            playerController.positionZ,
            (int)playerController.state
        );

        string playerActionDtoJson = playerActionDto.SaveToString();

        var values = new Dictionary<string, string>
        {
            {"roomId", postPlayerMBTI.res.gameroom_index.ToString()},
            {"pid", postPlayerMBTI.pid},
            {"data", playerActionDtoJson}
        };

        var content = new FormUrlEncodedContent(values);
        var response = await client.PostAsync(System.String.Format("http://{0}:{1}/game/update", postPlayerMBTI.res.server_host, postPlayerMBTI.res.server_port),content);
        var responseString = await response.Content.ReadAsStringAsync();
        return responseString;
    }
}
