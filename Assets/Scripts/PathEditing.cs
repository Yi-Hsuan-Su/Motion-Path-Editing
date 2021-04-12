using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UniHumanoid
{


    public class PathEditing : MonoBehaviour
    {
        // Start is called before the first frame update
        public hermitesplineline m_hermit;
        public Transform root;
        public GameObject[] cps;
        public GameObject model;
        public Transform[] trans;
        private bool isInit = false;
        void Start()
        {
            cps = new GameObject[GlobalData.cpsNum];
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!GlobalData.IsFinish())
                isInit = false;
            if (GlobalData.IsFinish() && !isInit)
            {
                FillCps();
            }
        }

        void FillCps()
        {
            for (int i = 0; i < GlobalData.cpsNum; i++)
            {
                cps[i] = new GameObject();//GameObject.CreatePrimitive(PrimitiveType.Cube);
                cps[i].transform.SetParent(root);
                Vector3 pos = GlobalData.GetCpsPos(i);
                cps[i].transform.position = new Vector3(-pos[0], pos[1], pos[2]);
                cps[i].transform.localScale = cps[i].transform.localScale * 0.2f;
            }
            m_hermit.FindBestAug(cps);
            isInit = true;
        }
    }
}