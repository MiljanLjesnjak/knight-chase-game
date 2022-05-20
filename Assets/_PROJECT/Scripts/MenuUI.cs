using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{

    [SerializeField]
    GameObject settings_panel = default;

    [SerializeField]
    GameObject toggleButton_image = default;

    [SerializeField]
    Toggle sound_mute_toggle = default;

    [SerializeField]
    Toggle show_moves_toggle = default;

    public static int input_type = 0;
    public static int mute_sound = 0;
    public static int show_moves = 0;

    Vector3 imgPos1 = new Vector3(-50, 0, 0);
    Vector3 imgPos2 = new Vector3(50, 0, 0);

    [SerializeField]
    Button[] themeButtons = default;

    [SerializeField]
    AudioSource audioSource = default;

    Color buttonColor = new Color(0, 0, 0, 0.25f);
    Color activeButtonColor = new Color(0, 0, 0, 0.75f);


    [Header("Materials")]
    [SerializeField]
    Material field_light = default;
    [SerializeField]
    Material field_dark = default;
    [SerializeField]
    Material ground = default;

    [Header("Textures")]
    [SerializeField]
    Texture[] textures = default;

    private void Awake()
    {
        //Application.targetFrameRate = 60;

        //Screen.sleepTimeout = SleepTimeout.NeverSleep;

        #region Settings_ControllType
        input_type = PlayerPrefs.GetInt("input_type", 1);
        if (input_type == 0)
            toggleButton_image.transform.localPosition = imgPos1;
        if (input_type == 1)
            toggleButton_image.transform.localPosition = imgPos2;
        #endregion

        #region Settings_MuteSound
        mute_sound = PlayerPrefs.GetInt("mute_sound", 0);
        if (mute_sound == 1)
        {
            sound_mute_toggle.isOn = true;
            audioSource.volume = 0;
        }
        else
        {
            sound_mute_toggle.isOn = false;
            audioSource.volume = 1;
        }
        #endregion

        #region Show_Possible_Moves
        show_moves = PlayerPrefs.GetInt("show_moves", 1);
        if(show_moves == 1)
        {
            show_moves_toggle.isOn = true;
        }
        else
        {
            show_moves_toggle.isOn = false;
        }
        #endregion

        #region HighlightActiveThemeButton
        for (int i = 0; i < themeButtons.Length; i++)
        {
            if (i == PlayerPrefs.GetInt("theme_index", 0) / 3)
                themeButtons[i].GetComponent<Image>().color = activeButtonColor;
            else
                themeButtons[i].GetComponent<Image>().color = buttonColor;
        }
        #endregion


        //Checks if first time playing (tutorial)
        //if (PlayerPrefs.GetInt("first_time", 0) == 0)
        //{
        //    SceneManager.LoadSceneAsync("TutorialsScene");
        //}

        setSkins();
    }

    private void Update()
    {
        //Back button
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settings_panel.activeSelf)
                closeSettingsPanel();
            else
                Application.Quit();
        }
    }

    //Click sound
    public void playClickSound()
    {
        audioSource.Play();
    }

    //Scene changing
    public void loadLevel()
    {
        PlayerPrefs.Save();
        SceneManager.LoadSceneAsync("LevelsScene");
    }
    
    public void loadTutorial()
    {
        field_dark.SetTexture("_BaseMap", textures[0]);
        field_light.SetTexture("_BaseMap", textures[1]);
        ground.SetTexture("_BaseMap", textures[2]);

        SceneManager.LoadSceneAsync(2);
    }

    //Opening and closing the panels
    public void openSettingsPanel()
    {
        if (!settings_panel.activeSelf)
        {
            settings_panel.SetActive(true);
        }

        else
        {
            settings_panel.SetActive(false);
        }
    }

    public void closeSettingsPanel()
    {
        settings_panel.SetActive(false);

    }

    //Touch Control Button
    public void Change_control_type()
    {
        //change text pos
        if (input_type == 0)
        {
            toggleButton_image.transform.localPosition = imgPos2;
        }
        if (input_type == 1)
        {
            toggleButton_image.transform.localPosition = imgPos1;
        }

        //change value
        if (input_type == 0)
            input_type = 1;
        else if (input_type == 1)
            input_type = 0;



        PlayerPrefs.SetInt("input_type", input_type);
    }


    //Themes
    public void changeTheme(int theme_index)
    {
        PlayerPrefs.SetInt("theme_index", theme_index);

        for (int i = 0; i < themeButtons.Length; i++)
        {
            if (i == PlayerPrefs.GetInt("theme_index", 0) / 3)
                themeButtons[i].GetComponent<Image>().color = activeButtonColor;
            else
                themeButtons[i].GetComponent<Image>().color = buttonColor;
        }

        setSkins();
    }

    void setSkins()
    {
        int theme_index = PlayerPrefs.GetInt("theme_index", 0);

        field_dark.SetTexture("_BaseMap", textures[theme_index + 0]);
        field_light.SetTexture("_BaseMap", textures[theme_index + 1]);
        ground.SetTexture("_BaseMap", textures[theme_index + 2]);

    }

    //Sound mute toggle
    public void MuteSound(Toggle tgl)
    {
        //change value
        if (tgl.isOn)
        {
            mute_sound = 1;
            audioSource.volume = 0;
        }
        else
        {
            mute_sound = 0;
            audioSource.volume = 1;
        }

        PlayerPrefs.SetInt("mute_sound", mute_sound);
    }

    //Show possible moves toggle
    public void ShowPossibleMoves(Toggle tgl)
    {
        if (tgl.isOn)
            show_moves = 1;
        else
            show_moves = 0;

        PlayerPrefs.SetInt("show_moves", show_moves);
    }
}
