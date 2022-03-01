using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;


public class map : MonoBehaviour
{
    public static map mapS;  //map class singleton

    [SerializeField] public GameObject[] hexes = new GameObject[20];

    public Sprite[] pl1logo, pl2logo, pl3logo;
    public GameObject pl1, pl2, pl3;
    public GameObject gBar1, gBar2, gBar3;
    public GameObject item1, item2, item3;
    public Sprite[] itemIcons;
    [Multiline] public string[] itemTips;

    [SerializeField] public GameObject hexInfoPanel;    
    [SerializeField] public GameObject resultPanel;
    [SerializeField] public Image enemyLogo1, enemyLogo2;
    [SerializeField] public Text nick1, nick2, gold1, gold2;
    [SerializeField] Button menuBtn;
    [SerializeField] Text waitText;
    public bool bWait = false;
    byte a = 255;

    //item
    public Vector3[] itemPos = new Vector3[3];
    public bool bNewItem = false;
    public GameObject sparksEffect;

    //sound
    public AudioClip[] skillClips;
    public AudioClip[] itemClips;
    public AudioClip[] otherClips;

    private void Awake()
    {
        mapS = this;

        menuBtn.onClick.AddListener(() =>
        {
            resultPanel.SetActive(true); resultPanel.GetComponent<resultScript>().enabled = true;
            //rankPanel.active = !rankPanel.active;
            //rankPanel.GetComponentInChildren<Text>().text =
            //    "(player1) " + pl1.GetComponent<PhotonView>().Owner.NickName.ToString() + ": " + pl1.GetComponent<stats>().gold.ToString() + "\n"
            //    + "(player2) " + pl2.GetComponent<PhotonView>().Owner.NickName.ToString() + ": " + pl2.GetComponent<stats>().gold.ToString() + "\n"
            //    + "(player3) " + pl3.GetComponent<PhotonView>().Owner.NickName.ToString() + ": " + pl3.GetComponent<stats>().gold.ToString();
        });

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
        if (bNewItem) //item hexes update
        {     
            item1.transform.position = (Vector3)PhotonNetwork.CurrentRoom.CustomProperties["iHex0"];
            item2.transform.position = (Vector3)PhotonNetwork.CurrentRoom.CustomProperties["iHex1"];
            item3.transform.position = (Vector3)PhotonNetwork.CurrentRoom.CustomProperties["iHex2"];

            bNewItem = false;            
        }

        if (bWait) { waitText.enabled = true; waitText.color = new Color32(0, 0, 0, a); a--; }
        else { waitText.enabled = false; }
        if (pl1!=null && pl2!=null && pl3!=null)
        {
            if (!pl1.GetComponent<stats>().bMyTurn && !pl2.GetComponent<stats>().bMyTurn && !pl3.GetComponent<stats>().bMyTurn) { bWait = false; }
        }
        

        //enemy panel update
        if (pl1 != null && pl2 != null && pl3 != null) 
        {            
            if (enemyLogo1.sprite == pl1logo[0] && pl1.GetComponent<stats>().bMyTurn)
            {
                enemyLogo1.sprite = pl1logo[1]; 
            }
            else if (enemyLogo1.sprite == pl1logo[1] && !pl1.GetComponent<stats>().bMyTurn)
            {
                enemyLogo1.sprite = pl1logo[0];
            }
            if (enemyLogo1.sprite == pl2logo[0] && pl2.GetComponent<stats>().bMyTurn)
            {
                enemyLogo1.sprite = pl2logo[1];
            }
            else if (enemyLogo1.sprite == pl2logo[1] && !pl2.GetComponent<stats>().bMyTurn)
            {
                enemyLogo1.sprite = pl2logo[0];
            }
            if (enemyLogo1.sprite == pl3logo[0] && pl3.GetComponent<stats>().bMyTurn)
            {
                enemyLogo1.sprite = pl3logo[1];
            }
            else if (enemyLogo1.sprite == pl3logo[1] && !pl3.GetComponent<stats>().bMyTurn)
            {
                enemyLogo1.sprite = pl3logo[0];
            }

            if (enemyLogo2.sprite == pl1logo[0] && pl1.GetComponent<stats>().bMyTurn)
            {
                enemyLogo2.sprite = pl1logo[1];
            }
            else if (enemyLogo2.sprite == pl1logo[1] && !pl1.GetComponent<stats>().bMyTurn)
            {
                enemyLogo2.sprite = pl1logo[0];
            }
            if (enemyLogo2.sprite == pl2logo[0] && pl2.GetComponent<stats>().bMyTurn)
            {
                enemyLogo2.sprite = pl2logo[1];
            }
            else if (enemyLogo2.sprite == pl2logo[1] && !pl2.GetComponent<stats>().bMyTurn)
            {
                enemyLogo2.sprite = pl2logo[0];
            }
            if (enemyLogo2.sprite == pl3logo[0] && pl3.GetComponent<stats>().bMyTurn)
            {
                enemyLogo2.sprite = pl3logo[1];
            }
            else if (enemyLogo2.sprite == pl3logo[1] && !pl3.GetComponent<stats>().bMyTurn)
            {
                enemyLogo2.sprite = pl3logo[0];
            }
        }

        //result panel view
        if (PhotonNetwork.CurrentRoom.CustomProperties["tNo"] != null) 
        {
            if ((int)PhotonNetwork.CurrentRoom.CustomProperties["tNo"]>10 &&  //game over
                GameObject.Find("ScreenCanvas/consolePanel").GetComponent<DebugStuff.ConsoleToGUI>().enabled)
            {
                Invoke("showResults", 5f);
                GameObject.Find("ScreenCanvas/consolePanel").GetComponent<DebugStuff.ConsoleToGUI>().enabled = false;                

                GameObject.FindGameObjectWithTag("Statics").GetComponent<AudioSource>().Stop();
                GetComponent<AudioSource>().PlayOneShot(otherClips[1]);  //se
            }
        }
            
    }


