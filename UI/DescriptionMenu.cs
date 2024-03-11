using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DescriptionMenu : MonoBehaviour
{

    [SerializeField] GameObject panel;
    PauseManager pauseManager;

    private void Awake()
    {
        pauseManager = GetComponent<PauseManager>();
    }

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
        pauseManager.PauseGame();
        panel.SetActive(true);
    }
}
