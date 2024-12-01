using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI title;

    //Main Menu
    public GameObject play;
    public GameObject options;
    public GameObject exit;

    //Tutorial Screen
    public GameObject tutorial;

    //Options Menu
    public GameObject volume;
    public GameObject volumeControl;
    public GameObject music;
    public GameObject musicControl;
    public GameObject se;
    public GameObject seControl;
    public GameObject mainmenu;

    //Transition
    public Animator transition;
    public float transitionTime = 2f;

    //Sound Control
    public AudioMixer master;

    public void Play()
    {
        StartCoroutine("Tutorial");

    }

    public void StartGame()
    {
        StartCoroutine("playGame");
    }

    public void Options()
    {
        StartCoroutine("OptionsTransition");
    }

    public void Menu()
    {
        StartCoroutine("MenuTransition");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void setMaster(float sliderValue)
    {
        master.SetFloat("MasterVol", Mathf.Log10(sliderValue) * 20);
    }

    public void setMusic(float sliderValue)
    {
        master.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
    }

    public void setSE(float sliderValue)
    {
        master.SetFloat("SEVol", Mathf.Log10(sliderValue) * 20);
    }

    //Option Transition
    IEnumerator OptionsTransition()
    {
        transition.SetBool("Done", false);
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);
        //Gets rid of Main Menu buttons
        play.SetActive(false);
        options.SetActive(false);
        exit.SetActive(false);

        //Activates all objects for Options Menu
        title.text = "Options";
        volume.SetActive(true);
        volumeControl.SetActive(true);
        music.SetActive(true);
        musicControl.SetActive(true);
        se.SetActive(true);
        seControl.SetActive(true);
        mainmenu.SetActive(true);


        //Finishes transition
        transition.SetBool("Done", true);
        yield return new WaitForSeconds(1f);
        transition.SetBool("Done", false);

        yield return null;
    }

    //Main Menu Transition
    IEnumerator MenuTransition()
    {
        transition.SetBool("Done", false);
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);
        //Gets rid of Option Menu buttons
        volume.SetActive(false);
        volumeControl.SetActive(false);
        music.SetActive(false);
        musicControl.SetActive(false);
        se.SetActive(false);
        seControl.SetActive(false);
        mainmenu.SetActive(false);

        //Activates all objects for Main Menu
        title.text = "Miserable Little Holiday";
        play.SetActive(true);
        options.SetActive(true);
        exit.SetActive(true);

        //Finishes Transition
        transition.SetBool("Done", true);
        yield return new WaitForSeconds(1f);
        transition.SetBool("Done", false);

        yield return null;
    }

    IEnumerator Tutorial()
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);

        //Activates tutorial screen and disables the Main Menu
        GameObject.FindGameObjectWithTag("Menus").SetActive(false);
        tutorial.SetActive(true);


        //Finishes Transition
        transition.SetBool("Done", true);
        yield return new WaitForSeconds(1f);
        transition.SetBool("Done", false);

        yield return null;
    }

    //Game Scens Transition
    IEnumerator playGame()
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene("GameScene");
        yield return null;
    }
}
