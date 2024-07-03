using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseEvent : MonoBehaviour
{
    public GameObject window;
    void Update()
    {
        if (!window.activeSelf)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }


        if(Input.GetKeyDown(KeyCode.Escape))
        {
            window.SetActive(!window.activeSelf);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }


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
