using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    //Score display
    public static int Score;
    public TextMeshProUGUI scoreDisplay;

    //Round display
    public TextMeshProUGUI round;
    private int roundNum = 1;

    //End screen
    public TextMeshProUGUI WinLose;
    public GameObject exit;
    public GameObject menu;
    public AudioSource audioplayer;
    public AudioClip win;
    public AudioClip lose;

    //Timer
    public TextMeshProUGUI Timer;
    private bool timeRunning;
    private float time = 30f;

    //Flashlight
    public GameObject flashlight;
    public GameObject battery;
    private bool batSpawn;

    //Spawner objects
    public GameObject BasicBoi;
    public GameObject[] presentBois = new GameObject[3];

    //Coordinates for spawning
    private Vector2 topLeft = new Vector2(-3f, 5f);
    private Vector2 bottomRight = new Vector2(6f, -3f);

    //Starting num of Fellas
    public int num = 6;

    public static bool GameStart;

    public static bool roundOver;

    public int totalBois;

    public Texture2D crosshair;

    //To keep track of last spawn coroutine
    private Coroutine lastSpawn;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(crosshair, Vector2.zero, CursorMode.ForceSoftware);

        batSpawn = true;
        totalBois = 1;

        Timer.text = Mathf.Round(time).ToString();
        num = 6;

        //Text display for round start
        StartCoroutine("GameLoopStart");
        StartCoroutine("CheckRoundOver");
    }

    //Update score
    private void Update()
    {
        scoreDisplay.text = Score.ToString();
    }

    //Return to Menu for Win/Lose buttons
    public void returnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    //Game quit for Win/Lose buttons
    public void exitGame()
    {
        Application.Quit();
    }

    IEnumerator CheckRoundOver()
    {
        while (true)
        {
            //Battery spawner call
            if (batSpawn)
            {
                StartCoroutine("BatSpawner");
            }

            //Checks for the timer
            if (!roundOver && timeRunning)
            {
                if(time >= 0f)
                {
                    //Decrease time as time goes on
                    time -= Time.deltaTime;
                    Timer.text = Mathf.Round(time).ToString();
                }
                else //When time hits 0 or less, stop the timer
                {
                    timeRunning = false;
                    time = 0f;
                }
                
            }

            if (time <= 0f)
            {
                //Lose by Time
                StopCoroutine(lastSpawn);

                //Cleanup the GameObjects in the scene
                GameObject[] bois = GameObject.FindGameObjectsWithTag("Dude");
                for (int i = 0; i < bois.Length; i++)
                {
                    Destroy(bois[i]);
                }

                //Turn off flashlight
                StopCoroutine("LightOn");
                flashlight.GetComponent<Flashlight>().clicking.Play();

                yield return new WaitForSeconds(0.5f);

                flashlight.GetComponent<Flashlight>().StopAllCoroutines();
                flashlight.SetActive(false);

                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

                //Play lose sound
                GameObject.FindGameObjectWithTag("MusicPlayer").GetComponent<AudioSource>().Stop();
                audioplayer.PlayOneShot(lose, 0.5f);

                WinLose.text = "You're too slow! \n Christmas is saved! \n You Lose :(";

                exit.SetActive(true);
                menu.SetActive(true);
                break;
            }

            if (totalBois == Person.Presents)
            {
                //Lose by Gifts
                StopCoroutine(lastSpawn);

                //Cleanup the GameObjects in the scene
                GameObject[] bois = GameObject.FindGameObjectsWithTag("Dude");
                for (int i = 0; i < bois.Length; i++)
                {
                    Destroy(bois[i]);
                }

                //Turn off flashlight
                StopCoroutine("LightOn");
                flashlight.GetComponent<Flashlight>().clicking.Play();

                yield return new WaitForSeconds(0.5f);

                flashlight.GetComponent<Flashlight>().StopAllCoroutines();
                flashlight.SetActive(false);

                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

                //Play lose sound
                GameObject.FindGameObjectWithTag("MusicPlayer").GetComponent<AudioSource>().Stop();
                audioplayer.PlayOneShot(lose, 0.5f);

                WinLose.text = "Everyone Got A Gift! \n Christmas is saved! \n You Lose :(";

                exit.SetActive(true);
                menu.SetActive(true);
                break;
            }

            if (roundOver)
            {
                timeRunning = false;
                if (roundNum <= 3)
                {
                    //Stop the coroutines for the spawn (in case it didnt shut off) and the Flashlight
                    StopCoroutine(lastSpawn);
                    StopCoroutine("LightOn");

                    //Count up the round and say round is completed
                    roundNum++;
                    WinLose.text = "Round Complete!";

                    //Turn off the flashlight
                    flashlight.GetComponent<Flashlight>().clicking.Play();
                    yield return new WaitForSeconds(0.5f);
                    flashlight.SetActive(false);
                    yield return new WaitForSeconds(2f);

                    //Cleanup the GameObjects in the scene
                    GameObject[] bois = GameObject.FindGameObjectsWithTag("Dude");
                    for (int i = 0; i < bois.Length; i++)
                    {
                        Destroy(bois[i]);
                    }

                    GameObject[] destruction = GameObject.FindGameObjectsWithTag("Destruction");
                    for (int i = 0; i < destruction.Length; i++)
                    {
                        Destroy(destruction[i]);
                    }

                    //Text display for next round
                    WinLose.text = "";
                    roundDisplay(roundNum);
                    time = 30f;
                    yield return new WaitForSeconds(3f);

                    //Start next round
                    num += 3;
                    lastSpawn = StartCoroutine(SpawnBois2(num));
                    round.text = "";
                    StartCoroutine("LightOn");
                }
                else if (roundNum < 5 && roundNum > 3)
                {
                    //Stop the coroutines for the spawn (in case it didnt shut off) and the Flashlight
                    StopCoroutine(lastSpawn);
                    StopCoroutine("LightOn");

                    //Count up the round and say round is completed
                    roundNum++;
                    WinLose.text = "Round Complete!";

                    //Turn off the flashlight
                    flashlight.GetComponent<Flashlight>().clicking.Play();
                    yield return new WaitForSeconds(0.5f);
                    flashlight.SetActive(false);
                    yield return new WaitForSeconds(2f);

                    //Cleanup the GameObjects in the scene
                    GameObject[] bois = GameObject.FindGameObjectsWithTag("Dude");
                    for (int i = 0; i < bois.Length; i++)
                    {
                        Destroy(bois[i]);
                    }

                    GameObject[] destruction = GameObject.FindGameObjectsWithTag("Destruction");
                    for (int i = 0; i < destruction.Length; i++)
                    {
                        Destroy(destruction[i]);
                    }

                    //Text display for next round
                    WinLose.text = "";
                    roundDisplay(roundNum);
                    time = 30f;
                    yield return new WaitForSeconds(3f);

                    //Start next round
                    num += 3;
                    lastSpawn = StartCoroutine(SpawnBois2(num));
                    round.text = "";
                    StartCoroutine("LightOn");
                }
                else if (roundNum == 5)
                {
                    //Win
                    timeRunning = false;

                    //Cleanup the GameObjects in the scene
                    GameObject[] bois = GameObject.FindGameObjectsWithTag("Dude");
                    for (int i = 0; i < bois.Length; i++)
                    {
                        Destroy(bois[i]);
                    }

                    GameObject[] destruction = GameObject.FindGameObjectsWithTag("Destruction");
                    for (int i = 0; i < destruction.Length; i++)
                    {
                        Destroy(destruction[i]);
                    }

                    //Turn off flashlight
                    flashlight.GetComponent<Flashlight>().clicking.Play();
                    yield return new WaitForSeconds(0.5f);
                    flashlight.GetComponent<Flashlight>().StopAllCoroutines();
                    flashlight.SetActive(false);

                    //Show cursor
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

                    //Play win sound
                    GameObject.FindGameObjectWithTag("MusicPlayer").GetComponent<AudioSource>().Stop();
                    audioplayer.PlayOneShot(win, 0.5f);

                    WinLose.text = "You Destroyed Christmas! \n Are you happy now? \n You Win!";

                    exit.SetActive(true);
                    menu.SetActive(true);
                    break;
                }
                
                //Once round checks are over, set roundOver to false to start everything up again
                roundOver = false;
            }
            yield return null;
        }
    }

    public void roundDone()
    {
        roundOver = true;
    }

    //Coroutine for clicking on someone without a present
    IEnumerator Mistake()
    {
        //Turn off the light
        flashlight.GetComponent<Flashlight>().click = false;
        flashlight.SetActive(false);

        yield return new WaitForSeconds(1f);

        //Turn it back on
        flashlight.SetActive(true);
        flashlight.GetComponent<Flashlight>().click = true;

        yield return null;
    }

    //Coroutine for spawning Batteries
    IEnumerator BatSpawner()
    {
        //Spawns a battery at a random position within the play field
        Vector2 spawnPos = new Vector2(Random.Range(topLeft.x + 1, bottomRight.x - 1), Random.Range(bottomRight.y + 1, topLeft.y - 1));
        Instantiate(battery, spawnPos, Quaternion.identity);

        //Waits 12 seconds before spawning a new one
        batSpawn = false;
        yield return new WaitForSeconds(12f);
        batSpawn = true;
    }

    //First initial run of the game
    IEnumerator GameLoopStart()
    {
        yield return new WaitForSeconds(2f);

        //Display round number
        roundDisplay(roundNum);

        yield return new WaitForSeconds(3f);

        //Spawn the fellas and turn on the light
        lastSpawn = StartCoroutine(SpawnBois1(num));
        round.text = "";
        StartCoroutine("LightOn");

        yield return new WaitForSeconds(3f);

    }

    //Score counter
    public void addScore(int score)
    {
        Score += score;
    }

    //Round display
    void roundDisplay(int roundNum)
    {
        round.text = "Round: " + roundNum.ToString();
    }

    //Flashlight on Coroutine
    IEnumerator LightOn()
    {
        flashlight.SetActive(true);
        flashlight.GetComponent<Flashlight>().clicking.Play();
        flashlight.GetComponent<Flashlight>().lightOn();
        GameStart = true;
        yield return null;
    }

    //Single Present Individual spawn
    IEnumerator SpawnBois1(int num)
    {
        Vector2 spawnPos;
        Vector2[] bois = new Vector2[num];

        //Spawn num of bois at random positions
        for (int i = 0; i < num; i++)
        {
            spawnPos = new Vector2(Random.Range(topLeft.x, bottomRight.x), Random.Range(bottomRight.y, topLeft.y));
            bois[i] = spawnPos;
            Instantiate(BasicBoi, spawnPos, Quaternion.identity);
        }

        //Check if the generated coordinate is not where someone else is
        do
        {
            spawnPos = new Vector2(Random.Range(topLeft.x, bottomRight.x), Random.Range(bottomRight.y, topLeft.y));
        } while (System.Array.IndexOf(bois, spawnPos) != -1);

        //Spawn a Present Individual
        GameObject Lad = presentBois[Random.Range(0, 3)];
        Instantiate(Lad, spawnPos, Quaternion.identity);

        //Get the total amount of fellas
        totalBois = GameObject.FindGameObjectsWithTag("Dude").Length;

        Lad.GetComponent<Person>().SetPresents(1);

        //Tell the timer to count
        timeRunning = true;
        yield return new WaitForSeconds(2f);
    }

    //Double Present Individuals spawn
    IEnumerator SpawnBois2(int num)
    {
        Vector2 spawnPos;
        Vector2[] bois = new Vector2[num];

        //Spawn num of bois at random positions
        for (int i = 0; i < num; i++)
        {
            spawnPos = new Vector2(Random.Range(topLeft.x, bottomRight.x), Random.Range(bottomRight.y, topLeft.y));
            bois[i] = spawnPos;
            Instantiate(BasicBoi, spawnPos, Quaternion.identity);
        }

        //Check if the generated coordinate is not where someone else is
        do
        {
            spawnPos = new Vector2(Random.Range(topLeft.x, bottomRight.x), Random.Range(bottomRight.y, topLeft.y));
        } while (System.Array.IndexOf(bois, spawnPos) != -1);

        //Spawn a Present Individual
        GameObject Lad = presentBois[Random.Range(0, 3)];
        Instantiate(Lad, spawnPos, Quaternion.identity);

        //Fix the array of bois to include new boi
        Vector2[] allbois = new Vector2[num + 1];

        for (int i = 0; i < num; i++)
        {
            allbois[i] = new Vector2(bois[i].x, bois[i].y);
        }
        allbois[allbois.Length - 1] = spawnPos;

        //Check if the generated coordinate is not where someone else is
        do
        {
            spawnPos = new Vector2(Random.Range(topLeft.x, bottomRight.x), Random.Range(bottomRight.y, topLeft.y));
        } while (System.Array.IndexOf(allbois, spawnPos) != -1);

        //Spawn a Present Individual
        GameObject Lad2 = presentBois[Random.Range(0, 3)];
        Instantiate(Lad, spawnPos, Quaternion.identity);

        //Get the total amount of fellas
        totalBois = GameObject.FindGameObjectsWithTag("Dude").Length;

        Lad2.GetComponent<Person>().SetPresents(2);

        //Tell the timer to count
        timeRunning = true;
        yield return new WaitForSeconds(2f);
    }

}
