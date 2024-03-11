using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuToCity : MonoBehaviour
{
    public void BackToCity()
    {
        SceneManager.LoadScene("Town");

    }
}

