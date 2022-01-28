using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class plChoose : MonoBehaviour
{
    public Sprite[] plSpr;
    [SerializeField] Slider slider;
    [SerializeField] GameObject startButton;
    [SerializeField] GameObject[] chars;

    static ExitGames.Client.Photon.Hashtable rProp = new ExitGames.Client.Photon.Hashtable();    
    static int[] currChoice = new int[3];

    private void Start()
    {  
        
    }


    private void OnMouseOver()
    {
        if (slider.value.ToString() != name) {
            GetComponent<SpriteRenderer>().sprite = plSpr[1];
        }
                    
    }

    private void OnMouseExit()
    {
        if (slider.value.ToString() != name) { 
            GetComponent<SpriteRenderer>().sprite = plSpr[0]; 
        }
        
    }

    private void OnMouseDown()
    {
        GetComponent<SpriteRenderer>().sprite = plSpr[2];

        if (name == "1")
        {
            slider.value = 1;
            chars[1].GetComponent<SpriteRenderer>().sprite = chars[1].GetComponent<plChoose>().plSpr[0];
            chars[2].GetComponent<SpriteRenderer>().sprite = chars[2].GetComponent<plChoose>().plSpr[0];

            clearChars();
            rProp["char1"] = PhotonNetwork.LocalPlayer.ActorNumber;
        }
        else if (name == "2")
        {
            slider.value = 2;
            chars[0].GetComponent<SpriteRenderer>().sprite = chars[0].GetComponent<plChoose>().plSpr[0];
            chars[2].GetComponent<SpriteRenderer>().sprite = chars[2].GetComponent<plChoose>().plSpr[0];

            clearChars();
            rProp["char2"] = PhotonNetwork.LocalPlayer.ActorNumber;
        }
        else
        {
            slider.value = 3;
            chars[0].GetComponent<SpriteRenderer>().sprite = chars[0].GetComponent<plChoose>().plSpr[0];
            chars[1].GetComponent<SpriteRenderer>().sprite = chars[1].GetComponent<plChoose>().plSpr[0];

            clearChars();
            rProp["char3"] = PhotonNetwork.LocalPlayer.ActorNumber;
        }

        //startButton.GetComponent<Button>().interactable = true;
        PhotonNetwork.CurrentRoom.SetCustomProperties(rProp);        
    }
       

    public void startRefrChars()
    {
        rProp["char1"] = 0; rProp["char2"] = 0; rProp["char3"] = 0;
        PhotonNetwork.CurrentRoom.SetCustomProperties(rProp);
        currChoice[0] = 0; currChoice[1] = 0; currChoice[2] = 0;

        InvokeRepeating("refrChars", 2f, 1f);
    }
    void refrChars()
    {
        currChoice[0] = (int)PhotonNetwork.CurrentRoom.CustomProperties["char1"];
        currChoice[1] = (int)PhotonNetwork.CurrentRoom.CustomProperties["char2"];
        currChoice[2] = (int)PhotonNetwork.CurrentRoom.CustomProperties["char3"];

        for(int i=0; i<=2; i++)
        {
            if (currChoice[i]!=0 && currChoice[i]!=PhotonNetwork.LocalPlayer.ActorNumber)
            {
                chars[i].GetComponent<SpriteRenderer>().sprite = chars[i].GetComponent<plChoose>().plSpr[3];
                chars[i].GetComponent<BoxCollider2D>().enabled = false;
            }
            else if(currChoice[i] == 0)
            {
                if (chars[i].GetComponent<SpriteRenderer>().sprite.name.Contains("unable") ) 
                { chars[i].GetComponent<SpriteRenderer>().sprite = chars[i].GetComponent<plChoose>().plSpr[0]; }
                
                chars[i].GetComponent<BoxCollider2D>().enabled = true;
            }
        }

        print(currChoice[0] +"  " + currChoice[1] + "  " + currChoice[2]);
    }

    void clearChars()
    {
        if (currChoice[0] == PhotonNetwork.LocalPlayer.ActorNumber) { rProp["char1"] = 0; }
        if (currChoice[1] == PhotonNetwork.LocalPlayer.ActorNumber) { rProp["char2"] = 0; }
        if (currChoice[2] == PhotonNetwork.LocalPlayer.ActorNumber) { rProp["char3"] = 0; }

        //PhotonNetwork.CurrentRoom.SetCustomProperties(rProp);
    }

}
