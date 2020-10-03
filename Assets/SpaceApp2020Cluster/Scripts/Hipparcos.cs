using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

// http://astronomy.webcrow.jp/hip/
// http://hubblesource.stsci.edu/sources/illustrations/constellations/
// https://stackoverflow.com/questions/21977786/star-b-v-color-index-to-apparent-rgb-color
// http://www.domenavi.com/SearchResults.php?jen1=168&keywrd=%E7%A7%8B%E3%81%AE%E6%98%9F%E5%BA%A7
// http://www.domenavi.com/SearchResults.php?jen2=%E6%98%9F%E5%BA%A7%E3%82%BB%E3%83%83%E3%83%88

namespace Constellation
{
    public class Hipparcos : MonoBehaviour {
        public enum ZodiacType
        {
            None = 0,
            Aries = 1,
            Taurus = 2,
            Gemini = 3,
            Cancer = 4,
            Leo = 5,
            Virgo = 6,
            Libra = 7,
            Scorpius = 8,
            Sagittarius = 9,
            Capricornus = 10,
            Aquarius = 11,
            Pisces = 12,
        }

        static public readonly string[] zodShortNames = { "---", "Ari", "Tau", "Gem", "Cnc", "Leo", "Vir", "Lib", "Sco", "Sgr", "Cap", "Aqr", "Psc" };
        /// <summary> 星座名(略記号)を星座タイプに変換 </summary>
        static public ZodiacType ShortNameToZodiacType(string _zodiacName)
        {
            ZodiacType zod = ZodiacType.None;
            if (_zodiacName.Contains(zodShortNames[1])) { zod = ZodiacType.Taurus; }
            else if (_zodiacName.Contains(zodShortNames[2])) { zod = ZodiacType.Taurus; }
            else if (_zodiacName.Contains(zodShortNames[3])) { zod = ZodiacType.Gemini; }
            else if (_zodiacName.Contains(zodShortNames[4])) { zod = ZodiacType.Cancer; }
            else if (_zodiacName.Contains(zodShortNames[5])) { zod = ZodiacType.Leo; }
            else if (_zodiacName.Contains(zodShortNames[6])) { zod = ZodiacType.Virgo; }
            else if (_zodiacName.Contains(zodShortNames[7])) { zod = ZodiacType.Libra; }
            else if (_zodiacName.Contains(zodShortNames[8])) { zod = ZodiacType.Scorpius; }
            else if (_zodiacName.Contains(zodShortNames[9])) { zod = ZodiacType.Sagittarius; }
            else if (_zodiacName.Contains(zodShortNames[10])) { zod = ZodiacType.Capricornus; }
            else if (_zodiacName.Contains(zodShortNames[11])) { zod = ZodiacType.Aquarius; }
            else if (_zodiacName.Contains(zodShortNames[12])) { zod = ZodiacType.Pisces; }
            return zod;
        }
        static public string ZodiacTypeToShortName(ZodiacType _zodiac){
            return (zodShortNames[(int)_zodiac]);
        }

        [SerializeField] TextAsset lightFile=null;
        [SerializeField] TextAsset appendLightFile = null;
        [SerializeField] TextAsset lineFile=null;
        [SerializeField] float m_distance = 200;
        public float distance { get { return m_distance; } }

        /// <summary> Hipparcos 星データ </summary>
        public struct HipData
        {
            /// Hipparcos星ID
            public int hipId;
            /// 地球からの方向
            public Vector3 direction;
            /// 星の色
            public Color color;
            /// 等級
            public float magnitude;
            /// 視差(≒距離)
            public float parallax;
            public HipData(int _id, Vector3 _direction, Color _color, float _magnitude, float _parallax)
            {
                hipId = _id;
                direction = _direction;
                color = _color;
                magnitude = _magnitude;
                parallax = _parallax;
            }
            public HipData(HipData _data)
            {
                hipId = _data.hipId;
                direction = _data.direction;
                color = _data.color;
                magnitude = _data.magnitude;
                parallax = _data.parallax;
            }
        }

        /// <summary> Hipparcos 星座線データ </summary>
        public struct HipLine
        {
            /// 星座名(略記号)
            public string constellationNameShort;
            /// 始点 Hipparcos星ID
            public HipData sttData;
            /// 終点 Hipparcos星ID
            public HipData endData;
            public HipLine(string _name, HipData _sttData, HipData _endData)
            {
                constellationNameShort = _name;
                sttData = _sttData;
                endData = _endData;
            }
        }

