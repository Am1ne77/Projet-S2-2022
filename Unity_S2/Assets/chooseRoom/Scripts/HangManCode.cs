using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HangManCode : MonoBehaviour
{
    public GameObject gameDescr;
    // Start is called before the first frame update


    void Start()
    {
        gameDescr.SetActive(false);
    }

    public void OnMouseOver()
    {
        gameDescr.SetActive(true);
        Debug.Log("hehe");
    }

    public void OnMouseExit()
    {
        gameDescr.SetActive(false);
        Debug.Log("not");
    }
}
