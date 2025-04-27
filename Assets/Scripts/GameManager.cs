using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const int QUESTIONS_PER_PLAYER = 10;
    
    // Main Panel stuff
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;
    public TextAsset questionsJson;

    public TextMeshProUGUI questionCounter;
    private List<Question> questions;
    
    // Result Panel Stuff
    public GameObject resultPanel;
    public Image resultImage;
    public TextMeshProUGUI resultText;
    
    private int currentQuestionIndex = 0;
    private ScoreManager scoreManager = new ScoreManager();
    
    public ColorQuestion sampleColorQuestion = new ColorQuestion {
        text = "Jaka wersja kolorystyczna wariacie?",
        answers = new ColorAnswer[] {
            new ColorAnswer { text = "Niebieski", color = "blue" },
            new ColorAnswer { text = "Zielony", color = "green" },
            new ColorAnswer { text = "Czerwony", color = "red" },
            new ColorAnswer { text = "Inny", color = "other" }
        }
    };
    
    void Start()
    {
        currentQuestionIndex = 0;
        resultPanel.SetActive(false);
		scoreManager.Start();
        LoadQuestions();
        ShowQuestion();
    }

    void LoadQuestions()
    {
        QuestionList loaded = JsonUtility.FromJson<QuestionList>(questionsJson.text);
        List<Question> shuffledQuestions = new List<Question>();
        
        foreach (var q in loaded.questions)
        {
            shuffledQuestions.Add(q);
        }
        shuffledQuestions = shuffledQuestions.OrderBy(q => UnityEngine.Random.value).ToList();
        // Take only the first QUESTIONS_PER_PLAYER
        questions = shuffledQuestions.Take(QUESTIONS_PER_PLAYER).ToList();
    }

    void ShowQuestion()
    {
        if (currentQuestionIndex >= questions.Count)
        {
            ShowColorQuestion();
            return;
        }

        questionCounter.text = $"Pytanie {currentQuestionIndex + 1} z {questions.Count + 1}";

        Question q = questions[currentQuestionIndex];
        Debug.Log($"Current question: {q.text}");
        questionText.text = q.text;
        
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = q.answers[index].text;
            answerButtons[index].onClick.RemoveAllListeners();
            answerButtons[index].onClick.AddListener(() => OnAnswerSelected(index));
        }
    }

    void ShowColorQuestion()
    {
        // last question should be a color question
        questionCounter.text = $"Pytanie {currentQuestionIndex + 1} z {questions.Count + 1}";
        
        questionText.text = sampleColorQuestion.text;
        
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = sampleColorQuestion.answers[index].text;
            answerButtons[index].onClick.RemoveAllListeners();
            answerButtons[index].onClick.AddListener(() => OnColorAnswerSelected(index));
        }
        return;
    }

    void OnAnswerSelected(int index)
    {
        Debug.Log($"Selected answer! Current index: {currentQuestionIndex}. Index: {index}");
        Answer answer = questions[currentQuestionIndex].answers[index];
        scoreManager.AddPoints(answer.firePoints, answer.waterPoints, answer.earthPoints, answer.airPoints);
        
        currentQuestionIndex++;
        ShowQuestion();
    }
    
    void OnColorAnswerSelected(int index)
    {
        ColorAnswer answer = sampleColorQuestion.answers[index];
        scoreManager.SetColor(answer.color);
        Debug.Log($"{answer.color}");
        ShowResult();
    }

    void ShowResult()
    {
        resultPanel.SetActive(true);
        Spirit resultSpirit = scoreManager.GetSpirit();
        resultText.text = $"Twój chowaniec to: {resultSpirit.Name}";
        string resultImagePath = scoreManager.GetFileName(resultSpirit.Name); 
        resultImage.sprite = Resources.Load<Sprite>(resultImagePath);
    }
    
    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}