using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KnightController : MonoBehaviour
{
    [SerializeField]
    GameObject knight_child = default;
    Rigidbody knight_rb;

    bool is_moving = false;

    public PlatformController PlatformController;
    public PossibleMovesHelp PossibleMoves;

    [SerializeField]
    [Range(0, 25)]
    float time_limit = 5;
    float timer = 0;

    [Header("Swipe Input")]
    int swipe_num = 1;
    Vector2 swipe_delta1 = Vector2.zero;
    Vector2 swipe_delta2 = Vector2.zero;

    [Header("Score")]
    int score_travel = 0, distance_traveled = 0, score_capture = 0;
    public static int score, best_score;
    public static bool record_broken;

    [Header("Sound")]
    [SerializeField]
    AudioSource clock_as = default;
    AudioSource knight_as = default;
    [SerializeField]
    AudioClip start_sound = default;
    [SerializeField]
    AudioClip move_sound = default, capture_sound = default;

    private void Awake()
    {
        //Application.targetFrameRate = 60;
        
        //Screen.sleepTimeout = SleepTimeout.NeverSleep;

        knight_as = GetComponent<AudioSource>();

        //Checks if a best score exists
        best_score = PlayerPrefs.GetInt("bestScore", 0);
        if (best_score == 0)
        {
            record_broken = false;
        }
        else
        {
            record_broken = true;
        }

        score = 0;
    }

    private void Start()
    {
        //Starts input
        StopAllCoroutines();

        //if (MenuUI.input_type == 1)
        //{
        //    StartCoroutine(inputTouch());
        //}
        //else if (MenuUI.input_type == 0)
        //{
        //    StartCoroutine(inputSwipe());
        //}
        //else
        //{
        //    StartCoroutine(inputTouch());
        //}

        //Sound mute
        if (MenuUI.mute_sound == 1)
        {
            knight_as.volume = 0;
            clock_as.volume = 0;
        }
        else
        {
            knight_as.volume = 1;
            clock_as.volume = 1;
        }

        knight_as.PlayOneShot(start_sound);

        PossibleMoves.ShowPossibleMoves();
    }

    private void Update()
    {
        //WEBGL
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit_info;
            Ray screen_ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(screen_ray, out hit_info))
            {
                if (hit_info.collider.gameObject.CompareTag("Field") || hit_info.collider.CompareTag("Enemy"))
                {
                    knightMoveCheck(transform.position, hit_info.collider.transform.position);
                }
            }
        }

        if (score_travel > 5)
        {
            timer += Time.deltaTime;
            if (timer > time_limit)
            {
                if (clock_as.isPlaying == false)
                {
                    clock_as.Play();
                    timer = 0;
                }
            }
        }

        //End game (by clock)
        if (timer > time_limit && clock_as.isPlaying)
        {
            Death();
        }

    }

    //Checks if knight can move
    bool knightMoveCheck(Vector3 current_pos, Vector3 desired_pos)
    {
        current_pos = new Vector3(Mathf.Round(current_pos.x), 0, Mathf.Round(current_pos.z));
        desired_pos = new Vector3(Mathf.Round(desired_pos.x), 0, Mathf.Round(desired_pos.z));

        if (is_moving)
        {
            return false;
        }

        if ((Mathf.Abs(Mathf.Round(desired_pos.x) - current_pos.x) == 1 && Mathf.Abs(Mathf.Round(desired_pos.z) - current_pos.z) == 2)
         || (Mathf.Abs(Mathf.Round(desired_pos.x) - current_pos.x) == 2 && Mathf.Abs(Mathf.Round(desired_pos.z) - current_pos.z) == 1))
        {
            RaycastHit hitinfo;
            if (Physics.Raycast(desired_pos + Vector3.up, Vector3.down, out hitinfo, 5))
            {
                if (hitinfo.collider.gameObject.CompareTag("Field") || hitinfo.collider.CompareTag("Enemy"))
                {
                    StartCoroutine(moveKnight(current_pos, desired_pos));
                    return true;
                }
            }
        }

        return false;
    }


    int noi = 32;
    IEnumerator moveKnight(Vector3 current_pos, Vector3 desired_pos)
    {
        is_moving = true;

        PossibleMoves.HideAllPossibleMoves();

        yield return new WaitForSeconds(0.075f);

        //Knight Move
        for (int i = 0; i < noi; i++)
        {
            //Moves the knight
            transform.Translate(new Vector3(Mathf.Round(desired_pos.x) - current_pos.x, 0, 0) / noi);
            //Moves the level
            PlatformController.LevelTranslate(-(Mathf.Round(desired_pos.z) - current_pos.z) / noi);


            //Rotates knight according to X position
            knight_child.transform.rotation = Quaternion.Euler(0, Mathf.Lerp(knight_child.transform.rotation.y, 25f * -transform.position.x, i), 0);

            //Jumps
            if (i < noi / 2)
            {
                transform.Translate(0, 2f / noi / 2, 0);
            }

            else
            {
                transform.Translate(0, -2f / noi / 2, 0);
            }

            yield return null;
        }


        //Rounds up position values of the knight
        transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));

        
        //Score
        //Checks if knight captures a piece
        Collider[] col = Physics.OverlapSphere(transform.position + new Vector3(0, 0.5f, 0), 0.5f);
        foreach (Collider c in col)
        {

            if (c.gameObject.CompareTag("Enemy"))
            {
                knight_as.PlayOneShot(capture_sound);
                Destroy(c.transform.parent.gameObject);
                score_capture += 3;
            }

        }

        //Move sound
        knight_as.PlayOneShot(move_sound);

        distance_traveled += Mathf.RoundToInt(desired_pos.z) - Mathf.RoundToInt(current_pos.z);

        //Sets local travel record (score_travel) and stops timer (because player moved and it wasnt backwards)
        if (distance_traveled > score_travel)
        {
            score_travel = distance_traveled;

            timer = 0;
            if (clock_as.isPlaying)
            {
                clock_as.Stop();
            }
        }


        score = score_travel + score_capture;

        //Checks best score
        if (score > best_score)
        {
            best_score = score;
        }


        is_moving = false;

        PossibleMoves.ShowPossibleMoves();
    }

    //INPUT --------------------
    //Checks for touch input 
    IEnumerator inputTouch()
    {
        while (true)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    RaycastHit hit_info;
                    Ray screen_ray = Camera.main.ScreenPointToRay(touch.position);

                    if (Physics.Raycast(screen_ray, out hit_info))
                    {
                        if (hit_info.collider.gameObject.CompareTag("Field") || hit_info.collider.CompareTag("Enemy"))
                        {
                            knightMoveCheck(transform.position, hit_info.collider.transform.position);
                        }
                    }
                }
            }

            yield return null;
        }
    }




    //Checks for swipe input
    int x = 0;
    IEnumerator inputSwipe()
    {
        x++;
        while (true)
        {
            if (swipe_num < 1 || swipe_num > 2)
            {
                swipe_num = 1;
            }

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    if (swipe_num == 1)
                    {
                        StartCoroutine("swipeTimer");
                    }

                    if (swipe_num == 2)
                    {

                        StopCoroutine("swipeTimer");
                    }
                }

                if (touch.phase == TouchPhase.Moved)
                {
                    //First swipe
                    if (swipe_num == 1)
                    {
                        swipe_delta1 += touch.deltaPosition;
                    }
                    //Second swipe
                    if (swipe_num == 2)
                    {
                        swipe_delta2 += touch.deltaPosition;
                    }
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    if (swipe_num == 2)
                    {
                        swipe_delta1 = getTouchDirection(swipe_delta1);
                        swipe_delta2 = getTouchDirection(swipe_delta2);

                        swipe_delta1 *= 2;
                        swipe_delta1 += swipe_delta2;

                        if (swipe_delta1.x != 0 && swipe_delta1.y != 0)
                        {
                            knightMoveCheck(transform.position, new Vector3(transform.position.x + swipe_delta1.x, 0, transform.position.z + swipe_delta1.y));
                        }

                        else
                        {
                            resetSwipeInput();
                        }
                    }

                    swipe_num++;

                }

            }

            yield return null;
        }
    }

    void resetSwipeInput()
    {
        x--;
        StopAllCoroutines();

        swipe_num = 1;
        swipe_delta1 = Vector2.zero;
        swipe_delta2 = Vector2.zero;

        StartCoroutine("inputSwipe");
    }

    IEnumerator swipeTimer()
    {
        yield return new WaitForSeconds(0.5f);
        resetSwipeInput();
    }

    Vector2 getTouchDirection(Vector2 delta)
    {
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            if (delta.x > 0)
            {
                return new Vector2(1, 0);
            }

            if (delta.x < 0)
            {
                return new Vector2(-1, 0);
            }
        }

        if (Mathf.Abs(delta.x) < Mathf.Abs(delta.y))
        {
            if (delta.y > 0)
            {
                return new Vector2(0, 1);
            }

            if (delta.y < 0)
            {
                return new Vector2(0, -1);
            }
        }

        return Vector2.zero;
    }
    //--------------------------

    //Death detection and function
    private void OnTriggerStay(Collider col)
    {
        if (is_moving == false)
        {
            if (col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("Ground"))
            {
                Death();
            }
        }
    }

    bool is_dead = false;
    public void Death()
    {
        if (!is_dead)
        {
            is_dead = true;

            transform.GetComponent<PossibleMovesHelp>().HideAllPossibleMoves();

            knight_rb = gameObject.AddComponent<Rigidbody>();
            gameObject.GetComponent<MeshCollider>().isTrigger = false;

            knight_rb.AddForce(new Vector3(Random.Range(-1, 1), Random.Range(0, 1), Random.Range(-1, 1)) * 5f, ForceMode.Impulse);

            GetComponent<GameUI>().open_pause_panel();

            StopAllCoroutines();
            this.enabled = false;
        }
    }

}


