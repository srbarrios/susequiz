[System.Serializable]
public class UserData
{
    public string mailAddress;
    public Question[] solvedQuestions;
    public int lives;
    public UserData(string mailAddress)
    {
        this.mailAddress = mailAddress;
        this.solvedQuestions = new Question[0];
        this.lives = 3;
    }
}
