using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore.Examples.HelloAR;

// Author : Ciaran Rowles

public class UIController : MonoBehaviour {
    [SerializeField]
    private GameObject cameraDepthCubeContainer;

    [SerializeField]
    private Camera ARCamera;

    private shaderGlow currentlyGlowing;

    void Start ()
    {
        cameraDepthCubeContainer.SetActive(true);
        HelloARController.spawnedObj += spawnInDoor;
        if(Application.isEditor)
        {
            ARCamera = GameObject.Find("CameraDebugObj").GetComponent<Camera>();
        }
        else
        {
            ARCamera = GameObject.Find("First Person Camera").GetComponent<Camera>();
            Destroy(GameObject.Find("CameraDebugObj"));
        }
	}

    void Update()
    {
        if (Application.isEditor)
        {
            if (Input.GetMouseButtonDown(0)) ButtonPressed(Input.mousePosition);
        }
        else
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began) ButtonPressed(Input.GetTouch(0).position);
        }

        if (!QuizController.instance.quizStarted) return;
        RaycastHit hit;
        Transform objectHit = null;
        Ray ray = ARCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        if (Physics.Raycast(ray, out hit)) objectHit = hit.transform;
        shaderGlow glowObj = null;
        //if (objectHit != null)
        //{
        //     if (QuizController.instance.questionCurrentlyGoing && objectHit.tag != "AnswerObject") return;
        //     if (!QuizController.instance.questionCurrentlyGoing && objectHit.tag != "QuizObject") return;
        //}
        if (objectHit != null)
        {
            glowObj = objectHit.GetComponent<shaderGlow>();
        }

        if (currentlyGlowing != glowObj)
        {
            if (currentlyGlowing != null) currentlyGlowing.lightOff();
            if (glowObj != null) glowObj.lightOn();
        }
        currentlyGlowing = glowObj;
    }

    public void ButtonPressed (Vector3 inputPos)
    {
        if (QuizController.instance.quizStarted)
        {
            RaycastHit hit;
            Transform objectHit = null;
            Ray ray = ARCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            if (Physics.Raycast(ray, out hit)) objectHit = hit.transform;
            if (objectHit == null) return;
            if (QuizController.instance.questionCurrentlyGoing && objectHit.tag != "AnswerObject") return;
            if (!QuizController.instance.questionCurrentlyGoing && objectHit.tag != "QuizObject") return;
            if (QuizController.instance.questionCurrentlyGoing)
            {
                QuizAnswerData dispatcher = objectHit.GetComponent<QuizAnswerData>();                
                if (dispatcher != null) dispatcher.quizStarted();
            }
            else
            {
                QuizDispatcher dispatcher = objectHit.GetComponent<QuizDispatcher>();
                if (dispatcher != null) dispatcher.quizStarted();
            }
        }
        else QuizController.instance.moveToNextIntroText();
    }

	void spawnInDoor ()
    {
        cameraDepthCubeContainer.SetActive(false);
    }
}
