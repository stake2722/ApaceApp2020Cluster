using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Constellation
{
    public class ConstellationQuadStars : MonoBehaviour
    {
        [SerializeField] Hipparcos hipparcosScr = null;
        [SerializeField] Material pointCloudMaterial = null;
        [SerializeField] bool hideZodiac = false;
        [SerializeField] bool useParallax = false;

        // Use this for initialization
        void Start()
        {
            if (hipparcosScr)
            {
                List<Hipparcos.HipData> _hipList = (hipparcosScr.hipAppendList.Count>0) ? hipparcosScr.hipAppendList : hipparcosScr.hipList;
                if(hideZodiac){
                    _hipList = Hipparcos.GetHipListWithoutZodiac(_hipList, hipparcosScr.hipLineList);
                }
                SetMeshStars(_hipList, transform, pointCloudMaterial, hipparcosScr.distance, useParallax);
            }
        }

        // Update is called once per frame
        void Update()
        {
        }

        static  public void SetMeshStars(List<Hipparcos.HipData> _hipList, Transform _meshTr, Material _meshMat, float _distance, bool _useParallax){
            if(_meshTr!=null){
                List<Vector3> vtcs = new List<Vector3>();
                List<Color> cols = new List<Color>();
                int groupCnt = 0;
                for (int i = 0; i < _hipList.Count; ++i)
                {
                    float paradist = 1f;
                    if ((_useParallax) && (_hipList[i].parallax != 0f))
                    {
                        paradist = Mathf.Clamp(1000f / Mathf.Abs(_hipList[i].parallax), 5, 5000f) * 0.005f;
                    }
                    Vector3 pos = _hipList[i].direction * _distance * paradist;
                    Color col = _hipList[i].color; // 色補正
                    col.r *= 4f;
                    col.g *= 2f;
                    col.b *= 3f;
                    vtcs.Add(pos);
                    cols.Add(col);
                    if ((vtcs.Count > (65534/4)) || (i == _hipList.Count - 1))
                    {
                        string name = "starsQuad" + groupCnt.ToString();
                        GameObject meshObj = new GameObject(name, new[] { typeof(MeshFilter), typeof(MeshRenderer) });
                        meshObj.transform.SetParent(_meshTr);
                        meshObj.transform.localPosition = Vector3.zero;
                        meshObj.transform.localRotation = Quaternion.identity;
                        meshObj.transform.localScale = Vector3.one;
                        meshObj.GetComponent<MeshRenderer>().material = _meshMat;
                        MeshFilter mf = meshObj.GetComponent<MeshFilter>();
                        Mesh mesh = TmLib.TmMesh.CreateQuadCloud(vtcs.ToArray(), cols.ToArray(),0.02f,true);
                        mf.mesh = mesh;
#if true
                        string path = UnityEditor.AssetDatabase.GenerateUniqueAssetPath("Assets/" + name + "_parallax.asset");
                        UnityEditor.AssetDatabase.CreateAsset(mesh, path);
                        UnityEditor.AssetDatabase.SaveAssets();
#endif
                        vtcs.Clear();
                        cols.Clear();
                        groupCnt++;
                    }
                }
            }
        }

        static public void SetEfcStars(List<Hipparcos.HipData> _hipList, Transform _paricleTr, float _distance, bool _useParallax)
        {
            ParticleSystem _ps = _paricleTr.GetComponent<ParticleSystem>();
            if (_ps != null)
            {
                ParticleSystem.MainModule pmm = _ps.main;
                _ps.Clear();
                pmm.maxParticles = 200000;
                pmm.playOnAwake = false;
                pmm.startSize = 1f;
                pmm.simulationSpace = ParticleSystemSimulationSpace.Local;
                pmm.scalingMode = ParticleSystemScalingMode.Hierarchy;
                _ps.Emit(_hipList.Count);
                ParticleSystem.Particle[] stars = new ParticleSystem.Particle[_hipList.Count];
                _ps.GetParticles(stars);
                for (int i = 0; i < _hipList.Count; ++i)
                {
                    Color col = _hipList[i].color; // 色補正
                    col.r *= 4f;
                    col.g *= 2f;
                    col.b *= 3f;
                    float paradist = 1f;
                    if(_useParallax){
                        paradist = Mathf.Clamp(1000f / _hipList[i].parallax, 50, 500f) * 0.005f;
                    }
                    stars[i].position = _hipList[i].direction * _distance * paradist;
                    stars[i].startColor = col;
                    stars[i].startSize *= Mathf.Clamp(10f / _hipList[i].magnitude, 0.5f, 10f);
                }
                _ps.Play();
                _ps.SetParticles(stars, _hipList.Count);
                _ps.Pause();
            }
        }
    }
}
