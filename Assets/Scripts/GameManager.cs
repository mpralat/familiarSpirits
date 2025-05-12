using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using ZXing;
using ZXing.QrCode;
using ZXing.Common;

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
    public RawImage qrCodeImage;

    public VideoPlayer videoPlayer;
    public RawImage videoRawImage;
    
    private int currentQuestionIndex = 0;
    private ScoreManager scoreManager;
	private UrlManager linkManager;

	private FrameQuestion frameQuestion = new FrameQuestion(
	text: "Jaka ramka?",
	answers: new FrameAnswer[]
	{
		new FrameAnswer("Piórka", "feathers"),
		new FrameAnswer("Drzewka", "trees"),
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
			this.linkManager = new UrlManager();
			this.linkManager.LoadLinks();
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

		string fileName = scoreManager.GetFileName(resultSpirit.Name);
		Debug.Log(fileName);
		string gifUrl = linkManager.GetUrlByName($"{fileName}.gif");

		GenerateQRCode(gifUrl);

        string videoFilePath = $"Assets/Resources/Spirits/{fileName}.webm";
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = "file://" + videoFilePath;

        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += (source) =>
        {
            videoRawImage.texture = videoPlayer.texture;
            videoPlayer.Play();
        };
		// LOOP
        videoPlayer.loopPointReached += OnLoopPointReached;

    }
    
    void OnLoopPointReached(VideoPlayer vp)
    {
        vp.frame = 0;  // Reset to the first frame
        vp.Play();  // Restart the video
    }

	void GenerateQRCode(string text)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new ZXing.Common.EncodingOptions
            {
                Height = 256,
                Width = 256
            }
        };

        var color32 = writer.Write(text);
        Texture2D texture = new Texture2D(256, 256);
        texture.SetPixels32(color32);
        texture.Apply();

        qrCodeImage.texture = texture;
    }
    
    public void ResetGame()
    {
        Start();
    }
}