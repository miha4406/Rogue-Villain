using UnityEngine;
using Photon.Pun;

public class slimeScript : MonoBehaviour
{
    public int crTurn = 0;

    public GameObject user;
    GameObject pl1, pl2, pl3;
    public int userID = 99;
   

    void Start()
    {        
       // print("userID="+userID);

        crTurn = (int)PhotonNetwork.CurrentRoom.CustomProperties["tNo"];

        pl1 = map.mapS.pl1;  pl2 = map.mapS.pl2;  pl3 = map.mapS.pl3;

        if (pl1.GetComponent<plControl>().enabled) { user = pl1; }
        else if (pl2.GetComponent<p2control>().enabled) { user = pl2; }
        else if (pl3.GetComponent<p3control>().enabled) { user = pl3; }

        if (PhotonNetwork.LocalPlayer.ActorNumber != userID)
        {
            gameObject.transform.Find("itemCanvas/Image").gameObject.SetActive(false);
        }
    }

    
    void Update()
    {
        //if (crTurn == (int)PhotonNetwork.CurrentRoom.CustomProperties["tNo"])
        //{
        //    if (gameObject.transform.Find("itemCanvas/Image").gameObject.active == true)
        //    {                
        //    }            
        //}

        if (crTurn+1 == (int)PhotonNetwork.CurrentRoom.CustomProperties["tNo"]) 
        {
            if (gameObject.transform.Find("slime").gameObject.active == false)  //once
            {
                gameObject.transform.Find("itemCanvas/Image").gameObject.SetActive(false);
                gameObject.transform.Find("slime").gameObject.SetActive(true);

                map.mapS.GetComponent<AudioSource>().PlayOneShot(map.mapS.itemClips[2]);
            }            
        }

        if (crTurn+2 == (int)PhotonNetwork.CurrentRoom.CustomProperties["tNo"])
        {
            Destroy(gameObject);
        }

    }
}
