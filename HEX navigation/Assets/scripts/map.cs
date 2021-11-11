using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class map : MonoBehaviour
{
    public static map mapS;  //map class singleton

    [SerializeField] public GameObject[] hexes = new GameObject[20];

    public GameObject pl1, pl2, pl3;
    public GameObject gBar1, gBar2, gBar3;    

    [SerializeField] public GameObject hexInfoPanel;
    [SerializeField] Button rankBtn;
    [SerializeField] GameObject rankPanel; 

    private void Awake()
    {
        mapS = this;    

        rankBtn.GetComponent<Button>().onClick.AddListener(() => {
            rankPanel.SetActive(true);
            rankPanel.GetComponentInChildren<Text>().text =
                pl1.GetComponent<PhotonView>().Owner.NickName.ToString() + ": " + pl1.GetComponent<stats>().gold.ToString() + "\n"
                + pl2.GetComponent<PhotonView>().Owner.NickName.ToString() + ": " + pl2.GetComponent<stats>().gold.ToString() + "\n"
                + pl3.GetComponent<PhotonView>().Owner.NickName.ToString() + ": " + pl3.GetComponent<stats>().gold.ToString();
        });
    }

}
