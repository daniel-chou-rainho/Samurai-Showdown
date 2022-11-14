using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabInteractableTwoAttach : XRGrabInteractable
{
    public Transform leftAttachTransform;
    public Transform rightAttachTransform;

    public GameObject blade;
    private HapticBlade hb;

    private void Start()
    {
        hb = blade.GetComponent<HapticBlade>();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (args.interactorObject.transform.CompareTag("Left Hand"))
        {
            attachTransform = leftAttachTransform;
        }
        else if (args.interactorObject.transform.CompareTag("Right Hand"))
        {
            attachTransform = rightAttachTransform;
        }

        // Pass Controller
        if(args.interactorObject is XRBaseControllerInteractor controllerInteractor)
            hb.con = controllerInteractor.xrController;

        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        // Reset
        hb.con = null;
    }
}
