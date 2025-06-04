using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject first;
    public GameObject second;
    public GameObject third;

    //First menu buttons
    public Button playButton;
    public Button exitButton;

    //Second menu buttons
    public Button continueButton;
    public Button newGameButton;

    public Button back2Button;

    //Third menu button and input
    public Button startButton;
    public TMP_InputField seedInputField;
    public Button back3Button;
    public Button randomButton;

    //Scene 
    public Object mainScene;

    void Start()
    {
        // Generate a random 5-digit integer
        int randomSeed = Random.Range(10000, 99999);

        // Set the random seed as the default value in the seed input field
        seedInputField.text = randomSeed.ToString();

        // Add listeners to buttons
        playButton.onClick.AddListener(OnPlayButtonPressed);
        exitButton.onClick.AddListener(OnExitButtonPressed);
        //continueButton.onClick.AddListener(OnContinueButtonPressed);
        newGameButton.onClick.AddListener(OnNewGameButtonPressed);
        back2Button.onClick.AddListener(OnBack2ButtonPressed);
        startButton.onClick.AddListener(OnStartButtonPressed);
        back3Button.onClick.AddListener(OnBack3ButtonPressed);
    }

    void Update()
    {
        // Check if the random button is clicked
        if (randomButton != null && randomButton.onClick != null)
        {
            randomButton.onClick.AddListener(() =>
            {
                // Generate a random 5-digit integer
                int randomSeed = Random.Range(10000, 99999);

                // Set the random seed as the value in the seed input field
                seedInputField.text = randomSeed.ToString();
            });
        }
    }

    // Example methods for button presses
    void OnPlayButtonPressed()
    {
        //temporary fix :)
        SceneManager.LoadScene(mainScene.name);

        //first.SetActive(false);
        //second.SetActive(true);
    }

    void OnExitButtonPressed()
    {
        Application.Quit();
    }

    void OnContinueButtonPressed()
    {
        // Logic for continuing the game
    }

    void OnNewGameButtonPressed()
    {
        second.SetActive(false);
        third.SetActive(true);
    }

    void OnBack2ButtonPressed()
    {
        second.SetActive(false);
        first.SetActive(true);
    }

    void OnStartButtonPressed()
    {
        // Logic for starting a new game with the specified seed
        string seedText = seedInputField.text;
        if (int.TryParse(seedText, out int seed))
        {
            Debug.Log("Starting new game with seed: " + seed);
            SceneManager.LoadScene(mainScene.name);
        }
        else
        {
            Debug.LogError("Invalid seed input. Please enter a valid integer.");
        }
    }

    void OnBack3ButtonPressed()
    {
        third.SetActive(false);
        second.SetActive(true);
    }
}
