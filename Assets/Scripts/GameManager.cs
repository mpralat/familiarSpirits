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
using System.Collections;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    private const int QUESTIONS_PER_PLAYER = 13;

    private static bool initialized = false;
    
    // Main Panel stuff
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;
    public TextAsset questionsJson;
    public CanvasGroup questionPanelGroup;
    private List<Question> questions;
    
    // Result Panel Stuff
    public GameObject resultPanel;
    public Image resultImage;
    public TextMeshProUGUI resultText;
    public RawImage qrCodeImage;
    public Image QRCodeBackground;
    public Button ShowQRCodeButton;
    public bool ShowQRCode = false;
    
    private int currentQuestionIndex = 0;
    private ScoreManager scoreManager;
	private UrlManager linkManager;
	bool isProcessing = false;
	Color selectedColor       = new Color(0.77f, 0.61f, 0.28f, 1f);
	Color defaultButtonColor  = new Color(0.85f, 0.78f, 0.65f, 1f);
	Color defaultTextColor    = new Color(0.2f, 0.15f, 0.1f, 1f);
	float deselectedAlpha     = 0.7f;
	
	private FrameQuestion frameQuestion = new FrameQuestion(
	text: "Twoja wieś organizuje festyn na powitanie wiosny. Z tej okazji przygotowano cztery konkurencje, w jakich możesz się wykazać. W której z nich bierzesz udział?",
	answers: new FrameAnswer[]
	{
		new FrameAnswer("Strzelanie z łuku do celu.", "feathers"),
		new FrameAnswer("Gra nożowa.", "trees"),
		new FrameAnswer("Konkurs robienia gaików.", "flowers"),
		new FrameAnswer("Picie miodu na czas.", "mushrooms")
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
			ShowQRCodeButton.onClick.RemoveAllListeners();
			ShowQRCodeButton.onClick.AddListener(() => ToggleQRCode());
			resultImage.enabled = !ShowQRCode;
			qrCodeImage.enabled = ShowQRCode;
			QRCodeBackground.enabled = ShowQRCode;
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

	    isProcessing = false;
	    for (int i = 0; i < answerButtons.Length; i++)
		    answerButtons[i].interactable = true;

	    Question q = questions[currentQuestionIndex];
	    questionText.text = q.text;

	    for (int i = 0; i < answerButtons.Length; i++)
	    {
		    int idx = i;
		    var btn = answerButtons[i];
		    btn.GetComponentInChildren<TextMeshProUGUI>().text = q.answers[idx].text;
		    btn.onClick.RemoveAllListeners();
		    btn.onClick.AddListener(() => OnAnswerSelected(idx));
	    }
    }

    void OnAnswerSelected(int index)
    {
	    if (isProcessing) return;
	    isProcessing = true;

	    var chosenBtn = answerButtons[index];
	    chosenBtn.image.color = selectedColor;
	    var chosenLabel = chosenBtn.GetComponentInChildren<TextMeshProUGUI>();
	    chosenLabel.color = defaultButtonColor;

	    for (int i = 0; i < answerButtons.Length; i++)
	    {
		    if (i == index) continue;
		    var btn = answerButtons[i];
		    var c = btn.image.color;
		    c.a = deselectedAlpha;
		    btn.image.color = c;
		    btn.interactable = false;
	    }
	    
	    var answer = questions[currentQuestionIndex].answers[index];
	    scoreManager.AddPoints(
		    answer.firePoints,
		    answer.waterPoints,
		    answer.earthPoints,
		    answer.airPoints
	    );

	    Invoke(nameof(NextQuestion), 0.4f);
    }

    void NextQuestion()
    {
	    foreach (var btn in answerButtons)
	    {
		    btn.image.color = defaultButtonColor;
		    var lbl = btn.GetComponentInChildren<TextMeshProUGUI>();
		    lbl.color = defaultTextColor;
		    btn.transform.localScale = Vector3.one;
		    btn.interactable = true;
	    }

	    currentQuestionIndex++;
	    ShowQuestion();

	    EventSystem.current.SetSelectedGameObject(null);

	    var pointerData = new PointerEventData(EventSystem.current);
	    foreach (var btn in answerButtons)
	    {
		    ExecuteEvents.Execute<IPointerExitHandler>(
			    btn.gameObject,
			    pointerData,
			    ExecuteEvents.pointerExitHandler
		    );
	    }
	    isProcessing = false;
    }

	void ShowColorQuestion()
	{
	    // allow new selection
	    isProcessing = false;

	    foreach (var btn in answerButtons)
	    {
	        btn.interactable                     = true;
	        btn.image.color                      = defaultButtonColor;
	        btn.GetComponentInChildren<TextMeshProUGUI>().color = defaultTextColor;
	    }
	    
	    Spirit spirit = scoreManager.CurrentSpirit;
	    ColorQuestion colorQ = spirit.ColorQuestion;
	    questionText.text = colorQ.text;

	    for (int i = 0; i < answerButtons.Length; i++)
	    {
	        int idx = i;
	        var btn = answerButtons[i];
	        btn.GetComponentInChildren<TextMeshProUGUI>().text = colorQ.answers[idx].text;
	        btn.onClick.RemoveAllListeners();
	        btn.onClick.AddListener(() => OnColorAnswerSelected(idx));
	    }
	}

	void OnColorAnswerSelected(int index)
	{
	    if (isProcessing) return;
	    isProcessing = true;

	    var chosen = answerButtons[index];
	    chosen.image.color = selectedColor;
	    chosen.GetComponentInChildren<TextMeshProUGUI>().color = defaultButtonColor;

	    for (int i = 0; i < answerButtons.Length; i++)
	    {
	        if (i == index) continue;
	        var b = answerButtons[i];
	        var c = b.image.color;
	        c.a = deselectedAlpha;
	        b.image.color = c;
	        b.interactable = false;
	    }

	    var answer = scoreManager.CurrentSpirit.ColorQuestion.answers[index];
	    scoreManager.SetColor(answer.color);

	    Invoke(nameof(ShowFrameQuestion), 0.4f);
	}

	void ShowFrameQuestion() 
	{
	    isProcessing = false;
	    foreach (var btn in answerButtons)
	    {
	        btn.interactable = true;
	        btn.image.color = defaultButtonColor;
	        btn.GetComponentInChildren<TextMeshProUGUI>().color = defaultTextColor;
	    }
	    questionText.text = frameQuestion.text;

	    for (int i = 0; i < answerButtons.Length; i++)
	    {
	        int idx = i;
	        var btn = answerButtons[i];
	        btn.GetComponentInChildren<TextMeshProUGUI>().text = frameQuestion.answers[idx].text;
	        btn.onClick.RemoveAllListeners();
	        btn.onClick.AddListener(() => OnFrameAnswerSelected(idx));
	    }
	}

	void OnFrameAnswerSelected(int index)
	{
	    if (isProcessing) return;
	    isProcessing = true;

	    var chosen = answerButtons[index];
	    chosen.image.color = selectedColor;
	    chosen.GetComponentInChildren<TextMeshProUGUI>().color = defaultButtonColor;

	    for (int i = 0; i < answerButtons.Length; i++)
	    {
	        if (i == index) continue;
	        var b = answerButtons[i];
	        var c = b.image.color;
	        c.a = deselectedAlpha;
	        b.image.color = c;
	        b.interactable = false;
	    }

	    scoreManager.SetFrame(frameQuestion.answers[index].frameName);
	    Invoke(nameof(ShowResult), 0.4f);
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
		resultImage.sprite = Resources.Load<Sprite>($"Spirits/{fileName}");
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

    public void ToggleQRCode()
    {
	    ShowQRCode = !ShowQRCode;
	    resultImage.enabled = !ShowQRCode;
	    qrCodeImage.enabled = ShowQRCode;
	    QRCodeBackground.enabled = ShowQRCode;

	    ShowQRCodeButton.GetComponentInChildren<TextMeshProUGUI>().text =
		    ShowQRCode ? "Pokaż Chowańca" : "Zabierz Chowańca ze sobą";
    }
}