using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using burningmime.curves;
using FastBezier;
namespace UniHumanoid
{
    public class hermitesplineline : MonoBehaviour
    {
        private float c_numSamples = 100;
        public List<Vector3> originalpoint;
        public List<Vector3> drawpoint;
        public List<Vector3> tangpoint;
        public List<Vector3> origtangpoint;
        public List<Vector3> arc_points;
        public  List<float> arc_length;
        public List<GameObject[]> controlpts = new List<GameObject[]>();
        public List<Vector3[]> prevcontrolpts = new List<Vector3[]>();
        public GameObject hip;
        public int frame;
        public float max_arc_length;
        public float avg_frame_dist;
        // Update is called once per frame

        void Start()
        {

        }

        public void FindBestAug(GameObject[] linepoints)
        {
            frame = GlobalData.FrameCountList[0]; 
            //hip = GameObject.Find("hip");
           // c_numSamples = GlobalData.FrameCount+1;
            List<Vector3> pts = new List<Vector3>();

            for (int i = 0; i < linepoints.Length; i++)
            {
                pts.Add(linepoints[i].transform.position);
            }
            CurvePreprocess.RemoveDuplicates(pts);
            List<Vector3> reduced = CurvePreprocess.RdpReduce(pts, 0.0001f);
            CubicBezier[] curves = CurveFit.Fit(reduced, 0.1f);

            Vector3 offset = new Vector3(1, 0, 0);
            for (int i = 0; i < curves.Length; i++)
            {
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.name = "conp0";
                obj.transform.localPosition = curves[i].p0 - offset;
                obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                obj.transform.SetParent(this.transform);

                GameObject obj1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj1.name = "conp1";
                obj1.transform.localPosition = curves[i].p1 - offset;
                obj1.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                obj1.transform.SetParent(this.transform);

                GameObject obj2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj2.name = "conp2";
                obj2.transform.localPosition = curves[i].p2 - offset;
                obj2.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                obj2.transform.SetParent(this.transform);

                GameObject obj3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj3.name = "conp3";
                obj3.transform.localPosition = curves[i].p3 - offset;
                obj3.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                obj3.transform.SetParent(this.transform);

                GameObject[] contset = { obj, obj1, obj2, obj3 };
                controlpts.Add(contset);
            }


            for (int i = 0; i < controlpts.Count; i++)
            {
                Vector3[] vcot = { controlpts[i][0].transform.position, controlpts[i][1].transform.position, controlpts[i][2].transform.position, controlpts[i][3].transform.position };
                prevcontrolpts.Add(vcot);
            }

            if (controlpts.Count > 1)
            {
                c_numSamples = (GlobalData.FrameCountList[0] / controlpts.Count) + 1;
            }
            else 
            {
                c_numSamples = GlobalData.FrameCountList[0] ;
            }

            //DrawBezier();
            calparaBezier();
            InitializeArcLengths();
            tangentCalculation();
            for (int i = 0; i < tangpoint.Count; i++) 
            {
                origtangpoint.Add(tangpoint[i]);
            }

        }

        public bool checkchange() 
        {
            for (int i = 0; i < controlpts.Count; i++)
            {
                for (int j = 0; j < controlpts[i].Length; j++)
                {
                    if (controlpts[i][j].transform.position != prevcontrolpts[i][j])
                    {
                        return true;
                    }
                }
            }
            return false;
          }

        private void Update()
        {
            if (checkchange())
            {
                //  DrawBezier();
               calparaBezier();
             InitializeArcLengths();
            tangentCalculation();
            }
        }

        public void DrawBezier()
        {
            drawpoint.Clear();
            for (int j = 0; j < controlpts.Count; ++j)
            {
                for (int i = 0; i < c_numSamples; ++i)
                {

                    float percent = ((float)i) / (c_numSamples - 1);
                    float x = (2 - 1) * percent;

                    int index = (int)x;
                    float t = x - Mathf.Floor(x);
                    Vector3 drawpts = new Vector3();
                    drawpts = BezierPathCalculation(controlpts[j][0].transform.position, controlpts[j][1].transform.position, controlpts[j][2].transform.position, controlpts[j][3].transform.position, t);
                    drawpoint.Add(drawpts);
                    arc_points.Add(drawpts);
                }
                drawpoint.RemoveAt(drawpoint.Count - 1);
            }

            /*
            LineRenderer lineRenderer;
            lineRenderer = this.GetComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Standard"));
            lineRenderer.startWidth = 0.01f;
            lineRenderer.sharedMaterial.SetColor("_Color", Color.red);
            lineRenderer.positionCount = drawpoint.Count;
            lineRenderer.SetPositions(drawpoint.ToArray());
            for (int i = 0; i < controlpts.Count; i++)
            {
                Vector3[] vcot = { controlpts[i][0].transform.position, controlpts[i][1].transform.position, controlpts[i][2].transform.position, controlpts[i][3].transform.position };
                prevcontrolpts.Add(vcot);
            }*/
        }

