using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Credits : MonoBehaviour
{
    public GameObject creditsPanel;
    public TextMeshProUGUI creditsText;

    // Start is hidden until it's called from other UI like a button
    private void Start()
    {
    }

    // Call this function from a Button to show the Credits Panel
    public void ShowCredits()
    {
        creditsText.text =
     "Programming: Beziks\n" +
     "Sound Design: Beziks\n" +
     "Graphic Design: Beziks, \n" +
     "Level Design: Beziks\n" +
     "Story Writing: Beziks\n" +
     "Voice Acting: - \n" +
     "Animation: Beziks\n" +
     "Quality Assurance: Beziks\n" +
     "Music Composition: -\n" +
     "Sound Effects by: -\n\n" +
     "Icons used from website https://game-icons.net/\r \n" +
     "Music used in game created by:\n" +
     "Track1: Music: Bensound.com/free-music-for-videos\r\nLicense code: 5Y79NFPQWQ0C8XGL \n" +
     "Track2: NickI\n" +
     "Track3: NickJ\n" +
     "Track4: NickH\n" +
     "Track5: NickI\n" +
     "Track6: NickJ\n" +
     "Track7: NickH\n" +
     "Track8: NickI\n" +
     "Track9: NickI\n" +
     "Track10: Music by https://www.bensound.com/free-music-for-videos\r\nLicense code: FSHPCNYUBBHGIHI4\n" +
     "Track11: Dungeon Of Fear by John Bartmann \n" +
     "Track12: NickI\n" +
     "Track13: NickH\n" +
     "Track14: NickI\n" +
     "Track15: NickJ\n\n" +
     "Testers: Beziks, Fouxen, Humcio\n\n" +
     "Special Thanks to all people playing!\n";

        creditsPanel.SetActive(true);
    }

    // Call this function from a Button to hide the Credits Panel
    public void HideCredits()
    {
        creditsPanel.SetActive(false);
    }
    private void OnEnable()
    {
        ShowCredits();
    }
        
}