        private List<HipData> m_hipList;
        private List<HipData> m_hipAppendList;
        private List<HipLine> m_hipLineList;
        public List<HipData> hipList { get { return m_hipList; } }
        public List<HipData> hipAppendList { get { return m_hipAppendList; } }
        public List<HipLine> hipLineList { get { return m_hipLineList; } }

        void Awake(){
            m_hipList = createHipList(lightFile);
            m_hipAppendList = createHipList(appendLightFile);
            if(hipAppendList.Count>0){
                // データ数の多い追加ファイルは細かい情報が省かれていることがあるので、すでにデータがある場合は差し替える 
                m_hipAppendList = appendHipList(m_hipAppendList,m_hipList);
            }
            m_hipLineList = createHipLineList(lineFile,m_hipList);
        }

    	// Use this for initialization
    	void Start () {
    	}
    	
    	// Update is called once per frame
    	void Update () {
    	}

        List<HipData> createHipList(TextAsset _lightsFile){
            List<HipData> list = new List<HipData>();
            if(_lightsFile!=null){
                StringReader sr = new StringReader(_lightsFile.text);
                while (sr.Peek() > -1)
                {
                    string lineStr = sr.ReadLine();
                    HipData data;
                    if (stringToHipData(lineStr, out data))
                    {
                        list.Add(data);
                    }
                }
                sr.Close();
            }
            return list;
        }

        List<HipLine> createHipLineList(TextAsset _linesFile, List<HipData> _hipList){
            List<HipLine> list = new List<HipLine>();
            StringReader sr = new StringReader(_linesFile.text);
            while (sr.Peek() > -1)
            {
                string lineStr = sr.ReadLine();
                HipLine data;
                if (stringToHipLine(lineStr, _hipList, out data))
                {
                    list.Add(data);
                }
            }
            sr.Close();
            return list;
        }

        List<HipData> appendHipList(List<HipData> _hipAppendList, List<HipData> _hipList)
        {
            List<HipData> list = _hipAppendList;
            for (int i = 0; i < _hipList.Count; ++i)
            {
                HipData data = _hipList[i];
                bool isFound = false;
                for (int j = 0; j < list.Count; ++j)
                {
                    if (list[j].hipId == data.hipId)
                    {
                        isFound = true;
                        list[j] = data;
                        break;
                    }
                }
                if (!isFound)
                {
                    list.Add(data);
                }
            }
            return list;
        }

        bool stringToHipData(string _hipStr, out HipData data){
            bool ret = true;
            data = new HipData();
            string[] dataArr = _hipStr.Split(',');
            try{
                int hipId = int.Parse(dataArr[0]);
                float hlH = float.Parse(dataArr[1]);
                float hlM = float.Parse(dataArr[2]);
                float hlS = float.Parse(dataArr[3]);
                int hsSgn = int.Parse(dataArr[4]);
                float hsH = float.Parse(dataArr[5]);
                float hsM = float.Parse(dataArr[6]);
                float hsS = float.Parse(dataArr[7]);
                float mag = float.Parse(dataArr[8]);
                float parallax = 0f;
                if (dataArr.Length > 9)
                { // parallax
                   float.TryParse(dataArr[9], out parallax);
                }
                Color col = Color.gray;
                if(dataArr.Length>12){ // B-V色指数
                    float bv = 0f;
                    if(float.TryParse(dataArr[12],out bv)){
                        col = BV2Col(bv);
                    }
                }
                float hDeg = (360f / 24f) * (hlH + hlM / 60f + hlS / 3600f);
                float sDeg = (hsH + hsM / 60f + hsS / 3600f) * (hsSgn==0?-1f:1f);
                Quaternion rotL = Quaternion.AngleAxis(hDeg, Vector3.up);
                Quaternion rotS = Quaternion.AngleAxis(sDeg, Vector3.right);
                Vector3 pos = rotL * rotS * Vector3.forward;
                data = new HipData(hipId, pos, col, mag,parallax);
            }catch{
                ret = false;
                Debug.Log("dataerr");
            }
            return ret;
        }

