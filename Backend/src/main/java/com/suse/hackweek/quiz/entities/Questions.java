package com.suse.hackweek.quiz.entities;

import java.io.Serializable;

public class Questions implements Serializable {

    private static final long serialVersionUID = -305726463442998985L;

    private Question[] questions;

    public Question[] getQuestions() {
        return questions;
    }

    public void setQuestions(Question[] questions) {
        this.questions = questions;
    }

}
