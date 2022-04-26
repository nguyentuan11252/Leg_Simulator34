using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Ins;

    public GameObject btnReplay;
    public GameObject btnTapPlay;
    private void Awake()
    {
        Ins = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        btnReplay.SetActive(false);
        btnTapPlay.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(Enemy.isDealth == true)
        {
            btnReplay.SetActive(true);
        }
        if(LegCharacter.isDealth == true)
        {
            StartCoroutine(ButtonReload());
        }
    }
    IEnumerator ButtonReload()
    {
        yield return new WaitForSeconds(2.5f);
        btnReplay.SetActive(true);
    }
    public void Replay()
    {
        SoccerPlayerController.isPlay = true;
        Enemy.isDealth = false;
        btnReplay.SetActive(false);
        btnTapPlay.SetActive(true);
        SceneManager.LoadScene("Gameplay3");
    }
    public void ReplayG4()
    {
        SoccerPlayerController.isPlay = true;
        LegCharacter.isDealth = false;
        btnTapPlay.SetActive(true);
        btnReplay.SetActive(false);
        SceneManager.LoadScene("Gameplay4");
    }
    public void TapPlayGame()
    {
        btnTapPlay.SetActive(false);
        SoccerPlayerController.isPlay = false;
    }

}
