package com.suse.hackweek.quiz.entities;

import java.io.Serializable;

public class User implements Serializable {

	private static final long serialVersionUID = -305726463442998985L;

    private String mailAddress;

    private Question[] solvedQuestions;

    private int lives;

    public String getMailAddress() {
        return mailAddress;
    }

    public void setMailAddress(String mailAddress) {
        this.mailAddress = mailAddress;
    }

    public Question[] getSolvedQuestions() {
        return solvedQuestions;
    }

    public void setSolvedQuestions(Question[] solvedQuestions) {
        this.solvedQuestions = solvedQuestions;
    }

    public int getLives() {
        return lives;
    }

    public void setLives(int lives) {
        this.lives = lives;
    }

}
