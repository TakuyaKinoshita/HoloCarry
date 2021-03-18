using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimSample : MonoBehaviour
{
    public Animator animator;
    public GameObject Hanabi;

    void Start()
    {
        Invoke("Walkanim", 3.0f);
        Invoke("Zitabataanim", 6.0f);
        Invoke("Goalanim", 13.0f);  
    }

    void Update()
    {
        

        
    }

    private void Walkanim()
    {
        animator.Play("Walk");
    }

    private void Zitabataanim()
    {
        animator.Play("Surprised");
        animator.Play("Zitabata");
    }

    private void Goalanim()
    {
        animator.Play("Goal");
        animator.Play("GoalSmile1");
        Hanabi.SetActive(true);
    }
}
