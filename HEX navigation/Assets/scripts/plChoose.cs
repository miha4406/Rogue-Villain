using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class plChoose : MonoBehaviour
{
    public Sprite[] plSpr;
    [SerializeField] Slider slider;
    [SerializeField] GameObject startButton;
    [SerializeField] GameObject[] chars;
    [SerializeField] GameObject statics;
    

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
            if (GetComponent<SpriteRenderer>().sprite != this.plSpr[3]) 
            { GetComponent<SpriteRenderer>().sprite = plSpr[0]; }            
        }        
    }

    private void OnMouseDown()
    {      
        GetComponent<SpriteRenderer>().sprite = plSpr[2];

        if (name == "1")
        {
            slider.value = 1;
            if (chars[1].GetComponent<SpriteRenderer>().sprite != chars[1].GetComponent<plChoose>().plSpr[3]) { chars[1].GetComponent<SpriteRenderer>().sprite = chars[1].GetComponent<plChoose>().plSpr[0]; }
            if (chars[2].GetComponent<SpriteRenderer>().sprite != chars[2].GetComponent<plChoose>().plSpr[3]) { chars[2].GetComponent<SpriteRenderer>().sprite = chars[2].GetComponent<plChoose>().plSpr[0]; }
        }
        else if (name == "2")
        {
            slider.value = 2;
            if (chars[0].GetComponent<SpriteRenderer>().sprite != chars[0].GetComponent<plChoose>().plSpr[3]) { chars[0].GetComponent<SpriteRenderer>().sprite = chars[0].GetComponent<plChoose>().plSpr[0]; }
            if (chars[2].GetComponent<SpriteRenderer>().sprite != chars[2].GetComponent<plChoose>().plSpr[3]) { chars[2].GetComponent<SpriteRenderer>().sprite = chars[2].GetComponent<plChoose>().plSpr[0]; }
        }
        else
        {
            slider.value = 3;
            if (chars[0].GetComponent<SpriteRenderer>().sprite != chars[0].GetComponent<plChoose>().plSpr[3]) { chars[0].GetComponent<SpriteRenderer>().sprite = chars[0].GetComponent<plChoose>().plSpr[0]; }
            if (chars[1].GetComponent<SpriteRenderer>().sprite != chars[1].GetComponent<plChoose>().plSpr[3]) { chars[1].GetComponent<SpriteRenderer>().sprite = chars[1].GetComponent<plChoose>().plSpr[0]; }
        }
                       

        statics.GetComponent<AudioSource>().PlayOneShot(statics.GetComponent<Statics>().bgmClips[2], 5.0f);
    }
       

}