        bool stringToHipLine(string _hipLineStr, List<HipData> _hipList, out HipLine data){
            bool ret = true;
            data = new HipLine();
            string[] dataArr = _hipLineStr.Split(',');
            string shortName = dataArr[0];
            try{
                int sttId = int.Parse(dataArr[1]);
                int endId = int.Parse(dataArr[2]);
                HipData sttData = _hipList.First(d => (d.hipId == sttId));
                HipData endData = _hipList.First(d => (d.hipId == endId));
                data = new HipLine(shortName, sttData, endData);
            }catch{
                ret = false;
                Debug.Log("linedataerr:"+shortName);
            }
            return ret;
        }

        /// <summary> bv_colorをRGBに変換 
        /// https://stackoverflow.com/questions/21977786/star-b-v-color-index-to-apparent-rgb-color
        /// </summary>
        static Color BV2Col(float _bv){
            Color col = Color.white;
            float t = 4600f * ((1 / ((0.92f * _bv) + 1.7f)) + (1f / ((0.92f * _bv) + 0.62f)));

            // t to xyY
            float x=0f, y=0f;
            if (t >= 1667f && t <= 4000f){
                x = ((-0.2661239f * Mathf.Pow(10f, 9f)) / Mathf.Pow(t, 3f)) + ((-0.2343580f * Mathf.Pow(10, 6)) / Mathf.Pow(t, 2f)) + ((0.8776956f * Mathf.Pow(10f, 3f)) / t) + 0.179910f;
            }
            else if (t > 4000f && t <= 25000f){
                x = ((-3.0258469f * Mathf.Pow(10f, 9f)) / Mathf.Pow(t, 3f)) + ((2.1070379f * Mathf.Pow(10f, 6f)) / Mathf.Pow(t, 2f)) + ((0.2226347f * Mathf.Pow(10f, 3f)) / t) + 0.240390f;
            }

            if (t >= 1667f && t <= 2222f){
                y = -1.1063814f * Mathf.Pow(x, 3f) - 1.34811020f * Mathf.Pow(x, 2f) + 2.18555832f * x - 0.20219683f;
            }
            else if (t > 2222f && t <= 4000f){
                y = -0.9549476f * Mathf.Pow(x, 3f) - 1.37418593f * Mathf.Pow(x, 2f) + 2.09137015f * x - 0.16748867f;
            }
            else if (t > 4000f && t <= 25000f){
                y = 3.0817580f * Mathf.Pow(x, 3f) - 5.87338670f * Mathf.Pow(x, 2f) + 3.75112997f * x - 0.37001483f;
            }

            // xyY to XYZ, Y = 1
            float Y = (y == 0f) ? 0f : 1f;
            float X = (y == 0f) ? 0f : (x * Y) / y;
            float Z = (y == 0f) ? 0f : ((1f - x - y) * Y) / y;

            // XYZ to RGB
            col.r = 0.41847f * X - 0.15866f * Y - 0.082835f * Z;
            col.g = -0.091169f * X + 0.25243f * Y + 0.015708f * Z;
            col.b = 0.00092090f * X - 0.0025498f * Y + 0.17860f * Z;
            col.a = 1f;
            return col;
        }

        /// <summary> Hipparcos 星座に含まれる星データを取得 </summary>
        static public List<HipData> GetZodiacHipList(List<HipLine> _lineList, string _constellationNameShort){
            List<HipLine> _zodiacLineList = GetLineList(_lineList, _constellationNameShort);
            return GetHipListFromLineList(_zodiacLineList);
        }

        /// <summary> Hipparcos 星座線データに含まれる星データを取得 </summary>
        static public List<HipData> GetHipListFromLineList(List<HipLine> _hipLineList)
        {
            List<HipData> retHipList = new List<HipData>();
            if (_hipLineList.Count > 0)
            {
                Dictionary<int, Vector3> starDic = new Dictionary<int, Vector3>();
                for (int i = 0; i < _hipLineList.Count; ++i)
                {
                    if (!starDic.ContainsKey(_hipLineList[i].sttData.hipId))
                    {
                        starDic.Add(_hipLineList[i].sttData.hipId, _hipLineList[i].sttData.direction);
                        retHipList.Add(new HipData(_hipLineList[i].sttData));
                    }
                    if (!starDic.ContainsKey(_hipLineList[i].endData.hipId))
                    {
                        starDic.Add(_hipLineList[i].endData.hipId, _hipLineList[i].endData.direction);
                        retHipList.Add(new HipData(_hipLineList[i].endData));
                    }
                }
            }
            return retHipList;
        }

