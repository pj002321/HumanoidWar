using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_mene : MonoBehaviour
{

    public GameObject mainMenuUI;
    

    public void OnClickNewGame()
    {
        mainMenuUI.SetActive(false);
        //SceneManager.LoadScene("SampleScene");
    }

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
