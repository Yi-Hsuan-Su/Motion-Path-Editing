using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;


namespace UniHumanoid
{
    [Obsolete("use BvhImporterContext")]
    public class ImporterContext: BvhImporterContext
    {
    }

    public class BvhImporterContext
    {
        #region Source
        String m_path;
        public String Path
        {
            get { return m_path; }
            set
            {
                if (m_path == value) return;
                m_path = value;
            }
        }
        public String Source; // source
        public List<Bvh> bvhList = new List<Bvh>();
        public Bvh Bvh;
        #endregion

        #region Imported
        public GameObject Root = null;
        public List<Transform> Nodes = new List<Transform>();
        public List<AnimationClip> animationList;
        public AvatarDescription AvatarDescription;
        public Avatar Avatar;
        public Mesh Mesh;
        public Material Material;
        public float scaling = 1.0f;
        public float alginSec = 0;
        #endregion

        #region Load
        [Obsolete("use Load(path)")]
        public void Parse()
        {
            Parse(Path);
        }

        public void Parse(string path)
        {
            Path = path;
            Source = File.ReadAllText(Path, Encoding.UTF8);
            Bvh = Bvh.Parse(Source);
            bvhList.Add(Bvh);
        }

        public void Load()
        {
            //
            // build hierarchy
            //
            GlobalData.clear();
            
            if (Root == null)
            {
                Root = new GameObject(System.IO.Path.GetFileNameWithoutExtension(Path));
                Root.tag = "cubeman";
                GlobalData.SetFocusObj(Root);
                GlobalData.animeobjname = Root.name;

                var hips = BuildHierarchy(Root.transform, Bvh.Root, 1.0f, null);
                var skeleton = Skeleton.Estimate(hips);
                var description = AvatarDescription.Create(hips.Traverse().ToArray(), skeleton);
                //
                // scaling. reposition
                //
                scaling = 1.0f;
                {
                    //var foot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
                    var foot = hips.Traverse().Skip(skeleton.GetBoneIndex(HumanBodyBones.LeftFoot)).First();
                    var hipHeight = hips.position.y - foot.position.y;
                    // hips height to a meter
                    scaling = 1.0f / hipHeight;
                    GlobalData.m_scale = scaling;
                    foreach (var x in Root.transform.Traverse())
                    {
                        x.localPosition *= scaling;
                    }

                    var scaledHeight = hipHeight * scaling;
                    hips.position = new Vector3(0, scaledHeight, 0); // foot to ground
                }
                GlobalData.FrameCountList.Add(Bvh.FrameCount);
                Debug.Log("Bvh.FrameCount : " + Bvh.FrameCount);

                Avatar = description.CreateAvatar(Root.transform);
                Avatar.name = "Avatar";
                AvatarDescription = description;

                var animator = Root.AddComponent<Animator>();
                animator.avatar = Avatar;
                GlobalData.SetAvatar(Avatar);

                AnimationClip Animation = BvhAnimation.CreateAnimationClip(Bvh, scaling, true);
                Animation.name = Root.name;
                Animation.legacy = true;
                Animation.wrapMode = WrapMode.Loop;
                alginSec = Animation.length;
                var animation = Root.AddComponent<Animation>();
                animation.AddClip(Animation, Animation.name);
                animation.clip = Animation;
                animation.Play();
                GlobalData.SetAnimationClip(Animation);

                var humanPoseTransfer = Root.AddComponent<HumanPoseTransfer>();
                humanPoseTransfer.Avatar = Avatar;

                // create SkinnedMesh for bone visualize
                var renderer = SkeletonMeshUtility.CreateRenderer(animator);
                Material = new Material(Shader.Find("Standard"));
                renderer.sharedMaterial = Material;
                Mesh = renderer.sharedMesh;
                Mesh.name = "box-man";

                Root.AddComponent<BoneMapping>();

            }
            else
            {
                GlobalData.FrameCountList.Add(Bvh.FrameCount);
                Debug.Log("Bvh.FrameCount : " + Bvh.FrameCount);

                AnimationClip Animation = BvhAnimation.CreateAnimationClip(Bvh, scaling);
                Animation.name = Root.name+ bvhList.Count().ToString();
                Animation.legacy = true;
                Animation.wrapMode = WrapMode.Loop;
                var animation = Root.GetComponent<Animation>();
                GlobalData.timeAlign = Animation.length / alginSec; 
                animation.AddClip(Animation, Animation.name);
                animation.clip = Animation;

                animation.Play();
              //  GlobalData.SetAnimationClip(Animation);
            }

            // create AnimationClip
            //



        }