        /// <summary> 指定した星座の星座線データを取得 </summary>
        static public List<HipLine> GetLineList(List<HipLine> _lineList, string _constellationNameShort)
        {
            List<HipLine> retList = new List<HipLine>();
            if (_lineList != null)
            {
                for (int i = 0; i < _lineList.Count; ++i)
                {
                    if (_lineList[i].constellationNameShort == _constellationNameShort)
                    {
                        retList.Add(_lineList[i]);
                    }
                }
            }
            return retList;
        }

        static private List<HipLine> getLineListWithoutOrOnlyZodiac(List<HipLine> _lineList, bool _isOnly)
        {
            List<HipLine> retList = new List<HipLine>();
            if (_lineList != null){
                for (int i = 0; i < _lineList.Count; ++i){
                    if (zodShortNames.Contains(_lineList[i].constellationNameShort)==_isOnly){
                        retList.Add(_lineList[i]);
                    }
                }
            }
            return retList;
        }

        /// <summary> 12星座を除く星座線データを取得 </summary>
        static public List<HipLine> GetLineListWithoutZodiac(List<HipLine> _lineList)
        {
            return getLineListWithoutOrOnlyZodiac(_lineList, false);
        }

        /// <summary> 12星座を除く星座線データを取得 </summary>
        static public List<HipData> GetHipListWithoutZodiac(List<HipData> _baseHipList, List<HipLine> _lineList)
        {
            List<HipData> retHipList = new List<HipData>();
            List<HipLine> onlyZodiacLineList = getLineListWithoutOrOnlyZodiac(_lineList, true);
            List<HipData> onlyZodiacHipList = GetHipListFromLineList(onlyZodiacLineList);
            foreach(HipData baseData in _baseHipList){
                HipData findData = onlyZodiacHipList.FirstOrDefault(d => (d.hipId == baseData.hipId) );
                if(findData.hipId!=baseData.hipId){
                    retHipList.Add(baseData);
                }
            }
            return retHipList;
        }

        /// <summary> 星リストの重心を取得 </summary>
        static public Vector3 GetCenterOfGravity(List<Hipparcos.HipData> _hipList)
        {
            Vector3 dir = Vector3.zero;
            for (int i = 0; i < _hipList.Count; ++i)
            {
                dir += _hipList[i].direction;
            }
            return dir / (float)_hipList.Count;
        }

        /// <summary> 指定した星座の重心を取得 </summary>
        static public Vector3 GetCenterOfGravity(List<Hipparcos.HipLine> _lineList, string _constellationNameShort)
        {
            List<Hipparcos.HipLine> hipLineList = Hipparcos.GetLineList(_lineList, _constellationNameShort);
            List<Hipparcos.HipData> hipList = Hipparcos.GetHipListFromLineList(hipLineList);
            return GetCenterOfGravity(hipList);
        }

        /// <summary> 視野角内の星リストを取得 </summary>
        static public List<Hipparcos.HipData> GetHipListInAngle(List<HipData> _hipList, Vector3 _centerDir, float _angle)
        {
            List<HipData> retHipList = new List<Hipparcos.HipData>();
            if (_hipList.Count > 0){
                for (int i = 0; i < _hipList.Count; ++i){
                    Vector3 tgtDir = _hipList[i].direction;
                    float angle = Vector3.Angle(tgtDir, _centerDir);
                    if (angle < _angle){
                        retHipList.Add(_hipList[i]);
                    }
                }
            }
            return retHipList;
        }

        /// <summary>
        /// Gets the hip data by identifier.
        /// </summary>
        /// <returns><c>true</c>, if hip data by identifier was gotten, <c>false</c> otherwise.</returns>
        /// <param name="_hipList">Hip list.</param>
        /// <param name="_hipId">Hip identifier.</param>
        /// <param name="_data">Data.</param>
        static public bool GetHipDataByID(List<HipData> _hipList, int _hipId, out HipData _data){
            bool ret = false;
            _data = new HipData();
            HipData retData = _hipList.FirstOrDefault(d => (d.hipId == _hipId));
            if(retData.hipId==_hipId){
                ret = true;
                _data = retData;
            }
            return ret;
        }

    }
}