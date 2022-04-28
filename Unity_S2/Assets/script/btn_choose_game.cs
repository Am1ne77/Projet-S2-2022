using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class btn_choose_game : MonoBehaviour
{
    public static UIManager instance;

    //Screen object variables

    public void Hangman()
    {
        SceneManager.LoadScene(4);
    }
    public void Puissance_4()
    {
        SceneManager.LoadScene(5);
    }
    public void MiniTank()
    {
        SceneManager.LoadScene(3);
    }
    
    
}
