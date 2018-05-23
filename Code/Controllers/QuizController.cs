using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore.Examples.HelloAR;

// Author : Ciaran Rowles
// TODO: refactor fading controls into another class, would make code much nicer
// TODO: Use a different timing method then ienumerators, or simplify the code. its a bit of a mess honestly.
public class QuizController : MonoBehaviour
{

    public static QuizController instance;

    public bool quizStarted;

    [SerializeField]
    private TextMesh introText;

    [SerializeField]
    private string[] introTexts;

    [SerializeField]
    private TextMesh correctText;

    private int introTextNum;

    [SerializeField]
    private bool fading;

    public bool questionCurrentlyGoing;

    public bool quizAnswerGivenLoading;

    [SerializeField]
    private TextMesh quizDisplayQuestion;

    [SerializeField]
    private TextMesh[] quizDisplayAnswers;


    [SerializeField]
    private BoxCollider[] quizAnswerColliders;

    public QuizDispatcher.QuizData currentQuiz;

    [SerializeField]
    private Transform QuizColliderParent;

    private bool gameStartedOnce;

    // Enforce a single instance of this class. 
    // implimented as a monobehaviour component instead of making a static
    // singleton so i can interact with the scene easily.
    void Start()
    {
        if (FindObjectsOfType<QuizController>().Length == 1) DontDestroyOnLoad(this.gameObject);
        else Destroy(this.gameObject);
        if (this != null) instance = this;
        iTween.FadeTo(introText.gameObject, 0f, 0.01f);
        if (Application.isEditor)
            StartCoroutine(wait1SecondToBeginGameInEditor());
        fadeToQuiz(0, 0.01f);
        iTween.FadeTo(correctText.gameObject, 0f, 0.01f);
        foreach (BoxCollider col in quizAnswerColliders) col.enabled = false;
    }


    // Allows player to simulate a player deploying the scenery and walking into rome.
    IEnumerator wait1SecondToBeginGameInEditor()
    {
        yield return new WaitForSeconds(1.5f);
        beginGame();
    }

    public void BeginQuiz(QuizDispatcher.QuizData quizToDo)
    {
        foreach (BoxCollider col in quizAnswerColliders) col.enabled = true;
        Debug.Log("BEGIN QUIZZ : " + quizToDo.Question);
        currentQuiz = quizToDo;
        quizDisplayQuestion.text = currentQuiz.Question;
        for (int i = 0; i < currentQuiz.Options.Length; i++)
        {
            quizDisplayAnswers[i].text = currentQuiz.Options[i];
        }
        questionCurrentlyGoing = true;
        fadeToQuiz(1, 2);

    }

    // Publicly called when the user touches the screen.
    public void moveToNextIntroText()
    {
        if (UIController.canChangeIntroText == false) return;
        if (fading == false)
        {
            fading = true;
            StartCoroutine(callBackFunctionIn2Seconds("moveToNextIntroTextAfterFade"));
            iTween.FadeTo(introText.gameObject, 0, 2f);
        }
    }

    // Denies the user from skipping through the text.
    private IEnumerator callbackAfterFadeOut()
    {
        yield return new WaitForSeconds(2);
        fading = false;
    }

    public void moveToNextIntroTextAfterFade()
    {
        if (introTextNum < 4)
        {
            introTextNum++;
            if (introTextNum == 2 || introTextNum == 3) introText.fontSize = 100;
            else introText.fontSize = 200;
            introText.text = introTexts[introTextNum];
            iTween.FadeTo(introText.gameObject, 1, 2f);
        }
        else
        {
            quizStarted = true;
            iTween.FadeTo(introText.gameObject, 0, 2f);
        }
    }

    private IEnumerator callBackFunctionIn2Seconds(string callBack)
    {
        yield return new WaitForSeconds(2);
        this.gameObject.SendMessage(callBack);
        StartCoroutine("callbackAfterFadeOut");
    }

    public void beginGame()
    {
        if (gameStartedOnce) return;
        fading = true;
        Debug.Log("Begin Game!");
        gameStartedOnce = true;
        iTween.FadeTo(introText.gameObject, 1, 2f);
        StartCoroutine("callbackAfterFadeOut");
    }

    public void quizAnswerGiven(int answerNum)
    {
        if (quizAnswerGivenLoading == true) return;
        quizAnswerGivenLoading = true;
        Debug.Log("QUIZ ANSWER GIVEN" + answerNum);
        if (answerNum == currentQuiz.rightAnswer)
        {
            correctText.text = "CORRECT";
            currentQuiz.origin.score++;
        }
        else correctText.text = "FALSE";
        iTween.FadeTo(correctText.gameObject, 1, 2);
        fadeToQuiz(0, 2);
        StartCoroutine("waitToChangeToCorrectSign");
    }

