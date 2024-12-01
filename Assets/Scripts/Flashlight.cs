using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Flashlight : MonoBehaviour
{
    private Camera main;

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    private Transform light;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

    //Battery variables
    public Image battery;
    private int batCharge = 6;
    private float batteryLife = 12f;
    private bool restart;

    //Click Sound
    public AudioSource clicking;

    //Burn out
    public AudioClip dead;

    //Sprites for destroyed presents
    public GameObject[] destroyed = new GameObject[3];

    public bool click = true;

    //Battery state sprites
    public Sprite empty;
    public Sprite low2;
    public Sprite low1;
    public Sprite half;
    public Sprite almostHalf;
    public Sprite almostFull;
    public Sprite full;

    Vector3 points;

    // Start is called before the first frame update
    void Start()
    {
        main = Camera.main;
        light = GetComponent<Transform>();
    }

    //Function to turn light on
    public void lightOn()
    {
        StartCoroutine("Battery");
    }

    // Update is called once per frame
    void Update()
    {
        //Move flashlight with mouse
        Vector3 temp = Input.mousePosition;
        temp.x += 1;
        temp.y -= 7;
        temp.z = 5f;
        this.transform.position = main.ScreenToWorldPoint(temp);


        //When battery is empty, restart the battery
        if (batCharge == 0)
        {
            StopCoroutine("Battery");
            StartCoroutine("Recharge");

        }

        //When flashlight is recharged, turn battery back on
        if (restart)
        {
            StopCoroutine("Recharge");
            clicking.Play(0);
            restart = false;
            StartCoroutine("Battery");
        }

        //On click events
        if (Input.GetMouseButtonDown(0) && click)
        {
            Vector2 ray = main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero);

            //When object clicked is a Present bearing individual
            if (hit && hit.collider.gameObject.tag == "Dude" && hit.collider.gameObject.GetComponent<Person>().present)
            {
                //Obtain him, and remove the gift from the pool
                GameObject dude = hit.collider.gameObject;
                dude.GetComponent<Person>().RemovePresents();

                //Obtain his specific present
                int present = dude.GetComponent<Person>().presentValue;

                //And his location
                Vector2 pos = dude.transform.position;

                //MURDER HIM
                Destroy(dude);

                //Spawn a mess
                Instantiate(destroyed[present - 1], pos, Quaternion.identity);

                //Remove him from the list of existence
                main.GetComponent<GameController>().totalBois -= 1;
            }

            //When object clicked is a Battery
            if (hit && hit.collider.gameObject.tag == "BatPickup" && batteryLife != 12f)
            {
                //Stop the battery
                StopCoroutine("Battery");

                //Give life and charge to it
                if (batteryLife % 2 == 0)
                {
                    batteryLife += 2f;
                }
                else
                {
                    batteryLife += 3f;
                }
                batCharge += 1;

                //Destroy the lad
                Destroy(hit.collider.gameObject);

                //Sound or particle effect?
                StartCoroutine("Battery");
            }

            //When object is just an Individual
            if (hit && hit.collider.gameObject.tag == "Dude" && !hit.collider.gameObject.GetComponent<Person>().present)
            {
                //Get flickered
                clicking.Play(0);
                clicking.PlayOneShot(dead, 0.1f);
                main.GetComponent<GameController>().StartCoroutine("Mistake");
            }
        }
    }


    private void FixedUpdate()
    {
        //Keep track of battery charge
        switch (batteryLife)
        {
            case 0f:
                batCharge = 0;
                clicking.Play(0);
                clicking.PlayOneShot(dead, 0.1f);
                battery.overrideSprite = empty;
                light.localScale = new Vector3(0, 0, 1);
                break;

            case 2f:
                batCharge = 1;
                battery.overrideSprite = low2;
                light.localScale = new Vector3(5, 5, 1);
                break;

            case 4f:
                batCharge = 2;
                battery.overrideSprite = low1;
                light.localScale = new Vector3(7, 7, 1);
                break;

            case 6f:
                batCharge = 3;
                battery.overrideSprite = half;
                light.localScale = new Vector3(9, 9, 1);
                break;

            case 8f:
                batCharge = 4;
                battery.overrideSprite = almostHalf;
                light.localScale = new Vector3(11, 11, 1);
                break;

            case 10f:
                batCharge = 5;
                battery.overrideSprite = almostFull;
                light.localScale = new Vector3(13, 13, 1);
                break;

            case 12f:
                batCharge = 6;
                battery.overrideSprite = full;
                light.localScale = new Vector3(15, 15, 1);
                break;

            default:
                break;
        }
    }

    //Recharge Coroutine
    private IEnumerator Recharge()
    {
        yield return new WaitForSeconds(1f);
        batteryLife = 4f;
        yield return new WaitForSeconds(1f);
        batteryLife = 8f;
        yield return new WaitForSeconds(1f);
        batteryLife = 12f;
        restart = true;
    }

    //Coroutine that drains Battery overtime
    private IEnumerator Battery()
    {
        while (batteryLife != 0)
        {
            yield return new WaitForSeconds(3f);
            batteryLife -= 1f;
        }
        
    }

}
