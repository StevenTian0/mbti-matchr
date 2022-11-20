using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerActionDto
{
    public float positionX;
    public float positionY;
    public float positionZ;
    public int state;

    public PlayerActionDto(float positionX, float positionY, float positionZ, int state)
    {
        this.positionX = positionX;
        this.positionY = positionY;
        this.positionZ = positionZ;
        this.state = state;
    }

    public static PlayerActionDto CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PlayerActionDto>(jsonString);
    }

    public string SaveToString()
    {
        string temp = JsonUtility.ToJson(this);
        temp.Replace("{", "%7B");
        temp.Replace("}", "%7D");
        return temp;
    }

}