        static Transform BuildHierarchy(Transform parent, BvhNode node, float toMeter)
        {
            var go = new GameObject(node.Name);
            go.transform.localPosition = node.Offset.ToXReversedVector3() * toMeter;
            go.transform.SetParent(parent, false);

            foreach (var child in node.Children)
            {
                BuildHierarchy(go.transform, child, toMeter);
            }

            return go.transform;
        }

        static Transform BuildHierarchy(Transform parent, BvhNode node, float toMeter, Transform sparent)
        {
            //Debug.Log("Part : " + node.Name);
            var go = new GameObject(node.Name);
            go.transform.localPosition = node.Offset.ToXReversedVector3() * toMeter;
            go.transform.SetParent(parent, false);

            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = node.Name;
            sphere.AddComponent<ShowBone>();
            sphere.GetComponent<ShowBone>().SetTransform(go.transform, true);
            if(sparent != null)
            {
                sphere.transform.SetParent(sparent, false);
                if (node.Name == "head")
                    sphere.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                else if(node.Name == "head" || node.Name == "rThumb1" || node.Name == "rIndex1" ||
                    node.Name == "rMid1" || node.Name == "rRing1" || node.Name == "rPinky1" ||
                    node.Name == "lThumb1" || node.Name == "lIndex1" || node.Name == "lMid1" ||
                    node.Name == "lRing1" || node.Name == "lPinky1" || node.Name == "leftEye" || node.Name == "rightEye")
                    sphere.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                else
                    sphere.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            }
            else
            {
                GlobalData.SetFocusObj(sphere);

                sphere.transform.localScale = sphere.transform.localScale * 0.2f;
                sphere.tag = "sphere";
            }

            foreach (var child in node.Children)
            {
                BuildHierarchy(go.transform, child, toMeter, sphere.transform);
            }

            return go.transform;
        }
        #endregion

#if UNITY_EDITOR
        protected virtual string GetPrefabPath()
        {
            var dir = System.IO.Path.GetDirectoryName(Path);
            var name = System.IO.Path.GetFileNameWithoutExtension(Path);
            var prefabPath = string.Format("{0}/{1}.prefab", dir, name);
            if (!Application.isPlaying && File.Exists(prefabPath))
            {
                // already exists
                if (IsOwn(prefabPath))
                {
                    //Debug.LogFormat("already exist. own: {0}", prefabPath);
                }
                else
                {
                    // but unknown prefab
                    var unique = AssetDatabase.GenerateUniqueAssetPath(prefabPath);
                    //Debug.LogFormat("already exist: {0} => {1}", prefabPath, unique);
                    prefabPath = unique;
                }
            }
            return prefabPath;
        }

        #region Assets
        IEnumerable<UnityEngine.Object> GetSubAssets(string path)
        {
            return AssetDatabase.LoadAllAssetsAtPath(path);
        }

        protected virtual bool IsOwn(string path)
        {
            foreach (var x in GetSubAssets(path))
            {
                //if (x is Transform) continue;
                if (x is GameObject) continue;
                if (x is Component) continue;
                if (AssetDatabase.IsSubAsset(x))
                {
                    return true;
                }
            }
            return false;
        }

        IEnumerable<UnityEngine.Object> ObjectsForSubAsset()
        {
            if (animationList.Count != 0) yield return animationList[0];
            if (AvatarDescription != null) yield return AvatarDescription;
            if (Avatar != null) yield return Avatar;
            if (Mesh != null) yield return Mesh;
            if (Material != null) yield return Material;
        }

        public void SaveAsAsset()
        {
            var path = GetPrefabPath();
            if (File.Exists(path))
            {
                // clear SubAssets
                foreach (var x in GetSubAssets(path).Where(x => !(x is GameObject) && !(x is Component)))
                {
                    GameObject.DestroyImmediate(x, true);
                }
            }

            // Add SubAsset
            foreach (var o in ObjectsForSubAsset())
            {
                AssetDatabase.AddObjectToAsset(o, path);
            }

            // Create or upate Main Asset
            if (File.Exists(path))
            {
                Debug.LogFormat("replace prefab: {0}", path);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                PrefabUtility.ReplacePrefab(Root, prefab, ReplacePrefabOptions.ReplaceNameBased);
            }
            else
            {
                Debug.LogFormat("create prefab: {0}", path);
                PrefabUtility.CreatePrefab(path, Root);
            }

            AssetDatabase.ImportAsset(path);
        }
        #endregion
#endif

        public void Destroy(bool destroySubAssets)
        {
            if (Root != null) GameObject.DestroyImmediate(Root);
            if (destroySubAssets)
            {
#if UNITY_EDITOR
                foreach (var o in ObjectsForSubAsset())
                {
                    UnityEngine.Object.DestroyImmediate(o, true);
                }
#endif
            }
        }
    }
}
