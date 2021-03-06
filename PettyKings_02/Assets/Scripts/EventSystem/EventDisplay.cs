﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public delegate void ButtonDel(int choice);


public class EventDisplay : MonoBehaviour {


    // A static reference to itself so other scripts can access it
    public static EventDisplay eventDisplay;


    // List of buttons of choices
    private List<Button> buttons_ = new List<Button>();


    public GameObject prefabButton;


    public GameObject bounceDisplay;


    public Texture diplomacyLogo_;
    public Texture domesticLogo_;
    public Texture religionLogo_;
    public Texture warLogo_;


    // References to parts of event screen for displaying data
    public Text nameText_;

    private GameObject victoryScreen_;
    public Texture victoryTexture_;
    public Texture defeatTexture_;
    public GameObject eventWindow_;
    public GameObject front_;
    public Text descriptionText_;
    public RawImage artworkImage_;
    public RawImage decisionTypeLogo_;
    public Image timerBar_;

    // Button variable
    public float btnSizeX = 500;
    public float btnSizeY = 50;
    public float btnPosY  = -300;
    public float btnPosX  = 0;

    // Default timer setting
    public float defaultTimerLength_;


    private Timer displayTimer_;
    private SoundPlay btnSoundLeft_;
    private SoundPlay btnSoundRight_;

    public float startTimerFlash_;
    float timerBarColourChanger_ = 2.0f;

    // When object is created
    void Awake()
    {

        // Check if an eventDisplay already exists
        if (eventDisplay == null)
        {

            // If not set the static reference to this object
            eventDisplay = this;
        }
        else if (eventDisplay != this)
        {

            // Else a different eventDisplay already exists destroy this object
            Destroy(gameObject);
        }
    }



    // Called when script is destroyed
    void OnDestroy()
    {
        if (eventDisplay == this)
        {
            // when destroyed remove static reference to itself
            eventDisplay = null;
        }
    }




    // Method called when object created
    void Start ()
    {

        victoryScreen_ = GameObject.Find("VictoryScreen");
        victoryScreen_.SetActive(false);

        buttons_.Add(GameObject.Find("DecisionBoxLeft").GetComponent<Button>());
        buttons_.Add(GameObject.Find("DecisionBoxRight").GetComponent<Button>());

        btnSoundLeft_ = GameObject.Find("DecisionBoxLeft").GetComponent<SoundPlay>();
        btnSoundRight_ = GameObject.Find("DecisionBoxRight").GetComponent<SoundPlay>();

        // Event display is not active when created
        gameObject.SetActive(false);
	}
    

    public void Display(EventDisplayData displayData, Timer newTimer)
    {
        // Set all display elements with data from event
        nameText_.text = displayData.name_;
        descriptionText_.text = displayData.description_;
        artworkImage_.texture = displayData.artwork_;

        switch (displayData.type_)
        {
            case DECISIONTYPE.DIPLOMACY:
                decisionTypeLogo_.texture = diplomacyLogo_;
                break;
            case DECISIONTYPE.DOMESTIC:
                decisionTypeLogo_.texture = domesticLogo_;
                break;
            case DECISIONTYPE.RELIGION:
                decisionTypeLogo_.texture = religionLogo_;
                break;
            case DECISIONTYPE.WAR:
                decisionTypeLogo_.texture = warLogo_;
                break;
            default:
                decisionTypeLogo_.texture = null;
                break;
        }
        //decisionTypeLogo_.texture = displayData.decisionLogo_;

        // Create the buttons
        CreateButtons(displayData);

        displayTimer_ = newTimer;
        displayTimer_.Pause();

        StartCoroutine(DisplayStart());
    }

    IEnumerator DisplayStart()
    {
        front_.SetActive(false);
        ShowButtons(false);
        //eventWindow_.transform.eulerAngles = new Vector3(0, 180, 0);
        float speed = 200;
        front_.SetActive(true);

        displayTimer_.Start();

        yield return new WaitForSeconds(0.5f);
        /*
        bool animFinished = false;
        while (!animFinished)
        {

            yield return null;
            eventWindow_.transform.eulerAngles += new Vector3(0, speed * Time.deltaTime, 0);

            //Debug.Log(eventWindow_.transform.eulerAngles);
            if (eventWindow_.transform.eulerAngles.y >= 270)
            {
                front_.SetActive(true);
            }

            if (eventWindow_.transform.eulerAngles.y <= speed * Time.deltaTime && eventWindow_.transform.eulerAngles.y >= -speed * Time.deltaTime)
            {
                eventWindow_.transform.eulerAngles = new Vector3(0, 0, 0);
                animFinished = true;
            }
        }
        */
        ShowButtons(true);
    }

