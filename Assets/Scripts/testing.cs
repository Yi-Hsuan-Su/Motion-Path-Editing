using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniHumanoid
{
    public class testing : MonoBehaviour
    {
        
        int index = 0;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log(Time.frameCount);
           /* Vector3 p = GlobalData.frameset[transform.gameObject.name].GetPosition(index);
            Vector3 r = GlobalData.frameset[transform.gameObject.name].GetmRotation(index++);
            if (p.x == -1000)
            {
                p = new Vector3(transform.position.x, p.y, p.z);
            }

            if (p.y == -1000)
            {
                p = new Vector3(p.x, transform.position.y, p.z);
            }

            if (p.z == -1000)
            {
                p = new Vector3(p.x, p.y, transform.position.z);
            }

            if (r.x == -1000)
            {
                r = new Vector3(transform.eulerAngles.x, r.y, r.z);
            }

            if (r.y == -1000)
            {
                r = new Vector3(p.x, transform.eulerAngles.y, r.z);
            }

            if (r.z == -1000)
            {
                r = new Vector3(r.x, r.y, transform.eulerAngles.z);
            }

            transform.position =  p * GlobalData.m_scale;
            transform.eulerAngles = r;
            index %= GlobalData.FrameCount - 1;*/
            
        }
    }
}