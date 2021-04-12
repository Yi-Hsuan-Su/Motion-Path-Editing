using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniHumanoid
{
    public class ShowBone : MonoBehaviour
    {
        // Start is called before the first frame update
        private Transform trans;
        private Vector3 lastPos;
        private Quaternion lastRot ;
        public float rotateoffset=0;
        private bool openInterpolation = false;
        public Animation cubeman;
        public hermitesplineline mspl;
        Vector3 frampos , offset;
        float  offsetx ,offsety , offsetz;
        int count = 0;
        int space = 3;
        float t = 0;
        public void SetTransform(Transform t, bool interpolation = false)
        {
            //this.transform.SetParent(GameObject.Find("ControlPoints").GetComponent<Transform>());
            mspl = GameObject.Find("ControlPoints").GetComponent<hermitesplineline>();
            trans = t;
            openInterpolation = interpolation;
        }

        public void Kill()
        {
            Destroy(this.gameObject);
        }
        // Update is called once per frame

       Vector3 Calpath(int i)
        {
           
             if (GlobalData.IsFinish() == true)
             {
                     Vector3 framepos = GlobalData.frameset[0]["hip"].GetPosition(i);
                      Vector3 newpos = framepos+(framepos - mspl.drawpoint[i]);
             }
            return Vector3.zero;
          //  Debug.LogError(this.transform.gameObject.name);
        }

        int Findcurrentframekey() 
        {
            string name = GlobalData.animeobjname;
           // Debug.LogWarning(name);
            if (cubeman == null)
            {
                cubeman = GameObject.Find(name).GetComponent<Animation>();
            }
            if (cubeman[name].normalizedTime > 1.0)
            {
                cubeman[name].normalizedTime = 0;
            }
            float tmp = cubeman[name].normalizedTime * GlobalData.FrameCountList[0];
            return (int)tmp;
  
        }

        void FixedUpdate()
        {
            
            if (GlobalData.IsFinish())
            {

                frampos = GlobalData.frameset[0]["hip"].GetPosition(Findcurrentframekey())*GlobalData.m_scale ;
                
                if(this.name == "hip")
                {
                    //frampos = trans.position;
                    offsetx = (-1 * frampos.x) - mspl.drawpoint[Findcurrentframekey()].x;
                    offsety = frampos.y - mspl.drawpoint[Findcurrentframekey()].y;
                    offsetz = frampos.z - mspl.drawpoint[Findcurrentframekey()].z;
                    offset = new Vector3(offsetx, offsety, offsetz);
                    GlobalData.PosOffset = offset;
                }
                else
                {
                    offset = GlobalData.PosOffset;
                }
                if (this.name == "lButtock")
                {
                    rotateoffset = Vector3.Angle(mspl.origtangpoint[Findcurrentframekey()], mspl.tangpoint[Findcurrentframekey()]);
                    Vector3 crossD = Vector3.Cross(mspl.origtangpoint[Findcurrentframekey()], mspl.tangpoint[Findcurrentframekey()]);
                    float dir = Vector3.Dot(crossD, Vector3.up);
                    rotateoffset *= dir;
                    Debug.DrawLine(mspl.drawpoint[Findcurrentframekey()], mspl.drawpoint[Findcurrentframekey()] + mspl.tangpoint[Findcurrentframekey()], Color.red);
                    Debug.DrawLine(mspl.drawpoint[Findcurrentframekey()], mspl.drawpoint[Findcurrentframekey()] + mspl.origtangpoint[Findcurrentframekey()], Color.blue);
                    GlobalData.RotOffset = rotateoffset;
                    //  Debug.LogWarning(rotateoffset);
                }
                
            }
            if (openInterpolation)
            {
               // if (count % 4 == 0)
               // {
                  
                    lastPos = trans.position;
                    lastRot = trans.rotation;
                    //count = 0;
                //}
                count++;
                t = (float)count / 12.0f;
                this.transform.position = Vector3.Lerp(this.transform.position, lastPos, 0.6f + t)- offset ;
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, lastRot, 0.6f + t);
                if (this.name == "lButtock")
                {
                    this.transform.parent.transform.rotation= Quaternion.Euler(new Vector3(0, rotateoffset, 0));
                }
            }
            else
            {
                this.transform.position = trans.position - new Vector3(2, 0, 0);
                this.transform.rotation = trans.rotation;
            }
        }
    }

}