using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationBlending : MonoBehaviour
{
    // Start is called before the first frame update
    Animation anim;
    public float maxSpeed = 25;
    public float speed = 5;
    float weightRun;
    float weightWalk;


    void Start()
    {
        anim = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetBlending(float ratio)
    {
        weightRun = ratio;
        weightWalk = 1 - weightRun;
        string[] names = new string[2];
        AnimationState[] animStates = new AnimationState[2]; 
        int i = 0;
        float len = 0;
        foreach (AnimationState _state in anim)
        {
            names[i] = _state.name;
            animStates[i++] = _state;
            _state.time = 0;
        }

        anim.Blend(names[0], weightWalk, 0.1f);
        anim.Blend(names[1], weightRun, 0.1f);
    }

}