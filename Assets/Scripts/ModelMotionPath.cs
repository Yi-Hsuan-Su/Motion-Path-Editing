using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniHumanoid
{
    public class ModelMotionPath : MonoBehaviour
    {
        public Transform trans = null;
        public bool flag = false;
        // Update is called once per frame
        void FixedUpdate()
        {
            if(trans == null && GlobalData.IsFinish())
            {
                trans = GameObject.FindGameObjectWithTag("sphere").GetComponent<Transform>();
                flag = true;
            }
            if (flag)
            {
               this.transform.position = Vector3.Lerp(this.transform.position, trans.position*0.2f - GlobalData.PosOffset*0.2f, 0.5f) - new Vector3(1,0,0) ;
               this.transform.rotation = Quaternion.Euler(new Vector3(0, GlobalData.RotOffset, 0));
            }
        }
    }
}