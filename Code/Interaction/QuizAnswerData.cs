using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizAnswerData : MonoBehaviour {

    [SerializeField]
    private int answerNum;

    public void quizStarted()
    {     
        QuizController.instance.quizAnswerGiven(answerNum);
    }
}