    public void hexInfo(Vector3 clHex, Vector3 plHex)
    {
        Text[] newText = hexInfoPanel.GetComponentsInChildren<Text>();        

        if (clHex == pl1.transform.position)
        {
            newText[0].text = "怪盗（プレイヤー１）";
            newText[1].text = pl1.GetComponent<PhotonView>().Owner.NickName.ToString();  
            newText[2].text = "距離：" + Mathf.RoundToInt(Vector3.Distance(clHex, plHex) + 0.3f).ToString();
        }
        else if (clHex == pl2.transform.position)
        {
            newText[0].text = "海賊（プレイヤー２）";
            newText[1].text = pl2.GetComponent<PhotonView>().Owner.NickName.ToString();  
            newText[2].text = "距離：" + Mathf.RoundToInt(Vector3.Distance(clHex, plHex) + 0.3f).ToString();
        }
        else if (clHex == pl3.transform.position)
        {
            newText[0].text = "探検家（プレイヤー３）";
            newText[1].text = pl3.GetComponent<PhotonView>().Owner.NickName.ToString();  
            newText[2].text = "距離：" + Mathf.RoundToInt(Vector3.Distance(clHex, plHex) + 0.3f).ToString();
        }
        else if (clHex == gBar1?.transform.position || clHex == gBar2?.transform.position || clHex == gBar3?.transform.position)
        {
            newText[0].text = "金塊マス";
            newText[1].text = "金塊を探せる";
            newText[2].text = "距離：" + Mathf.FloorToInt(Vector3.Distance(clHex, plHex) + 0.3f).ToString();
        }
        else if (clHex == (Vector3)PhotonNetwork.CurrentRoom.CustomProperties["iHex0"] 
                || clHex == (Vector3)PhotonNetwork.CurrentRoom.CustomProperties["iHex1"] 
                || clHex == (Vector3)PhotonNetwork.CurrentRoom.CustomProperties["iHex2"])
        {
            newText[0].text = "アイテム・マス";
            newText[1].text = "アイテムを拾える";
            newText[2].text = "距離：" + Mathf.RoundToInt(Vector3.Distance(clHex, plHex) + 0.3f).ToString();
        }
        else
        {
            newText[0].text = "普段マス";
            newText[1].text = "効果なし";
            newText[2].text = "距離：" + Mathf.RoundToInt(Vector3.Distance(clHex, plHex) + 0.3f).ToString();
        }
    }

    public void bombClear()
    {
        foreach (GameObject bomb in GameObject.FindGameObjectsWithTag("bombPref"))
        {
            Destroy(bomb);
        }
    }

    void showResults()
    {
        resultPanel.SetActive(true); resultPanel.GetComponent<result>().enabled = true;        

        GetComponent<AudioSource>().PlayOneShot(otherClips[2]);  //se
    }
}
