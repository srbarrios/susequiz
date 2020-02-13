using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class QuizMgr : MonoBehaviour
{
    readonly static string questionsURL = "https://raw.githubusercontent.com/srbarrios/susequiz/master/Assets/StreamingAssets/sampleQuestions.json";
    readonly static Color32 lemonColor = new Color32(200, 250, 0, 255);
    readonly static Color32 greenColor = new Color32(52, 224, 96, 255);
    readonly static Color32 redColor = new Color32(255, 87, 87, 255);

    //Panels
    GameObject webManager;
    GameObject welcomePanel;
    GameObject loginPanel;
    GameObject gamePanel;
    GameObject winPanel;
    GameObject losePanel;

    //Login Panel
    Text emailText;

    //Game Panel
    GameObject questionLabel;
    GameObject[] answers;
    GameObject[] geekos;

    //Data
    UserData userData;
    Questions questions;
    Question currentQuestion;

    // Start is called before the first frame update
    void Start()
    {
        geekos = new GameObject[3];
        answers = new GameObject[4];

        webManager = GameObject.Find("WebMgr");
        welcomePanel = GameObject.Find("WelcomePanel");
        loginPanel = GameObject.Find("LoginPanel");
        gamePanel = GameObject.Find("GamePanel");
        winPanel = GameObject.Find("WinPanel");
        losePanel = GameObject.Find("LosePanel");

        emailText = GameObject.Find("EmailText").GetComponent<Text>();

        questionLabel = GameObject.Find("QuestionLabel");
        answers[0] = GameObject.Find("Answer1Label");
        answers[1] = GameObject.Find("Answer2Label");
        answers[2] = GameObject.Find("Answer3Label");
        answers[3] = GameObject.Find("Answer4Label");

        geekos[0] = GameObject.Find("Geeko1");
        geekos[1] = GameObject.Find("Geeko2");
        geekos[2] = GameObject.Find("Geeko3");

        //Show first panel (Welcome Panel)
        welcomePanel.SetActive(true);
        loginPanel.SetActive(false);
        gamePanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadGame(string mailAddress)
    {
        // TODO: Get user data from Database requesting by mailAddress
        //       Create a new user data if mailAddress not exist
        userData = new UserData(mailAddress);

        // string userDataFile = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "sampleUserData.json"));
        // userData = JsonUtility.FromJson<UserData>(userDataFile);

        // Get questions from GitHub repository
        Action<string> callbackQuestions = data =>
        {
            //Save questions in memory
            questions = JsonUtility.FromJson<Questions>(data);

            // Pre-load first question
            RefreshGeeckos();
            NextQuestion();

            // Load Game Panel
            gamePanel.SetActive(true);
            loginPanel.SetActive(false);
        };
        webManager.GetComponent<WebRequestMgr>().request(questionsURL, callbackQuestions);
    }

    public void OnLoginClick()
    {
        if (IsValidEmail(emailText.text))
        {
           LoadGame(emailText.text);
        }
    }

    public void OnAnswerClick(Text userAnswer)
    {
        if (currentQuestion.answer == userAnswer.text) 
        {
            StartCoroutine(RightAnswer());
        } else
        {
            StartCoroutine(WrongAnswer(userAnswer.text));
        }
    }

    private void RightAnswerAnimation(string rightAnswer)
    {
        foreach (GameObject answer in answers)
        {
            answer.GetComponentInParent<Button>().interactable = false;
            if (answer.GetComponent<Text>().text == rightAnswer)
            {
                answer.GetComponentInParent<Image>().color = lemonColor;
            }
        }
    }

    private void WrongAnswerAnimation(string rightAnswer, string wrongAnswer)
    {
        foreach (GameObject answer in answers) {
            answer.GetComponentInParent<Button>().interactable = false;
            if (answer.GetComponent<Text>().text == rightAnswer) 
            {
                print($"Green answer: {rightAnswer}");
                answer.GetComponentInParent<Image>().color = lemonColor;
            }
            if (answer.GetComponent<Text>().text == wrongAnswer) 
            { 
                print($"Red answer: {wrongAnswer}");
                answer.GetComponentInParent<Image>().color = redColor;
            }
        }
    }

    private void ResetButtonColors() {
        foreach (GameObject answer in answers)
        {
            answer.GetComponentInParent<Button>().interactable = true;
            answer.GetComponentInParent<Image>().color = greenColor;
        }
    }

    public void OnTryAgainClick()
    {
        // TODO: Reset UserData on Database
        userData.solvedQuestions = new Question[0];
        userData.lives = 3;
        SaveUserData();

        // Load Login Panel
        loginPanel.SetActive(true);
        losePanel.SetActive(false);
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            MailAddress mailAddress = new System.Net.Mail.MailAddress(email);
            return mailAddress.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private IEnumerator RightAnswer()
    {
        RightAnswerAnimation(currentQuestion.answer);
        yield return new WaitForEndOfFrame();
        ResetButtonColors();
        SolvedQuestion();
        SaveUserData();
        if (userData.solvedQuestions.Length >= 10)
        {
            Win();
        }
        else 
        {
            NextQuestion(); 
        }
    }
    private IEnumerator WrongAnswer(string userAnswer)
    {
        WrongAnswerAnimation(currentQuestion.answer, userAnswer);
        yield return new WaitForSeconds(2.0f);
        ResetButtonColors();
        if (userData.lives > 0)
        {
            LoseGeecko();
            SaveUserData();
            NextQuestion();
        }
        else
        {
            // Load Lose Panel
            losePanel.SetActive(true);
            gamePanel.SetActive(false);
        }
    }

    private void Win()
    {
        // TODO: Send e-mail with user details to HR

        // Load Win Panel
        winPanel.SetActive(true);
        gamePanel.SetActive(false);
    }

    private void SolvedQuestion()
    {
        List<Question> currentSolvedQuestions = userData.solvedQuestions.ToList();
        currentSolvedQuestions.Add(currentQuestion);
        userData.solvedQuestions = currentSolvedQuestions.ToArray();
        Question[] remainingQuestions = questions.questions.Except(userData.solvedQuestions).ToArray();
        questions.questions = remainingQuestions;
    }

    private void NextQuestion()
    {
        if (questions.questions.Length == 0) Win();
        currentQuestion = questions.questions[UnityEngine.Random.Range(0, questions.questions.Length)];
        questionLabel.GetComponent<Text>().text = currentQuestion.question;
        List<string> nextAnswers = new List<string>();
        nextAnswers.Add(currentQuestion.answer);
        nextAnswers.AddRange(currentQuestion.wrongAnswers);
        List<string> shuffledAnswers = nextAnswers.OrderBy(x => Guid.NewGuid()).ToList();

        answers[0].GetComponent<Text>().text = shuffledAnswers[0];
        answers[1].GetComponent<Text>().text = shuffledAnswers[1];
        answers[2].GetComponent<Text>().text = shuffledAnswers[2];
        answers[3].GetComponent<Text>().text = shuffledAnswers[3];
    }

    private void SaveUserData()
    {
        // TODO: Connect to database and save user data
        // print($"\nE-Mail:{userData.mailAddress}\nLives: {userData.lives}\nSolved Questions: {userData.solvedQuestions.Length}");
    }

    private void LoseGeecko()
    {
        userData.lives--;
        RefreshGeeckos();
    }

    private void RefreshGeeckos()
    {
        for (int i = 0; i < geekos.Length; i++)
        {
            geekos[i].SetActive(userData.lives > i);
        }
    }
}
