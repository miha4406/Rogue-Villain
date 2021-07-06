
using UnityEngine;

public class stats : MonoBehaviour
{
    public int movDist; //players mobility

    public int hitCount = 0; //needs for collision

    public int gold = 0; //players gold

    public Vector3 pasSkillHex = Vector3.down; //needs for passive skills

    public GameObject[] actSkillObj = new GameObject[4];  //active skill target(s)

    public int skillCD = 0;  //active skill cooldown

    public int item1 = 0; 
    public int item2 = 0;  //items ID
}

