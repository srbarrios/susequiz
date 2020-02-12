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
    readonly string questionsURL = "https://raw.githubusercontent.com/srbarrios/susequiz/master/Assets/StreamingAssets/sampleQuestions.json";

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
    GameObject answer1Label;
    GameObject answer2Label;
    GameObject answer3Label;
    GameObject answer4Label;
    GameObject[] geekos;

    //Data
    UserData userData;
    Questions questions;
    Question currentQuestion;

    // Start is called before the first frame update
    void Start()
    {
        geekos = new GameObject[3];

        webManager = GameObject.Find("WebMgr");
        welcomePanel = GameObject.Find("WelcomePanel");
        loginPanel = GameObject.Find("LoginPanel");
        gamePanel = GameObject.Find("GamePanel");
        winPanel = GameObject.Find("WinPanel");
        losePanel = GameObject.Find("LosePanel");

        emailText = GameObject.Find("EmailText").GetComponent<Text>();

        questionLabel = GameObject.Find("QuestionLabel");
        answer1Label = GameObject.Find("Answer1Label");
        answer2Label = GameObject.Find("Answer2Label");
        answer3Label = GameObject.Find("Answer3Label");
        answer4Label = GameObject.Find("Answer4Label");

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
            RightAnswer();
        } else
        {
            WrongAnswer();
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

    private void RightAnswer()
    {
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

    private void Win()
    {
        // TODO: Send e-mail with user details to HR

        // Load Win Panel
        winPanel.SetActive(true);
        gamePanel.SetActive(false);
    }

    private void WrongAnswer()
    {
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
        List<string> answers = new List<string>();
        answers.Add(currentQuestion.answer);
        answers.AddRange(currentQuestion.wrongAnswers);
        List<string> shuffledAnswers = answers.OrderBy(x => Guid.NewGuid()).ToList();

        answer1Label.GetComponent<Text>().text = shuffledAnswers[0];
        answer2Label.GetComponent<Text>().text = shuffledAnswers[1];
        answer3Label.GetComponent<Text>().text = shuffledAnswers[2];
        answer4Label.GetComponent<Text>().text = shuffledAnswers[3];
    }

    private void SaveUserData()
    {
        // TODO: Connect to database and save user data
        print($"\nE-Mail:{userData.mailAddress}\nLives: {userData.lives}\nSolved Questions: {userData.solvedQuestions.Length}");
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
