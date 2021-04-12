using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniHumanoid
{
    public static class GlobalData
    {
        // Start is called before the first frame update
        public static int cpsNum = 70;
        //public static int FrameCount = 0;
        public static List<int> FrameCountList = new List<int>();
        public static int focusIndex = 0;
        public static int bvhIndex = 0;
        private static Vector3[] cpsPos;
        private static bool isInit = false;
        private static bool finish = false;
        private static Avatar avatar;
        private static AnimationClip anim;
        private static List<GameObject> focusObj = new List<GameObject>();
        public static List<Dictionary<string, BvhAnimation.CurveSet>> frameset = new List<Dictionary<string, BvhAnimation.CurveSet>>();
        public static float m_scale =0;
        public static string animeobjname;
        public static Vector3 PosOffset = new Vector3(0,0,0);
        public static float RotOffset = 0;
        public static float blendingRatio = 0.0f;
        public static float timeAlign = 0.0f;
        public static void clear()
        {
            finish = false;
            //if (focusObj.Count > 1)
           // {
                //focusObj[1].GetComponent<ShowBone>().Kill();
                //focusObj.Clear();
          //  }
        }

        public static void Switch(int id)
        {
            for (int i = 0; i < focusObj.Count; i++)
            {
                focusObj[i].SetActive(false);
            }
            focusIndex = id;

            if (id != 2)
                focusObj[bvhIndex * 2 + focusIndex].SetActive(true);

        }
        public static int GetObjNum()
        {
            return focusObj.Count;
        }
        public static GameObject GetFocusObj()
        {
            return focusObj[0];
        }
        public static void SetFocusObj(GameObject obj)
        {
            focusObj.Add(obj);
        }
        public static AnimationClip GetAnimationClip()
        {
            return anim;
        }
        public static void SetAnimationClip(AnimationClip a)
        {
            anim = a;
        }
        public static Avatar GetAvatar()
        {
            return avatar;
        }
        public static void SetAvatar(Avatar a)
        {
            avatar = a;
        }
        public static bool IsFinish()
        {
            return finish;
        }
        public static Vector3 GetCpsPos(int index)
        {
            return cpsPos[index];
        }


        public static void SetCps(Vector3 p, int index)
        {
            //Debug.Log("Control Point[" + index + "] is " + p);
            if (!isInit)
            {
                cpsPos = new Vector3[cpsNum];
                isInit = true;
            }
            if (index >= cpsNum)
            {
                Debug.Log("Cps index out of range!");
                return;
            }

            cpsPos[index] = p;
           // Debug.Log("cpsPos[" + index + "] = " + p);
            if (index == cpsNum - 1)
            {
                finish = true;
            }
        }
    }

}