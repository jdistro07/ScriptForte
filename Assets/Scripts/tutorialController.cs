using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tutorialController : MonoBehaviour {

    // objects
    [Header("Objects")]
    public GameObject panelObject;

    [Header("Components")]
    public Animator animator;
    public AnimationClip animationClip;
    public AudioSource notifySound;

    private void Start()
    {
        animator = panelObject.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(inMovementTutorialColliderStay()); // if player stays for 3 seconds inside the collider
    }

    private void OnTriggerExit(Collider other)
    {
        if (!(other.gameObject.name == "FPS Controller"))
        {
            StartCoroutine(inMovementTutorialColliderLeave());
        }
    }

    // movement controls coroutines
    IEnumerator inMovementTutorialColliderStay() //player inside the collider
    {
        yield return new WaitForSeconds(1);

        panelObject.SetActive(true);
        animator.SetInteger("open", 1);

        var notified = false;

        if(!notified)
            notifySound.Play();

    }

    IEnumerator inMovementTutorialColliderLeave() // player leave
    {
        yield return new WaitForSeconds(5);

        animator.SetInteger("open", 0);
        yield return new WaitForSeconds(animationClip.length);
        panelObject.SetActive(false);

        //turn off collider
        this.gameObject.SetActive(false);
    }
}
