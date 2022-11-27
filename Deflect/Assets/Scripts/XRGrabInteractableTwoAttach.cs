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

    public AudioClip clip;
    private AudioSource source;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    private void Start()
    {
        source = GetComponent<AudioSource>();
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
        {
            hb.con = controllerInteractor.xrController;
            source.pitch = Random.Range(minPitch, maxPitch);
            source.PlayOneShot(clip, 0.1f); // Equip SFX
        }

        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        // Reset
        hb.con = null;
    }
}
