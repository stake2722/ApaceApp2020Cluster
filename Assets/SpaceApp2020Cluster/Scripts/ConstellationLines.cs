using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Linq;

namespace Constellation
{
    public class ConstellationLines : MonoBehaviour
    {
        [System.Serializable]
        public class GLine
        {
            public Vector3 p0;
            public Vector3 p1;
            public Color color;
            public GLine(Vector3 _p0, Vector3 _p1, Color _color)
            {
                p0 = _p0;
                p1 = _p1;
                color = _color;
            }
        }

        [SerializeField] Hipparcos hipparcosScr = null;
        [SerializeField] bool hideZodiac = true;

        private List<Hipparcos.HipLine> hipLineList;

        void Start()
        {
            string name = "ConstellationLines";
            if (hideZodiac){
                name = "ConstellationLinesWithoutZodiac";
                hipLineList = Hipparcos.GetLineListWithoutZodiac(hipparcosScr.hipLineList);
            }else{
                name = "ZodiacConstellationLines";
                hipLineList = hipparcosScr.hipLineList;
            }
            setMeshLines(hipLineList,name);

        }

        private void Update()
        {
        }

        void setMeshLines(List<Hipparcos.HipLine> _hipLineList, string _name)
        {
            if (_hipLineList != null)
            {
                Vector3[] posArr = new Vector3[_hipLineList.Count * 2];
                for (int i = 0; i < _hipLineList.Count; ++i){
                    posArr[i * 2 + 0] = _hipLineList[i].sttData.direction * hipparcosScr.distance;
                    posArr[i * 2 + 1] = _hipLineList[i].endData.direction * hipparcosScr.distance;
                }
                Mesh mesh = TmLib.TmMesh.CreateLine(posArr, TmLib.TmMesh.LineMeshType.Lines, Color.gray);

#if false
                string path = UnityEditor.AssetDatabase.GenerateUniqueAssetPath("Assets/" + _name + ".asset");
                UnityEditor.AssetDatabase.CreateAsset(mesh, path);
                UnityEditor.AssetDatabase.SaveAssets();
#endif

                transform.GetComponent<MeshFilter>().mesh = mesh;
                transform.GetComponent<MeshRenderer>().material.color = new Color(0.5f,0.5f,1f,1f);
            }
        }

    }
}