    public void DisplayEnd()
    {
        GameObject oldDisplay = Instantiate(bounceDisplay);

        GameObject oldWindow = Instantiate(eventWindow_);

        oldWindow.transform.position = eventWindow_.transform.position;

        oldWindow.transform.SetParent(oldDisplay.transform);

        oldDisplay.transform.SetParent(transform.parent);



        ClearButtons();
        ShowButtons(false);

    }


    private void ClearButtons()
    {

        for (int i = 0; i < buttons_.Count; i++)
        {

            buttons_[i].GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    // Creates buttons for event display
    private void CreateButtons(EventDisplayData displayData)
    {

        // Destroy previous buttons
        //DestroyButtons();

        // If there are buttons to create
        if (displayData.btnFunctions_.Length > 0)
        {

            // Loop for each button to create
            for (int i = 0; i < displayData.btnFunctions_.Length; i++)
            {
                /*
                // Create new button
                GameObject newButton = (GameObject)Instantiate(prefabButton);

                // Get button's transform componenet
                RectTransform btnTransform = newButton.GetComponent<RectTransform>();

                // Setup button's transform position/size/anchor/etc...
                btnTransform.SetParent(GetComponent<RectTransform>(), false);
                btnTransform.localScale = new Vector3(1, 1, 1);
                btnTransform.sizeDelta = new Vector2(btnSizeX / displayData.btnFunctions_.Length, btnSizeY);
                btnTransform.anchorMax = new Vector2(0, 1);
                btnTransform.anchorMin = new Vector2(0, 1);
                btnTransform.pivot = new Vector2(0, 1);
                btnTransform.anchoredPosition = new Vector3(btnPosX + i * btnTransform.sizeDelta.x, btnPosY, 0);
                */
                // Set button text

                if (i < 2)
                {
                    if (i < displayData.btnText_.Length)
                    {
                        buttons_[i].GetComponentInChildren<Text>().text = displayData.btnText_[i];
                    }
                    else
                    {
                        buttons_[i].GetComponentInChildren<Text>().text = "option " + (i + 1);
                    }
                    // Create method for button OnClick from function pointer in display data
                    buttons_[i].GetComponent<Button>().onClick.RemoveAllListeners();
                    int tempInt = i;
                    ButtonDel tempButtonDel = displayData.btnFunctions_[i];

                    if(i == 0)
                    {
                        tempButtonDel += btnSoundLeft_.Play;
                    }
                    else
                    {
                        tempButtonDel += btnSoundRight_.Play; 
                    }

                    buttons_[i].GetComponent<Button>().onClick.AddListener(() => tempButtonDel(tempInt));

                    // Add new button to button list
                    //buttons_.Add(newButton.GetComponent<Button>());
                }
                
            }
        }
    }

    void DestroyButtons()
    {

        // Loop for each button in the list of buttons and destroy the button
        for (int i = 0; i < buttons_.Count; i++)
        {
            if (buttons_[i].gameObject)
            {
                Destroy(buttons_[i].gameObject);
            }
        }

        // After all the buttons are destroyed clear the list of buttons
        buttons_.Clear();
    }

    void ShowButtons(bool value)
    {
        foreach(Button btn in buttons_)
        {
            btn.gameObject.SetActive(value);
        }
    }

    public void UpdateTimerBar(float percentage)
    {
        timerBar_.fillAmount = percentage;
        timerBar_.color = Color.white;
        
        if(percentage < startTimerFlash_ && timerBarColourChanger_ > 1.0f)
        {
            timerBar_.color = Color.red;
            timerBarColourChanger_ -= Time.deltaTime;
        }

        if(timerBarColourChanger_ < 1.0f)
        {
            timerBar_.color = Color.white;
            timerBarColourChanger_ -= Time.deltaTime;
        }

        if(timerBarColourChanger_ < 0.0f)
        {
            timerBarColourChanger_ = 2.0f;
        }
    }
        


    public void DisplayVictory(bool victory)
    {
        if (victory)
        {
            victoryScreen_.GetComponentInChildren<RawImage>().texture = defeatTexture_;
        }
        else
        {
            victoryScreen_.GetComponentInChildren<RawImage>().texture = victoryTexture_;
        }
        victoryScreen_.SetActive(true);
    }

    // Return pointer to the event's timer
    public Timer GetTimer()
    {
        return displayTimer_;
    }

}
