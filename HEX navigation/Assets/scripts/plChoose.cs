using UnityEngine;
using UnityEngine.UI;

public class plChoose : MonoBehaviour
{
    public Sprite[] plSpr;
    [SerializeField] Slider slider;
    [SerializeField] GameObject startButton;
    [SerializeField] GameObject pl1r, pl2r, pl3r;


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
            pl2r.GetComponent<SpriteRenderer>().sprite = pl2r.GetComponent<plChoose>().plSpr[0];
            pl3r.GetComponent<SpriteRenderer>().sprite = pl3r.GetComponent<plChoose>().plSpr[0];
        }
        else if (name == "2")
        {
            slider.value = 2;
            pl1r.GetComponent<SpriteRenderer>().sprite = pl1r.GetComponent<plChoose>().plSpr[0];
            pl3r.GetComponent<SpriteRenderer>().sprite = pl3r.GetComponent<plChoose>().plSpr[0];            
        }
        else
        {
            slider.value = 3;
            pl1r.GetComponent<SpriteRenderer>().sprite = pl1r.GetComponent<plChoose>().plSpr[0];
            pl2r.GetComponent<SpriteRenderer>().sprite = pl2r.GetComponent<plChoose>().plSpr[0];
        }

        startButton.GetComponent<Button>().interactable = true;
    }
       
}
