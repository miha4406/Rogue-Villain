using UnityEngine;
using UnityEngine.UI;

public class resultScript : MonoBehaviour
{
    [SerializeField] Image img1, img2, img3, imgF;

    [SerializeField] public Sprite[] p1sp, p2sp, p3sp, frames;

    private int gold1, gold2, gold3;


    void Start()
    {
        gold1 = GameObject.FindGameObjectWithTag("player1").GetComponent<stats>().gold;
        gold2 = GameObject.FindGameObjectWithTag("player2").GetComponent<stats>().gold;
        gold3 = GameObject.FindGameObjectWithTag("player3").GetComponent<stats>().gold;


        if (gold1==gold2 && gold2==gold3)
        {
            //img1.sprite = p1sp[0]; img2.sprite = p2sp[0]; img3.sprite = p3sp[0];
            //img1.GetComponent<RectTransform>().sizeDelta = new Vector2(img1.GetComponent<RectTransform>().rect.width/3, img1.GetComponent<RectTransform>().sizeDelta.y);            
            imgF.sprite = frames[0];
        }

        if (gold1>gold2 && gold2>gold3)  //1-2-3
        {
            img1.sprite = p1sp[1]; img2.sprite = p2sp[2]; img3.sprite = p3sp[3];
            imgF.sprite = frames[1];
        }
        if (gold1>gold3 && gold3>gold2)  //1-3-2
        {
            img1.sprite = p1sp[1]; img2.sprite = p3sp[2]; img3.sprite = p2sp[3];
            imgF.sprite = frames[1];
        }

        if (gold2>gold1 && gold1>gold3)  //2-1-3
        {
            img1.sprite = p2sp[1]; img2.sprite = p1sp[2]; img3.sprite = p3sp[3];
            imgF.sprite = frames[1];
        }
        if (gold2>gold3 && gold3>gold1)  //2-3-1
        {
            img1.sprite = p2sp[1]; img2.sprite = p3sp[2]; img3.sprite = p1sp[3];
            imgF.sprite = frames[1];
        }

        if (gold3>gold1 && gold1>gold2)  //3-1-2
        {
            img1.sprite = p3sp[1]; img2.sprite = p1sp[2]; img3.sprite = p2sp[3];
            imgF.sprite = frames[1];
        }
        if (gold3>gold2 && gold2>gold1)  //3-2-1
        {
            img1.sprite = p3sp[1]; img2.sprite = p2sp[2]; img3.sprite = p1sp[3];
            imgF.sprite = frames[1];
        }

        if (gold1==gold2 && gold2>gold3)  //1+2-3
        {
            img1.sprite = p1sp[4]; img2.sprite = p2sp[5]; img3.sprite = p3sp[6];
            imgF.sprite = frames[2];
        }
        if (gold1==gold3 && gold3>gold2)  //1+3-2
        {
            img1.sprite = p1sp[4]; img2.sprite = p3sp[5]; img3.sprite = p2sp[6];
            imgF.sprite = frames[2];
        }
        if (gold3==gold2 && gold2>gold1)  //3+2-1
        {
            img1.sprite = p3sp[4]; img2.sprite = p2sp[5]; img3.sprite = p1sp[6];
            imgF.sprite = frames[2];
        }

        if (gold1==gold2 && gold2<gold3)  //3-1+2
        {
            img1.sprite = p3sp[4]; img2.sprite = p1sp[5]; img3.sprite = p2sp[6];
            imgF.sprite = frames[3];
        }
        if (gold1==gold3 && gold3<gold2)  //2-1+3
        {
            img1.sprite = p2sp[4]; img2.sprite = p1sp[5]; img3.sprite = p3sp[6];
            imgF.sprite = frames[3];
        }
        if (gold2==gold3 && gold3<gold1)  //1-2+3
        {
            img1.sprite = p1sp[4]; img2.sprite = p2sp[5]; img3.sprite = p3sp[6];
            imgF.sprite = frames[3];
        }
    }

    
    
}
