using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Constellation
{
    public class ConstellationNames : MonoBehaviour
    {
        public struct NamePosData
        {
            public int constellationId;
            public string shortName;
            public string longName;
            public string jpnName;
            public Vector3 pos;
            public float rot;
            public NamePosData(int _id, Vector3 _pos, float _rot, string _sName, string _lName, string _jName)
            {
                constellationId = _id;
                pos = _pos;
                rot = _rot;
                shortName = _sName;
                longName = _lName;
                jpnName = _jName;
            }
        }

        [SerializeField] TextAsset nameFile = null;
        [SerializeField] TextAsset positionFile = null;
        [SerializeField] GameObject textPrefab = null;
        [SerializeField] Camera targetCamera = null;
        [SerializeField] float distance = 99f;
        List<NamePosData> m_namePosList;
        List<GameObject> m_nameObjList;
        public List<NamePosData> namePosList { get { return m_namePosList; } }
        public List<GameObject> nameObjList { get { return m_nameObjList; } }

        void Awake(){
            m_namePosList = new List<NamePosData>();
            m_nameObjList = new List<GameObject>();
            List<Vector4> posList = createPosList();
            List<string[]> nameList = createNameList();
            // 必ず行数が同じなので手抜き
            for (int i = 0; i < posList.Count; ++i){
                if((nameList.Count>i)&&(nameList[i].Length>=3)){
                    NamePosData data = new NamePosData(i+1,posList[i],posList[i].w,nameList[i][0],nameList[i][1],nameList[i][2]);
                    m_namePosList.Add(data);
                }
            }
        }

        // Use this for initialization
        void Start () {
            foreach (NamePosData data in m_namePosList)
            {
                Vector3 pos = data.pos * distance;
                Quaternion rot = Quaternion.LookRotation(pos);
                GameObject go = Instantiate(textPrefab, pos, rot);
                go.GetComponent<TextMesh>().text = data.longName;
                go.GetComponent<TextMesh>().characterSize = distance * 0.01f;
                go.transform.SetParent(transform);
                go.transform.localPosition = pos;
                go.transform.localRotation = rot * transform.root.rotation;
                m_nameObjList.Add(go);
            }
        }
        
        // Update is called once per frame
        void Update () {
            foreach(GameObject go in m_nameObjList){
                go.transform.rotation = Quaternion.LookRotation(go.transform.position, targetCamera.transform.rotation * Vector3.up);
            }
        }

        List<Vector4> createPosList(){
            List<Vector4> list = new List<Vector4>();
            StringReader sr = new StringReader(positionFile.text);
            while (sr.Peek() > -1)
            {
                string lineStr = sr.ReadLine();
                string[] dataArr = lineStr.Split(',');
    //            int id = int.Parse(dataArr[0]);
                float lH = float.Parse(dataArr[1]);
                float lM = float.Parse(dataArr[2]);
                float lD = lH * (360f / 24f) + lM * (1f / 60f);
                float sD = float.Parse(dataArr[3]);
                Quaternion rotL = Quaternion.AngleAxis(lD, Vector3.up);
                Quaternion rotS = Quaternion.AngleAxis(sD, Vector3.right);
                Vector3 pos = rotL * rotS * Vector3.forward;
                list.Add(new Vector4(pos.x,pos.y,pos.z,0f));
            }
            sr.Close();
            return list;
        }

        List<string[]> createNameList()
        {
            List<string[]> list = new List<string[]>();
            StringReader sr = new StringReader(nameFile.text);
            while (sr.Peek() > -1)
            {
                string lineStr = sr.ReadLine();
                string[] dataArr = lineStr.Split(',');
                if(dataArr.Length>=4){
                    string[] strArr = new string[3];
                    strArr[0] = dataArr[1];
                    strArr[1] = dataArr[2];
                    strArr[2] = dataArr[3];
                    list.Add(strArr);
                }
            }
            sr.Close();
            return list;
        }
    }
}
