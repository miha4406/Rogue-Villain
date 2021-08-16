
using UnityEngine;

public class slimeScript : MonoBehaviour
{
    public int crTurn = 0;

    public GameObject user;
    GameObject pl1, pl2, pl3;       
   

    void Start()
    {
        crTurn = GameObject.Find("GameLogic").GetComponent<turnEnd>().turnNo;

        pl1 = GameObject.Find("player1");
        pl2 = GameObject.Find("player2");
        pl3 = GameObject.Find("player3");        

        if (pl1.GetComponent<plControl>().enabled) { user = pl1; }
        else if (pl2.GetComponent<p2control>().enabled) { user = pl2; }
        else if (pl3.GetComponent<p3control>().enabled) { user = pl3; }
    }

    
    void Update()
    {   
        if (crTurn+2 == GameObject.Find("GameLogic").GetComponent<turnEnd>().turnNo)
        {
            Destroy(gameObject);
        }

    }
}
