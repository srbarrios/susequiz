using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public Text emailText;
    MailAddress mailAddress;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClick() 
    {
        // Check e-mail format
        if (IsValidEmail(emailText.text)) {
            GameObject.Find("QuizMgr").GetComponent<QuizMgr>().LoadGame(mailAddress);
        }
    }
    bool IsValidEmail(string email)
    {
        try
        {
            mailAddress = new System.Net.Mail.MailAddress(email);
            return mailAddress.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
