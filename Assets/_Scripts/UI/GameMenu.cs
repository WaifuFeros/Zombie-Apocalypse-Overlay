using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public void LoadGame(int buildIndex) => SceneManager.LoadScene(buildIndex);

    public void QuitApplication() => Application.Quit();
}
