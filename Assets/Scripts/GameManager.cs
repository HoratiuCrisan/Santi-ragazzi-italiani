using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class AudioThreshold
    {
        public AudioSource audioSource;
        public int threshold;
        public bool isPlayed = false;
    }

    public TextMeshProUGUI sentenceText; // Text pentru propoziție
    public TextMeshProUGUI scoreText; // Text pentru scor
    public Button[] choiceButtons;
    public AudioSource clappingSound; // AudioSource for the clapping sound
    public AudioSource sadSound;
    public AudioSource backgroundSound; // AudioSource for the background sound

    private string currentSentence;
    private int currentWordIndex;
    private List<string> words;
    private int score;
    private Animator[] animators;
    public AudioThreshold[] audioThresholds;
    private bool gameEnded = false;

    private string[] sentences = new string[]
    {
        "Ciao amico, tutto bene?",
        "Che figo questo gioco!",
        "Andiamo al bar.",
        "Sei un grande!",
        "Mangiamo una pizza.",
        "Vieni qui, bro.",
        "Ho fame, tu?",
        "Che bella giornata!",
        "Andiamo in spiaggia.",
        "Ti va un gelato?",
        "Che figata!",
        "Sei pronto?",
        "Oggi è bello.",
        "Facciamo shopping?",
        "Sto bene, grazie.",
        "Andiamo al cinema.",
        "Che roba è?",
        "Sono stanco.",
        "Vuoi uscire?",
        "Ci vediamo dopo."
    };

    private int currentSentenceIndex;
    private string[] currentChoices; // Array pentru a ține minte cuvintele actuale

    void Start()
    {
        GameObject[] animatableObjects = GameObject.FindGameObjectsWithTag("Animatable");
        animators = new Animator[animatableObjects.Length];

        for (int i = 0; i < animatableObjects.Length; i++)
        {
            animators[i] = animatableObjects[i].GetComponent<Animator>();
        }

        currentSentenceIndex = 0;
        currentWordIndex = 0;
        currentSentence = "";
        score = 50;
        InitializeWords();
        UpdateChoices();
        UpdateScoreText();
    }

    void InitializeWords()
    {
        words = new List<string>();
        foreach (var sentence in sentences)
        {
            words.AddRange(sentence.Split(' '));
        }
    }

    public void ChooseWord(int index)
    {
        if (gameEnded) return; // Prevent interactions after game ends

        currentSentence += currentChoices[index] + " "; // Folosește cuvântul corect
        currentWordIndex++;

        // Stop any existing coroutine to reset the timer
        StopAllCoroutines();
        StartCoroutine(ScoreAndResetCoroutine(currentChoices[index]));

        UpdateText();
        UpdateChoices();
    }

    private IEnumerator ScoreAndResetCoroutine(string chosenWord)
    {
        yield return new WaitForSeconds(3); // Wait for 3 seconds

        if ((chosenWord.EndsWith(".") || chosenWord.EndsWith("!") || chosenWord.EndsWith("?")) && sentenceText.text.Length >= 2)
        {
            int increaseAmount = 5;
            score += increaseAmount;
            ActivateAnimation();
        }
        else
        {
            int decreaseAmount = 5;
            score -= decreaseAmount;
        }

        score = Mathf.Clamp(score, 0, 100); // Asigură-te că scorul rămâne între 0 și 100
        UpdateScoreText();

        if (score >= 100)
        {
            EndGame();
        } 
        else if (score <= 0)
        {
            EndBadGame();
        }
        else
        {
            // Reset the sentence after updating the score
            currentSentenceIndex++;
            currentWordIndex = 0;
            currentSentence = "";
            if (currentSentenceIndex >= sentences.Length)
            {
                currentSentenceIndex = 0; // Resetăm la prima propoziție dacă ajungem la final
            }
            UpdateText();
            UpdateChoices();
        }
    }

    private void EndGame()
    {
        gameEnded = true;
        clappingSound.Play();
        backgroundSound.Stop(); // Stop the background sound
        sentenceText.color = Color.green;
        sentenceText.text = "YOU WON!";
        // Disable choice buttons
        foreach (var button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }

        StartCoroutine(LoadSceneAfterDelay(8f));
    }

    private void EndBadGame()
    {
        gameEnded = true;
        backgroundSound.Stop();
        sadSound.Play();
        sentenceText.color = Color.red;
        sentenceText.text = "YOU LOST!";
        foreach (var button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }

        StartCoroutine(LoadSceneAfterDelay(8f)); 
    }

    IEnumerator LoadSceneAfterDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime); // Wait for n seconds
        SceneManager.LoadScene("SampleScene");
    }


    private void Update()
    {
        foreach (Animator animator in animators)
        {
            animator.SetFloat("Points", score);
        }

        foreach (AudioThreshold audioThreshold in audioThresholds)
        {
            if ((score == audioThreshold.threshold) && !audioThreshold.audioSource.isPlaying && !audioThreshold.isPlayed)
            {
                audioThreshold.audioSource.Play();
                audioThreshold.isPlayed = true;
            }
            else if ((score < audioThreshold.threshold || score > audioThreshold.threshold) && audioThreshold.audioSource.isPlaying)
            {
                audioThreshold.audioSource.Stop();
            }
        }
    }

    void UpdateText()
    {
        sentenceText.text = currentSentence;
    }

    void UpdateChoices()
    {
        if (gameEnded) return; // Prevent updating choices after game ends

        currentChoices = new string[choiceButtons.Length]; // Initializează array-ul de cuvinte actuale

        if (currentSentenceIndex >= sentences.Length)
        {
            foreach (var button in choiceButtons)
            {
                button.gameObject.SetActive(false);
            }
            return;
        }

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            int randomIndex = Random.Range(0, words.Count);
            currentChoices[i] = words[randomIndex]; // Salvează cuvântul în array
            choiceButtons[i].gameObject.SetActive(true);
            choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentChoices[i];
        }
    }

    void ActivateAnimation()
    {
        Debug.Log("Activate animation");
        // Aici adaugi logica pentru a activa animația din Krita
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
        // Schimbă culoarea textului pe măsură ce scorul scade de la verde la roșu
        float scorePercentage = score / 100f;
        Color scoreColor = Color.Lerp(Color.red, Color.green, scorePercentage);
        scoreText.color = scoreColor;
    }
}
