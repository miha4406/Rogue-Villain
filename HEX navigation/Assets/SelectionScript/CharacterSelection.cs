using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CharacterSelection : MonoBehaviour
{
    public int selectedCharacter = 0;
    public GameObject[] characters;
    public TMP_Text[] charactersDescriptions;
   

    public void NextCharacter()
    {
        characters[selectedCharacter].SetActive(false);
        charactersDescriptions[selectedCharacter].gameObject.SetActive(false);
        selectedCharacter = (selectedCharacter + 1) % characters.Length;
        characters[selectedCharacter].SetActive(true);
        
        //selectedCharacter = (selectedCharacter + 1) % charactersDescriptions.Length;
        charactersDescriptions[selectedCharacter].gameObject.SetActive(true);
    }

    public void PreviousCharacter()
    {
        characters[selectedCharacter].SetActive(false);
        charactersDescriptions[selectedCharacter].gameObject.SetActive(false);
        selectedCharacter--;
        if (selectedCharacter < 0)
        {
            selectedCharacter += characters.Length;
            //selectedCharacter += charactersDescriptions.Length;
        }
        characters[selectedCharacter].SetActive(true);
        charactersDescriptions[selectedCharacter].gameObject.SetActive(true);
    }

    public void PlayGame()
    {
        PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void ReturnMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
