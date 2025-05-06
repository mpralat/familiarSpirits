using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const int QUESTIONS_PER_PLAYER = 13;

    private static bool initialized = false;
    
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
    private ScoreManager scoreManager;
    
    void Start()
    {
		if (!initialized)
        {
            initialized = true;
			this.scoreManager = new ScoreManager();
			this.scoreManager.LoadSpirits();
			LoadQuestions();
		}
        currentQuestionIndex = 0;
        resultPanel.SetActive(false);
		this.scoreManager.ResetPoints();
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
         	scoreManager.CalculateSpirit();
			ShowColorQuestion();
            return;
        }

        questionCounter.text = $"Pytanie {currentQuestionIndex + 1} z {questions.Count + 1}";

        Question q = questions[currentQuestionIndex];
        questionText.text = q.text;
        
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = q.answers[index].text;
            answerButtons[index].onClick.RemoveAllListeners();
            answerButtons[index].onClick.AddListener(() => OnAnswerSelected(index));
        }
    }

    void OnAnswerSelected(int index)
    {
        Answer answer = questions[currentQuestionIndex].answers[index];
        scoreManager.AddPoints(answer.firePoints, answer.waterPoints, answer.earthPoints, answer.airPoints);
        
        currentQuestionIndex++;
        ShowQuestion();
    }

	void ShowColorQuestion()
    {
        // last question should be a color question
        questionCounter.text = $"Pytanie {currentQuestionIndex + 1} z {questions.Count + 1}";
        
		Spirit spirit = scoreManager.CurrentSpirit;
		
		ColorQuestion colorQuestion = spirit.ColorQuestion;
        questionText.text = colorQuestion.text;
    
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = colorQuestion.answers[index].text;
            answerButtons[index].onClick.RemoveAllListeners();
            answerButtons[index].onClick.AddListener(() => OnColorAnswerSelected(index));
        }
		
        return;
    }
    
    void OnColorAnswerSelected(int index)
    {
		Spirit spirit = scoreManager.CurrentSpirit;
		ColorAnswer answer = spirit.ColorQuestion.answers[index];
        scoreManager.SetColor(answer.color);
        Debug.Log($"{answer.color}");
        ShowResult();
    }

    void ShowResult()
    {	
		Spirit resultSpirit = scoreManager.CurrentSpirit;
		resultPanel.SetActive(true);
        resultText.text = $"Twój chowaniec to: {resultSpirit.Name}";
        string resultImagePath = scoreManager.GetFileName(resultSpirit.Name);
		Debug.Log($"image path: {resultImagePath}");
        resultImage.sprite = Resources.Load<Sprite>(resultImagePath);
    }
    
    public void ResetGame()
    {
        Start();
    }
}