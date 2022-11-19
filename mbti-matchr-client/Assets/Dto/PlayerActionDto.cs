using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerActionDto
{
    public double positionX;
    public double positionY;
    public double positionZ;
    public int state;

    public PlayerActionDto(double positionX, double positionY, double positionZ, int state)
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
        return JsonUtility.ToJson(this);
    }

}
