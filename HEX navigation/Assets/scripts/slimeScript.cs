using UnityEngine;
using Photon.Pun;

public class slimeScript : MonoBehaviour
{
    public int crTurn = 0;

    public GameObject user;
    GameObject pl1, pl2, pl3;       
   

    void Start()
    {
        crTurn = (int)PhotonNetwork.CurrentRoom.CustomProperties["tNo"]; 

        pl1 = GameObject.FindGameObjectWithTag("player1");
        pl2 = GameObject.FindGameObjectWithTag("player2");
        pl3 = GameObject.FindGameObjectWithTag("player3");        

        if (pl1.GetComponent<plControl>().enabled) { user = pl1; }
        else if (pl2.GetComponent<p2control>().enabled) { user = pl2; }
        else if (pl3.GetComponent<p3control>().enabled) { user = pl3; }

        //print("crTurn=" +crTurn);
    }

    
    void Update()
    {   
        if (crTurn+2 == (int)PhotonNetwork.CurrentRoom.CustomProperties["tNo"])
        {
            Destroy(gameObject);
        }

    }
}
