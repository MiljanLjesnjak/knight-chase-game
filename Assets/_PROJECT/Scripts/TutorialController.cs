using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    GameObject knight_child = default;

    [Header("Sound")]
    [SerializeField]
    AudioSource knight_as = default;
    [SerializeField]
    AudioClip move_sound = default, capture_sound = default;

    [Header("Tutorial")]
    [SerializeField]
    Button ui_tutorial_button = default;
    [SerializeField]
    Text ui_tutorial_text = default;

    int step = 0;
    string[] tutorial_text = { "Welcome !\nThis short tutorial will teach you the basics to the game.\n\n[press here to start]\n",
                               "The knight is a chess piece, that may move two squares vertically and one square horizontally, or two squares horizontally and one square vertically\n(with both forming the shape of an L).\nWhile moving, the knight can jump over pieces to reach its destination.\nThe flashing red fields are marked as fields to which the knight can move.\n\n[press here to continue]\n",
                               "There are 2 types of input you can use:\nTouch and swipe input.\nYou can switch between them in the settings menu.\n\n[press here to continue]\n",
                               "When using the touch input, you press the field to which you want the knight to move, under condition that the knight can move to that field.\nGo ahead and try moving the knight to any field you like.\n\n[move the knight to continue]\n",
                               "To move using the swipe input, you have to swipe 2 times in a row.\n1st swipe indicates the direction in which the knight will move 2 spaces,\n and the 2nd the direction in which the knight will move 1 space to the side.\nFor the sake of this tutorial, you'll be using the touch input.\n\n[press here to continue]\n",
                               "You get 1 point for every field traveled, and 3 points for capturing a piece.\nBut beware, the pieces can also capture your knight.\nMove the knight to the rook to capture it.\n\n[capture the piece to continue]\n",
                               "There is also a time limit, so make sure you don't take too much time to advance.\nHurry up if you hear a clock ticking, as that means you have just a couple of seconds left.\nOtherwise the enemy knight will capture you.\n\n[press here to continue]\n",
                               "To pause the game, click the clock icon in the top-left corner.\nNow pause the game and use the pause menu to return to the main menu and finish the tutorial." };


    [Header("Step 1")]
    [SerializeField]
    Material flashing_red_mat = default;
    [SerializeField]
    Texture default_texture = default;
    [SerializeField]
    Texture flashing_red_texture = default;

    [Header("Step 5")]
    [SerializeField]
    GameObject enemy_rook = default;

    [Header("Step 6")]
    [SerializeField]
    GameObject pause_button = default;


    private void Awake()
    {
        Application.targetFrameRate = 60;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        knight_child = transform.GetChild(0).gameObject;
        knight_as = GetComponent<AudioSource>();

        flashing_red_mat.SetTexture("_BaseMap", default_texture);
    }

    private void Start()
    {
        StartCoroutine("TutorialStep0");
    }


    //TUTORIAL START----------------------------------
    public void TutorialNextStep()
    {
        StopCoroutine("TutorialStep" + step);
        step++;
        ui_tutorial_text.text = tutorial_text[step];
        StartCoroutine("TutorialStep" + step);
    }

    //Initial welcome
    IEnumerator TutorialStep0()
    {
        ui_tutorial_text.text = tutorial_text[step];
        yield return null;
    }

    //Where can the knight move
    IEnumerator TutorialStep1()
    {
        while (true)
        {
            //Fields flash in red color
            flashing_red_mat.SetTexture("_BaseMap", flashing_red_texture);

            yield return new WaitForSeconds(0.75f);

            flashing_red_mat.SetTexture("_BaseMap", default_texture);

            yield return new WaitForSeconds(0.75f);



            yield return null;
        }
    }

    //Types of input
    IEnumerator TutorialStep2()
    {
        flashing_red_mat.SetTexture("_BaseMap", default_texture);

        yield return null;
    }

    //Touch input, move the knight
    IEnumerator TutorialStep3()
    {
        ui_tutorial_button.interactable = false;

        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                RaycastHit hit_info;
                Ray screen_ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(screen_ray, out hit_info))
                {
                    if (hit_info.collider.CompareTag("Field"))
                    {
                        if ((Mathf.Abs(Mathf.Round(hit_info.collider.transform.position.x) - transform.position.x) == 1 && Mathf.Abs(Mathf.Round(hit_info.collider.transform.position.z) - transform.position.z) == 2)
                         || (Mathf.Abs(Mathf.Round(hit_info.collider.transform.position.x) - transform.position.x) == 2 && Mathf.Abs(Mathf.Round(hit_info.collider.transform.position.z) - transform.position.z) == 1))
                        {
                            knight_as.PlayOneShot(move_sound);

                            //Move the knight
                            transform.position = new Vector3(hit_info.collider.transform.position.x, 0.1f, hit_info.collider.transform.position.z);

                            ui_tutorial_button.interactable = true;

                            TutorialNextStep();

                            break;
                        }
                    }
                }
            }

            yield return null;
        }
    }

    //Swipe input
    IEnumerator TutorialStep4()
    {

        yield return null;

    }

    //Score and capture
    IEnumerator TutorialStep5()
    {
        ui_tutorial_button.interactable = false;
        enemy_rook.SetActive(true);

        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                RaycastHit hit_info;
                Ray screen_ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(screen_ray, out hit_info))
                {
                    if (hit_info.collider.CompareTag("Enemy"))
                    {
                        if ((Mathf.Abs(Mathf.Round(hit_info.collider.transform.position.x) - transform.position.x) == 1 && Mathf.Abs(Mathf.Round(hit_info.collider.transform.position.z) - transform.position.z) == 2)
                         || (Mathf.Abs(Mathf.Round(hit_info.collider.transform.position.x) - transform.position.x) == 2 && Mathf.Abs(Mathf.Round(hit_info.collider.transform.position.z) - transform.position.z) == 1))
                        {
                            enemy_rook.SetActive(false);
                            knight_as.PlayOneShot(capture_sound);

                            //Move the knight
                            transform.position = new Vector3(hit_info.collider.transform.position.x, 0.1f, hit_info.collider.transform.position.z);

                            ui_tutorial_button.interactable = true;

                            TutorialNextStep();

                        }
                    }
                }
            }

            yield return null;
        }
    }

    //Time limit
    IEnumerator TutorialStep6()
    {
        yield return null;
    }

    //Pause, final step
    IEnumerator TutorialStep7()
    {
        //Sets tutorial as done so it doesnt start again
        PlayerPrefs.SetInt("first_time", 1);

        pause_button.SetActive(true);
        ui_tutorial_button.GetComponent<Button>().enabled = false;



        yield return null;
    }



    //TUTORIAL END------------------------------------




}


