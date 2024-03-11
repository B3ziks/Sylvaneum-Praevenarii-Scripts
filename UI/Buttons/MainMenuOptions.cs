using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuOptions : MonoBehaviour
{

    [SerializeField] GameObject panel;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void CloseMenu()
    {
        panel.SetActive(false);
    }

    public void OpenMenu()
    {
        panel.SetActive(true);
    }
}
