using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
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
        questions = new List<Question>(loaded.questions);
        questions.Sort((a, b) => a.order.CompareTo(b.order));
    }

    void ShowQuestion()
    {
        if (currentQuestionIndex >= questions.Count)
        {
            ShowResult();
            return;
        }

        questionCounter.text = $"Pytanie {currentQuestionIndex + 1} z {questions.Count}";

        Question q = questions[currentQuestionIndex];
        Debug.Log($"Current question: {q.text}");
        questionText.text = q.text;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = q.answers[i].text;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }
    }

    void OnAnswerSelected(int index)
    {
        Debug.Log($"Selected answer! Current index: {currentQuestionIndex}. Index: {index}");
        Answer answer = questions[currentQuestionIndex].answers[index];
        scoreManager.AddPoints(answer.firePoints, answer.waterPoints, answer.earthPoints, answer.airPoints);
        
        currentQuestionIndex++;
        ShowQuestion();
    }

    void ShowResult()
    {
        resultPanel.SetActive(true);
        Spirit resultSpirit = scoreManager.GetSpirit();
        resultText.text = $"Twój chowaniec to: {resultSpirit.Name}";
        //resultImage.sprite = Resources.Load<Sprite>($"Spirits/{resultImageName}");
    }
    
    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}