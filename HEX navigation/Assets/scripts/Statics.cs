using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;


public class Statics : MonoBehaviourPun
{
    public string pref1owner;    
    public string pref2owner;    
    public string pref3owner;

    bool bScene = false;
    bool bStart = false;
    

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);       
    }


    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex==1 && bScene==false)
        {
            NetInstantiate();

            bScene = true;
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus)) { BotInstantiate(); }

        if (bStart == false) { gameStart(); }
    }


    private void NetInstantiate()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1) 
        {      
            if (GameObject.FindGameObjectWithTag("Statics").GetComponent<Statics>().pref1owner == PhotonNetwork.NickName)
            {
                PhotonNetwork.Instantiate("pl1pref", new Vector3(-0.5f, 0, -0.9f), Quaternion.identity);               
            }
            else if (GameObject.FindGameObjectWithTag("Statics").GetComponent<Statics>().pref2owner == PhotonNetwork.NickName)
            {
                PhotonNetwork.Instantiate("pl2pref", new Vector3(-0.5f, 0, 0.9f), Quaternion.identity).transform.Rotate(0,180f,0);                
            }
            else if (GameObject.FindGameObjectWithTag("Statics").GetComponent<Statics>().pref3owner == PhotonNetwork.NickName)
            {
                PhotonNetwork.Instantiate("pl3pref", new Vector3(1f, 0, 0), Quaternion.identity).transform.Rotate(0, -90f, 0);
            }
        }
    }


    private void BotInstantiate()  //adds players for testing 
    {        
        if (GameObject.FindGameObjectWithTag("player1") == null)
        {
            print("player1 was null");
            PhotonNetwork.Instantiate("pl1pref", new Vector3(-0.5f, 0, -0.9f), Quaternion.identity);
        }
        if (GameObject.FindGameObjectWithTag("player2") == null)
         {
            print("player2 was null");
            PhotonNetwork.Instantiate("pl2pref", new Vector3(-0.5f, 0, 0.9f), Quaternion.identity).transform.Rotate(0, 180f, 0);
        }
        if (GameObject.FindGameObjectWithTag("player3") == null)
        {
            print("player3 was null");
            PhotonNetwork.Instantiate("pl3pref", new Vector3(1f, 0, 0), Quaternion.identity).transform.Rotate(0, -90f, 0);
        }               
    }


    private void gameStart()
    {
        if (GameObject.FindGameObjectWithTag("player1") != null 
            && GameObject.FindGameObjectWithTag("player2") != null
            && GameObject.FindGameObjectWithTag("player3") != null) {

            if (PhotonNetwork.IsMasterClient)  //Host adds gold bars
            {
                if (GameObject.FindGameObjectWithTag("gBar1")==null) {
                    map.mapS.gBar1 = PhotonNetwork.Instantiate("goldBar1", new Vector3(0f, 0f, 0f), Quaternion.identity);                    
                }
                if (GameObject.FindGameObjectWithTag("gBar2") == null)
                {
                    map.mapS.gBar2 = PhotonNetwork.Instantiate("goldBar2", new Vector3(0f, -10f, 0f), Quaternion.identity);
                    map.mapS.gBar2.SetActive(false);
                }
                if (GameObject.FindGameObjectWithTag("gBar3") == null)
                {
                    map.mapS.gBar3 = PhotonNetwork.Instantiate("goldBar3", new Vector3(0f, -10f, 0f), Quaternion.identity);
                    map.mapS.gBar3.SetActive(false);
                }
            }

            //GameObject.Find("ScreenCanvas/butRank").GetComponent<Button>().interactable = true;
            map.mapS.pl1 = GameObject.FindGameObjectWithTag("player1");
            map.mapS.pl2 = GameObject.FindGameObjectWithTag("player2");
            map.mapS.pl3 = GameObject.FindGameObjectWithTag("player3");

            if (GameObject.FindGameObjectWithTag("GameLogic") == null)  //only one GL per room
            {
                if (PhotonNetwork.IsMasterClient) { PhotonNetwork.Instantiate("GameLogic", new Vector3(-10f, -10f, -10f), Quaternion.identity); }
            }

            if (PhotonNetwork.IsMasterClient) {
                GameObject.FindGameObjectWithTag("GameLogic").GetComponent<PhotonView>().RPC("RPC_pl1start",
                    GameObject.FindGameObjectWithTag("player1").GetComponent<PhotonView>().Owner);
            }                
            
            bStart = true;
        }        
    }

    
}
