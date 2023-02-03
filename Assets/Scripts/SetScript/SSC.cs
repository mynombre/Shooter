using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SSC : MonoBehaviour
{

    public TextMeshProUGUI myText;
    private string currentOption = "On";

    void Start()
    {
        myText.text = currentOption;
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void myButtonClick()
    {
        switch (currentOption)
        {
            case "On":
                currentOption = "Off";
                myText.text = currentOption;
                Screen.SetResolution(960, 540, false, 60);
                Screen.fullScreen = !Screen.fullScreen;
                break;

            case "Off":
                currentOption = "On";
                myText.text = currentOption;
                Screen.fullScreen = !Screen.fullScreen;
                break;
        }

    }
}
