using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniHumanoid;
public class CameraTracking : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject cubeman;
    public Slider slider;
    bool init = false;
    AnimationBlending animBlend;
    public void DoCameraTracking()
    {
        Debug.Log("DoCameraTracking!");
        Animator anim = transform.GetComponent<Animator>();

        anim.SetTrigger("run");
    }

    public void SetBlendingRatio()
    {
        if (!init)
        {
            init = true;
            cubeman = GlobalData.GetFocusObj();
            animBlend = cubeman.GetComponent<AnimationBlending>();
        }
        GlobalData.blendingRatio = slider.value;
        animBlend.SetBlending(GlobalData.blendingRatio);
    }
  
    public void SetCps()
    {
        // Debug.Log("FrameCountList [0] " + GlobalData.FrameCountList[0]);

        int count = GlobalData.FrameCountList[0];
        if(GlobalData.frameset.Count > 1)
        {
            if(GlobalData.FrameCountList[0] > GlobalData.FrameCountList[1])
            {
                count = GlobalData.FrameCountList[1];
            }
        }

        int step = count / (GlobalData.cpsNum - 1);
        Vector3[] cp0s = new Vector3[GlobalData.cpsNum];
        var set0 = GlobalData.frameset[0]["hip"];
     //   Debug.Log("step " + step);
        for (int k = 0, m = 0; m < GlobalData.cpsNum; k += step, m++)
        {
           // Debug.Log("k " + k);
            cp0s[m] = set0.GetPosition(k) * GlobalData.m_scale;
        }

        if(GlobalData.frameset.Count > 1)
        {
            Debug.Log("FrameCountList [1] " + GlobalData.FrameCountList[1]);
            int step1 = count / (GlobalData.cpsNum - 1);
            Vector3[] cp1s = new Vector3[GlobalData.cpsNum];
            var set1 = GlobalData.frameset[1]["hip"];

            for (int k = 0, m = 0; k < GlobalData.cpsNum; k += step1, m++)
            {
                cp1s[m] = set1.GetPosition(k) * GlobalData.m_scale;
            }

            for (int i = 0; i < GlobalData.cpsNum; i++)
            {
                GlobalData.SetCps(cp0s[i] * GlobalData.blendingRatio + cp1s[i]*(1.0f - GlobalData.blendingRatio), i);
            }
        
        }
        else
        {
            for(int i =0; i < GlobalData.cpsNum; i++)
            {
                GlobalData.SetCps(cp0s[i], i);
            }
        }
       
    }
}
