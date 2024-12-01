using System.Collections;
using TMPro;
using UnityEngine;

public class Person : MonoBehaviour
{
    //Movement variables
    private float movementDuration = 2.0f;
    private float waitBeforeMoving;
    private bool hasArrived = false;
    public static float speed = 10f;

    //Present variables
    public bool present;
    public int presentValue;
    public static int Presents;
    private TextMeshProUGUI presentsLeft;

    //Present giving cooldown
    public bool cooldown;
    private float pauseTime = 2.5f;

    //To keep track of coroutines called
    private Coroutine lastRoutine;
    
    //Animator for movement
    public Animator anim;

    //Present boi obtaining
    public GameObject[] presentBois = new GameObject[3];
    private Transform spawnPos;

    //Access to change score
    private GameController control;
    private int score = 10;

    //Walking sound
    public AudioSource walking;

    private void Start()
    {
        //Get UI Text for presents remaining
        presentsLeft = GameObject.FindGameObjectWithTag("PresentsNum").GetComponent<TextMeshProUGUI>();
        control = Camera.main.GetComponent<GameController>();
    }

    private void Update()
    {
        presentsLeft.text = Presents.ToString();
        //If they have no destination and the game has started
        if (!hasArrived && GameController.GameStart)
        {
            hasArrived = true;
            float randX = Random.Range(-2.5f, 5f);
            float randY = Random.Range(-4f, 4.5f);
            //Generate random location and move to it
            if (present)
            {
                lastRoutine = StartCoroutine(MoveToPoint(new Vector3(randX, randY, 20f), speed));
            }
            else
            {
                lastRoutine = StartCoroutine(MoveToPoint(new Vector3(randX, randY, 10f), speed));
            }
            
        }

        if (Presents == 0)
        {
            control.roundDone();
        }

        if (cooldown)
        {
            StartCoroutine("coolOff");
        }
        
    }

    IEnumerator coolOff()
    {
        yield return new WaitForSeconds(2.5f);
        cooldown = false;
    }

    //Remove presents on person capture
    public void RemovePresents()
    {
        Presents -= 1;
        control.addScore(score);
    }

    public void SetPresents(int num)
    {
        Presents = num;
    }

    //Detect on collision bump
    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject thing = collision.gameObject;
        //If it bumps into another person, the current person has a present, and the other one doesn't
        if ((thing.tag == "Dude") && (this.present == true) && thing.GetComponent<Person>().present != true && !cooldown)
        {
            Person thingP = thing.GetComponent<Person>();
            //Stop movement
            StopCoroutine(lastRoutine);
            thingP.StopCoroutine(thingP.lastRoutine);

            //Give them a present
            spawnPos = thing.transform;
            thingP.present = true;
            //Spawn present boi
            GameObject spawn = presentBois[Random.Range(0, 3)];
            GameObject newboi = Instantiate(spawn, spawnPos.position, Quaternion.identity);
            newboi.GetComponent<Person>().cooldown = true;
            cooldown = true;
            //Remove old boi
            Destroy(thing);
            Presents += 1;

            //Exchange period
            anim.SetFloat("Blend", 0);
            anim.SetBool("Left", false);
            anim.SetBool("Right", false);
            startExchange();

        }
    }

    //Movement Routine
    private IEnumerator MoveToPoint(Vector3 targetPos, float speed)
    {
        float timer = 0.0f;
        Vector3 startPos = transform.position;

        walking.Play(0);

        while (timer < movementDuration)
        {
            timer += Time.deltaTime;
            float t = timer / movementDuration;
            t = t * t * t * (t * (6f * t - 15f) + speed);
            transform.position = Vector3.Lerp(startPos, targetPos, t);

            if (targetPos.x < 0)
            {
                anim.SetFloat("Blend", -3);
            }
            else if (targetPos.x > 0)
            {
                anim.SetFloat("Blend", 5);
            }
            else
            {
                anim.SetFloat("Blend", 2);
            }

            yield return null;
        }
        waitBeforeMoving = Random.Range(0f, 8f);

        if (targetPos.x < startPos.x)
        {
            anim.SetFloat("Blend", 0);
            anim.SetBool("Left", true);
        }
        else if (targetPos.x > startPos.x)
        {
            anim.SetFloat("Blend", 0);
            anim.SetBool("Right", true);
        }
        else
        {
            anim.SetFloat("Blend", 0);
            anim.SetBool("Left", false);
            anim.SetBool("Right", false);
        }

        yield return new WaitForSeconds(waitBeforeMoving);
        hasArrived = false;
    }

    //Gift Exchange Routine
    private IEnumerator Exchange(float pauseTime)
    {
        yield return new WaitForSeconds(pauseTime);
        hasArrived = false;
    }

    public void startExchange()
    {
        StartCoroutine(Exchange(pauseTime));
    }
}