    IEnumerator waitToChangeToCorrectSign()
    {
        yield return new WaitForSeconds(2);
        QuizDispatcher.QuizData lastQuiz = currentQuiz;
        currentQuiz = currentQuiz.origin.getNextQuiz();
        if (currentQuiz != null)
        {
            iTween.FadeTo(correctText.gameObject, 0, 2f);
            Debug.Log(currentQuiz.Question);
            quizDisplayQuestion.text = currentQuiz.Question;
            for (int i = 0; i < currentQuiz.Options.Length; i++)
            {
                quizDisplayAnswers[i].text = currentQuiz.Options[i];
            }
            StartCoroutine("waitToDisableCorrectSign");
        }
        else
        {
            foreach (BoxCollider col in quizAnswerColliders) col.enabled = false;
            iTween.FadeTo(correctText.gameObject, 0, 2f);
            lastQuiz.origin.complete();
            StartCoroutine(waitUntilCorrectIsGoneForScore(lastQuiz));
        }
    }

    IEnumerator waitUntilCorrectIsGoneForScore(QuizDispatcher.QuizData quiz)
    {
        yield return new WaitForSeconds(2);
        correctText.fontSize = 125;
        correctText.text = "YOU SCORED : " + quiz.origin.score + " OUT OF " + quiz.origin.allQuizes.Length;
        iTween.FadeTo(correctText.gameObject, 1, 2f);
        StartCoroutine("waitToDisableQuizScoreText");
    }

    IEnumerator waitToDisableQuizScoreText()
    {
        yield return new WaitForSeconds(2);
        iTween.FadeTo(correctText.gameObject, 0, 2f);
        StartCoroutine("waitToSayQuizIsGone");
    }

    IEnumerator waitToLeaveCurrentQuiz()
    {
        yield return new WaitForSeconds(2);
        quizAnswerGivenLoading = false;
    }

    IEnumerator waitToDisableCorrectSign()
    {
        yield return new WaitForSeconds(2);
        fadeToQuiz(1, 2);
        StartCoroutine("waitToShowNewQuizAvailable");
    }

    IEnumerator waitToFadeScoredSign()
    {
        yield return new WaitForSeconds(2);
        iTween.FadeTo(correctText.gameObject, 0, 3f);
        StartCoroutine("waitToSayQuizIsGone");
    }

    IEnumerator waitToSayQuizIsGone()
    {
        yield return new WaitForSeconds(3);
        quizAnswerGivenLoading = false;
        questionCurrentlyGoing = false;
        foreach (BoxCollider col in quizAnswerColliders) col.enabled = false;
        checkAllQuizzesForDone();
    }

    IEnumerator waitToShowNewQuizAvailable()
    {
        yield return new WaitForSeconds(2);
        quizAnswerGivenLoading = false;
    }

    private void fadeToQuiz(float fadeAmount, float time)
    {
        iTween.FadeTo(quizDisplayQuestion.gameObject, fadeAmount, time);
        foreach (TextMesh mesh in quizDisplayAnswers)
        {
            iTween.FadeTo(mesh.gameObject, fadeAmount, time);
        }
    }

    void checkAllQuizzesForDone()
    {
        foreach (Transform t in QuizColliderParent)
        {
            if (t.GetComponent<QuizDispatcher>().completed == false) return;
        }
        correctText.fontSize = 100;
        correctText.text = "YOU HAVE FOUND ALL THE PIECES";
        iTween.FadeTo(correctText.gameObject, 1, 2f);
        StartCoroutine("finalCorrectFade1");
    }
    IEnumerator finalCorrectFade1()
    {
        yield return new WaitForSeconds(2);
        iTween.FadeTo(correctText.gameObject, 0, 2f);
        StartCoroutine("finalCorrectFade2WaitToShowScore");
    }

    IEnumerator finalCorrectFade2WaitToShowScore()
    {
        yield return new WaitForSeconds(2);
        int score = 0;
        int maxScore = 0;
        foreach (Transform t in QuizColliderParent)
        {
            score += t.GetComponent<QuizDispatcher>().score;
            maxScore += t.GetComponent<QuizDispatcher>().allQuizes.Length;
        }
        correctText.text = "You scored: " + score + " out of " + maxScore;
        iTween.FadeTo(correctText.gameObject, 1, 2f);
        StartCoroutine("finalCorrectFade3SEndGame");
    }

    IEnumerator finalCorrectFade3SEndGame()
    {
        yield return new WaitForSeconds(10);
        Debug.Log("GAME DONE");
        Application.Quit();
    }
}
