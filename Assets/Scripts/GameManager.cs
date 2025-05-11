using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

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

    public VideoPlayer videoPlayer;  // VideoPlayer to play the WebM file
    public RawImage videoRawImage;   // RawImage to display the video
    
    private int currentQuestionIndex = 0;
    private ScoreManager scoreManager;

	private FrameQuestion frameQuestion = new FrameQuestion(
	text: "Jaka ramka?",
	answers: new FrameAnswer[]
	{
		new FrameAnswer("Piórka", "feathers"),
		new FrameAnswer("Drzewka", "branches"),
		new FrameAnswer("Kwiatki", "flowers"),
		new FrameAnswer("Grzybki", "mushrooms")
	}
);
    
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

        questionCounter.text = $"Pytanie {currentQuestionIndex + 1} z {questions.Count + 2}";

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
        questionCounter.text = $"Pytanie {currentQuestionIndex + 1} z {questions.Count + 2}";
        
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
        Debug.Log($"Kolorek: {answer.color}");
        ShowFrameQuestion();
    }

	void ShowFrameQuestion()
	{
		questionCounter.text = $"Pytanie {currentQuestionIndex + 1} z {questions.Count + 2}";
        questionText.text = frameQuestion.text;
    
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = frameQuestion.answers[index].text;
            answerButtons[index].onClick.RemoveAllListeners();
            answerButtons[index].onClick.AddListener(() => OnFrameAnswerSelected(index));
        }
	}

	void OnFrameAnswerSelected(int index)
    {
		scoreManager.SetFrame(frameQuestion.answers[index].frameName);
        ShowResult();
    }

    void ShowResult()
    {	
		Spirit resultSpirit = scoreManager.CurrentSpirit;
		resultPanel.SetActive(true);
        resultText.text = $"Twój chowaniec to: {resultSpirit.Name}";
        // string resultVideoPath = scoreManager.GetFileName(resultSpirit.Name);
		// Debug.Log($"video path: {resultVideoPath}");
        
		// Path to the video file (relative to the Resources folder)
        string resultVideoPath = "Ankluz_brown_feathers"; // No file extension
        string videoFilePath = "Assets/Resources/" + resultVideoPath + ".webm";  // If you want full path

        // Set the VideoPlayer source to the path
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = "file://" + videoFilePath;  // Assuming a path to a file on the disk

        // Set up the video to play on the RawImage
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += (source) =>
        {
            videoRawImage.texture = videoPlayer.texture;  // Assign the video texture to RawImage
            videoPlayer.Play();
        };
        videoPlayer.loopPointReached += OnLoopPointReached;

    }
    
    void OnLoopPointReached(VideoPlayer vp)
    {
        vp.frame = 0;  // Reset to the first frame
        vp.Play();  // Restart the video
    }
    
    public void ResetGame()
    {
        Start();
    }
}