using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using UnityEngine;
using UnityEngine.UI;

public class QuizMgr : MonoBehaviour
{
    GameObject welcomePanel;
    GameObject loginPanel;
    GameObject gamePanel;
    GameObject winPanel;
    GameObject losePanel;
    GameObject questionLabel;
    GameObject answer1Label;
    GameObject answer2Label;
    GameObject answer3Label;
    GameObject answer4Label;
    GameObject[] geeckos;
    UserData userData;
    Questions questions;
    Question currentQuestion;

    // Start is called before the first frame update
    void Start()
    {
        geeckos = new GameObject[3];

        welcomePanel = GameObject.Find("WelcomePanel");
        loginPanel = GameObject.Find("LoginPanel");
        gamePanel = GameObject.Find("GamePanel");
        winPanel = GameObject.Find("WinPanel");
        losePanel = GameObject.Find("LosePanel");

        questionLabel = GameObject.Find("QuestionLabel");
        answer1Label = GameObject.Find("Answer1Label");
        answer2Label = GameObject.Find("Answer2Label");
        answer3Label = GameObject.Find("Answer3Label");
        answer4Label = GameObject.Find("Answer4Label");

        geeckos[0] = GameObject.Find("Geecko1");
        geeckos[1] = GameObject.Find("Geecko2");
        geeckos[2] = GameObject.Find("Geecko3");

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

    public void LoadGame(MailAddress mailAddress)
    {

        // Get questions and user data
        string userDataFile = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "sampleUserData.json"));
        string questionsFile = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "sampleQuestions.json"));
        userData = JsonUtility.FromJson<UserData>(userDataFile);
        questions = JsonUtility.FromJson<Questions>(questionsFile);

        // Pre-load first question
        RefreshGeeckos();
        NextQuestion();

        // Load Game Panel
        gamePanel.SetActive(true);
        loginPanel.SetActive(false);
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

        // Load Login Panel
        loginPanel.SetActive(true);
        losePanel.SetActive(false);
    }

    private void RightAnswer()
    {
        SolvedQuestion();
        SaveUserData();
        if (userData.solvedQuestions.Length >= 10) 
        {
            // TODO: Send e-mail with user details to HR
            
            // Load Win Panel
            winPanel.SetActive(true);
            gamePanel.SetActive(false);
        } 
        else 
        {
            NextQuestion(); 
        }
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
        var currentSolvedQuestions = userData.solvedQuestions.ToList();
        currentSolvedQuestions.Add(currentQuestion);
        userData.solvedQuestions = currentSolvedQuestions.ToArray();
    }

    private void NextQuestion()
    {
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
        // TODO: Connect to database an save user data
        print("Lives: " + userData.lives);
        print("Solved Questions: " + userData.solvedQuestions.ToString());
    }

    private void LoseGeecko()
    {
        userData.lives--;
        RefreshGeeckos();
    }

    private void RefreshGeeckos()
    {
        for (int i = 0; i < geeckos.Length; i++)
        {
            geeckos[i].SetActive(userData.lives > i);
        }
    }
}
