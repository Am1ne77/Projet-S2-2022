using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Pendu_main : MonoBehaviour
{
    
    private static string _inputOfPlayer;

    public InputField Letter_input;
    
    public void Update()
    {
        if(Letter_input.text != null)
            _inputOfPlayer = Letter_input.text;
    }
    
    
    public static char GetInput()
    {
        if (_inputOfPlayer == null)
        {
            Debug.Log("Enter a valid char");
            GetInput();
        }
        string str = _inputOfPlayer;
        char character = '0';
        if (str.Length != 0 && char.IsLetter(str[0]))
        {
            character = char.Parse(str[0].ToString());
        }
        else
        {
            Debug.Log("Invalid Argument");
        }
        
        return character;

    }
    
}
