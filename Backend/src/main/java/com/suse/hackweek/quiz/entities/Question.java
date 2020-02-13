package com.suse.hackweek.quiz.entities;

import java.io.Serializable;

public class Question implements Serializable {

    private static final long serialVersionUID = -305726463442998985L;

    private String question;

    private String answer;

    private String[] wrongAnswers;

    public String getQuestion() {
        return question;
    }

    public void setQuestion(String question) {
        this.question = question;
    }

    public String getAnswer() {
        return answer;
    }

    public void setAnswer(String answer) {
        this.answer = answer;
    }

    public String[] getWrongAnswers() {
        return wrongAnswers;
    }

    public void setWrongAnswers(String[] wrongAnswers) {
        this.wrongAnswers = wrongAnswers;
    }

}
