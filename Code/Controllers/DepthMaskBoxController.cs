using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Ciaran Rowles

public class DepthMaskBoxController : MonoBehaviour {

    public Renderer reverseDoorRenderer;

    [SerializeField]
    private GameObject Camera;

    [SerializeField]
    private GameObject Door;

    private Vector3 doorPosition;

    private bool insideBoxLastFrame = true;

    [SerializeField]
    private Transform startMovePoint;

    [SerializeField]
    private Transform endMovePoint;

    [SerializeField]
    private Transform DepthMaskToMove;

    private void Start ()
    {
        // TODO: Find more efficiant solution.
        // This allows the camera positioning system to be tested in editor
        if(!Application.isEditor) Camera = GameObject.Find("First Person Camera");
        else Camera = GameObject.Find("CameraDebugObj");
        doorPosition = Door.transform.position;
        DepthMaskToMove.position = startMovePoint.position;
        iTween.MoveTo(DepthMaskToMove.gameObject, endMovePoint.position, 10);
    }

    // Cycles through all mesh renderers in box and disables them, this gives the impression that once you
    // have walked through the door, the area inside the box is now just the normal scenery.
	public void DisableAllMeshRenderers (bool isInside)
    {
        Debug.Log("Mode Switched");
        List<Renderer> meshRenderersToDisable = new List<Renderer>(GetComponentsInChildren<Renderer>());
        meshRenderersToDisable.Remove(reverseDoorRenderer);
        meshRenderersToDisable.Remove(reverseDoorRenderer.transform.parent.GetComponent<Renderer>());
        foreach(Renderer rend in meshRenderersToDisable) rend.enabled = isInside;
        if (!isInside) QuizController.instance.beginGame();
    }

    private void Update ()
    {
        isCameraInsideBoxDetector();
    }

    private void isCameraInsideBoxDetector()
    {
        bool isInsideboxNow = false;
        if (Camera.transform.position.x < Door.transform.position.x)   isInsideboxNow = true;
        else isInsideboxNow = false;
        if (isInsideboxNow != insideBoxLastFrame) DisableAllMeshRenderers(isInsideboxNow);
        insideBoxLastFrame = isInsideboxNow;
    }

}
