using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizDispatcher : MonoBehaviour {

    // A conveniant feature of unity is the ability to declare default variables in editor, 
    // for this reason i have not used very much inheritance in this project, if this was
    // written in ue4 this would not be the case and i would be using alot more inheritance.

    public QuizData[] allQuizes = new QuizData[2];

    private int quizNum;

    public bool completed;

    public int score;

	public void quizStarted ()
    {
        if (completed == false)
        {
            QuizController.instance.BeginQuiz(getNextQuiz());
        }
    }

    public void complete ()
    {
        completed = true;
        this.GetComponent<MeshRenderer>().enabled = false;
    }

    public QuizData getNextQuiz()
    {
        quizNum++;
        if (quizNum - 1 > allQuizes.Length - 1) return null;
        return allQuizes[quizNum - 1];
    }

    [System.Serializable]
    public class QuizData {

        
        public QuizData (string QuestionIn,string[] optionsIn, int answerIn, QuizDispatcher originIn)
        {
            Question = QuestionIn;
            Options = optionsIn;
            rightAnswer = answerIn;
            origin = originIn;
        }
        

        public string Question;

        public string[] Options;

        public int rightAnswer;

        public QuizDispatcher origin;
    }
}


