using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;


public class map : MonoBehaviour
{
    public static map mapS;  //map class singleton

    [SerializeField] public GameObject[] hexes = new GameObject[20];

    [SerializeField] public Sprite pl1logo, pl2logo, pl3logo;
    public GameObject pl1, pl2, pl3;
    public GameObject gBar1, gBar2, gBar3;    

    [SerializeField] public GameObject hexInfoPanel;    
    [SerializeField] public GameObject resultPanel;
    [SerializeField] public Image enemyLogo1, enemyLogo2;
    [SerializeField] public Text px1, px2, nick1, nick2, gold1, gold2;


    public Vector3[] itemPos = new Vector3[3];
    public bool bNewItem = false;
    [SerializeField] Material itemMat;
    [SerializeField] Material groundMat;


    private void Awake()
    {
        mapS = this;        

        //rankBtn.GetComponent<Button>().onClick.AddListener(() => 
        //{
        //    rankPanel.active = !rankPanel.active;
        //    rankPanel.GetComponentInChildren<Text>().text =
        //        "(player1) " + pl1.GetComponent<PhotonView>().Owner.NickName.ToString() + ": " + pl1.GetComponent<stats>().gold.ToString() + "\n"
        //        + "(player2) " + pl2.GetComponent<PhotonView>().Owner.NickName.ToString() + ": " + pl2.GetComponent<stats>().gold.ToString() + "\n"
        //        + "(player3) " + pl3.GetComponent<PhotonView>().Owner.NickName.ToString() + ": " + pl3.GetComponent<stats>().gold.ToString();
        //});

        GameObject.Find("ScreenCanvas/resultPanel/butExit").GetComponent<Button>().onClick.AddListener(() =>
        {            
            PhotonNetwork.LoadLevel("s0");
            PhotonNetwork.LeaveRoom();
        });
        resultPanel.SetActive(false);

        itemPos = new Vector3[3] { Vector3.zero, Vector3.zero, Vector3.zero} ;
    }


    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient && bNewItem) {  //Host is controlled by itemControl
           
            foreach (GameObject hex in hexes)
            {
                if (hex != null) { hex.GetComponent<Renderer>().material = groundMat; }
            }

            print(PhotonNetwork.CurrentRoom.CustomProperties["iHex1"]); //takes moment to refresh

            foreach (GameObject hex in hexes)
            {
                if (hex != null)
                {
                    if (hex.transform.position == (Vector3)PhotonNetwork.CurrentRoom.CustomProperties["iHex0"] 
                        || hex.transform.position == (Vector3)PhotonNetwork.CurrentRoom.CustomProperties["iHex1"] 
                        || hex.transform.position == (Vector3)PhotonNetwork.CurrentRoom.CustomProperties["iHex2"])
                    {
                        hex.GetComponent<Renderer>().material = itemMat;
                    }
                }
            }

            bNewItem = false;            
        }        
    }


    public void hexInfo(Vector3 clHex, Vector3 plHex)
    {
        Text[] newText = hexInfoPanel.GetComponentsInChildren<Text>();        

        if (clHex == pl1.transform.position)
        {
            newText[0].text = "怪盗（プレイヤー１）";
            newText[1].text = pl1.GetComponent<PhotonView>().Owner.NickName.ToString();  //"About player 1";
            newText[2].text = "距離：" + Mathf.RoundToInt(Vector3.Distance(clHex, plHex) + 0.3f).ToString();
        }
        else if (clHex == pl2.transform.position)
        {
            newText[0].text = "海賊（プレイヤー２）";
            newText[1].text = pl2.GetComponent<PhotonView>().Owner.NickName.ToString();  //"About player 2";
            newText[2].text = "距離：" + Mathf.RoundToInt(Vector3.Distance(clHex, plHex) + 0.3f).ToString();
        }
        else if (clHex == pl3.transform.position)
        {
            newText[0].text = "探検家（プレイヤー３）";
            newText[1].text = pl3.GetComponent<PhotonView>().Owner.NickName.ToString();  //"About player 3";
            newText[2].text = "距離：" + Mathf.RoundToInt(Vector3.Distance(clHex, plHex) + 0.3f).ToString();
        }
        else if (clHex == gBar1?.transform.position || clHex == gBar2?.transform.position || clHex == gBar3?.transform.position)
        {
            newText[0].text = "Gold bar HEX";
            newText[1].text = "Can pick up gold bar";
            newText[2].text = "Distance: " + Mathf.FloorToInt(Vector3.Distance(clHex, plHex) + 0.3f).ToString();
        }
        else if (clHex == (Vector3)PhotonNetwork.CurrentRoom.CustomProperties["iHex0"] 
                || clHex == (Vector3)PhotonNetwork.CurrentRoom.CustomProperties["iHex1"] 
                || clHex == (Vector3)PhotonNetwork.CurrentRoom.CustomProperties["iHex2"])
        {
            newText[0].text = "Item HEX";
            newText[1].text = "Can pick up item";
            newText[2].text = "Distance: " + Mathf.RoundToInt(Vector3.Distance(clHex, plHex) + 0.3f).ToString();
        }
        else
        {
            newText[0].text = "Plain HEX";
            newText[1].text = "No effect";
            newText[2].text = "Distance: " + Mathf.RoundToInt(Vector3.Distance(clHex, plHex) + 0.3f).ToString();
        }
    }

    public void bombClear()
    {
        foreach (GameObject bomb in GameObject.FindGameObjectsWithTag("bombPref"))
        {
            Destroy(bomb);
        }
    }
}
