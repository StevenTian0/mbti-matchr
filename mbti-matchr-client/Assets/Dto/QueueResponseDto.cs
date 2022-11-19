using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class QueueResponseDto
{
    public string uuid;
    public string matched_mbti;
    public string matched_uuid;
    public string server_host;
    public int server_port;
    public int gameroom_index;

    public QueueResponseDto(string uuid, string matched_mbti, string matched_uuid, string server_host, int server_port, int gameroom_index)
    {
        this.uuid = uuid;
        this.matched_mbti = matched_mbti;
        this.matched_uuid = matched_uuid;
        this.server_host = server_host;
        this.server_port = server_port;
        this.gameroom_index = gameroom_index;
    }

    public static QueueResponseDto CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<QueueResponseDto>(jsonString);
    }
}