        public void calparaBezier()
        {
            arc_points.Clear();
            for (int j = 0; j < controlpts.Count; ++j)
            {
               // arc_points = new List<Vector3>();
                for (int i = 0; i < c_numSamples*2; ++i)
                {
                    float percent = ((float)i) / (c_numSamples * 2 - 1);
                    float x = (2 - 1) * percent;

                    int index = (int)x;
                    float t = x - Mathf.Floor(x);
                    Vector3 drawpts = new Vector3();
                    drawpts = BezierPathCalculation(controlpts[j][0].transform.position, controlpts[j][1].transform.position, controlpts[j][2].transform.position, controlpts[j][3].transform.position, t);
                    arc_points.Add(drawpts);
                }
                arc_points.RemoveAt(arc_points.Count - 1);
            }



            for (int i = 0; i < controlpts.Count; i++)
            {
                Vector3[] vcot = { controlpts[i][0].transform.position, controlpts[i][1].transform.position, controlpts[i][2].transform.position, controlpts[i][3].transform.position };
                prevcontrolpts.Add(vcot);
            }

        }
        public GameObject GetIndexClamped(GameObject[] points, int index)
        {
            if (index < 0)
                return points[0];
            else if (index >= (points.Length))
                return points[points.Length - 1];
            return points[index];
        }


        public Vector3 BezierPathCalculation(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float tt = t * t;
            float ttt = t * tt;
            float u = 1.0f - t;
            float uu = u * u;
            float uuu = u * uu;

            Vector3 B = new Vector3();
            B = uuu * p0;
            B += 3.0f * uu * t * p1;
            B += 3.0f * u * tt * p2;
            B += ttt * p3;

            return B;
        }
        public void  tangentCalculation()
        {
            tangpoint.Clear();
            for (int i = 0; i < drawpoint.Count; i++)
            {
                if (i + 1 < drawpoint.Count)
                {
                    tangpoint.Add((drawpoint[i + 1] - drawpoint[i]).normalized);
                }
            }
            tangpoint.Add( tangpoint[tangpoint.Count - 1]);
        }


        public void InitializeArcLengths()
        {
            drawpoint.Clear();


                arc_length.Clear();
                int count =arc_points.Count;
                arc_length.Add(0);
                float clen = 0;
                Vector3 pp =arc_points[0];
                for (int i = 1; i < count; i++)
                {
                    Vector3 np = arc_points[i];
                    clen += VectorHelper.Distance(pp, np);
                    arc_length.Add(clen);
                    pp = np;
                }
                max_arc_length = clen;
                avg_frame_dist = max_arc_length / (c_numSamples * controlpts.Count);

                double min = 10000;
                int idx = 0;
                for (int i = 0; i < c_numSamples*controlpts.Count; i++)
                {
                    min = 10000;
                    //Debug.LogWarning(avg_frame_dist * i);
                    for (int j = 0; j < arc_length.Count; j++)
                    {
                        float dist = arc_length[j] - (avg_frame_dist * i);
                        if (min > Mathf.Pow(dist, 2.0f))
                        {
                            min = Mathf.Pow(dist, 2.0f);
                            //Debug.LogWarning(min);
                            idx = j;
                        }
                    }
                    drawpoint.Add(arc_points[idx]);
                    //   Debug.LogWarning(idx);
                }
           

            LineRenderer lineRenderer;
            lineRenderer = this.GetComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Standard"));
            lineRenderer.startWidth = 0.01f;
            lineRenderer.sharedMaterial.SetColor("_Color", Color.red);
            lineRenderer.positionCount = drawpoint.Count;
            lineRenderer.SetPositions(drawpoint.ToArray());

       
        }

    }
}