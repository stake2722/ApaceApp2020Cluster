﻿using UnityEngine;
using System.Collections;

namespace TmLib
{
    public class TmMesh
    {
        public enum AxisType { XY, XZ }
        public static Mesh CreatePointCloud(Vector3[] _vertices, Color _cvtxCol)
        {
            Color[] _cvtxCols = new Color[] { _cvtxCol };
            return CreatePointCloud(_vertices, _cvtxCols);
        }
        public static Mesh CreatePointCloud(Vector3[] _vertices, Color[] _cvtxCols)
        {
            int vertNum = _vertices.Length;
            int[] incides = new int[vertNum];
            Vector2[] uv = new Vector2[vertNum];
            Color[] colors = new Color[vertNum];
            Vector3[] normals = new Vector3[vertNum];

            for (int ii = 0; ii < _vertices.Length; ++ii)
            {
                incides[ii] = ii;
                uv[ii] = new Vector2(_vertices[ii].x + 0.5f, _vertices[ii].y + 0.5f);
                colors[ii] = _cvtxCols[(_cvtxCols.Length > ii) ? ii : 0];
                normals[ii] = -_vertices[ii].normalized;
            }
            Mesh mesh = new Mesh();
            mesh.vertices = _vertices;
            mesh.uv = uv;
            mesh.colors = colors;
            mesh.normals = normals;
            //mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.SetIndices(incides, MeshTopology.Points, 0);
            return mesh;
        }

        public static Mesh CreateQuadCloud(Vector3[] _vertices, Color[] _cvtxCols, float _sizeRate = 0.02f, bool _scaleByGrayScale=false)
        {
            int vertNum = _vertices.Length;
            int[] incides = new int[vertNum * 4];
            Vector2[] uv = new Vector2[vertNum * 4];
            Color[] colors = new Color[vertNum * 4];
            Vector3[] normals = new Vector3[vertNum * 4];
            Vector3[] quadVertices = new Vector3[vertNum * 4];

            Vector2[] ofsUv = new Vector2[4];
            ofsUv[0] = new Vector2(0f, 1f);
            ofsUv[1] = new Vector2(1f, 1f);
            ofsUv[2] = new Vector2(1f, 0f);
            ofsUv[3] = new Vector2(0f, 0f);
            float quadScale = _sizeRate;
            for (int ii = 0; ii < _vertices.Length; ++ii)
            {
                if (_scaleByGrayScale)
                {
                    quadScale = _sizeRate * colors[ii].grayscale;
                }
                Quaternion dirRot = Quaternion.LookRotation(_vertices[ii]);
                Vector3[] ofsDir = new Vector3[4];
                ofsDir[0] = dirRot * new Vector3(-0.5f,0.5f,0f);
                ofsDir[1] = dirRot * new Vector3(0.5f, 0.5f, 0f);
                ofsDir[2] = dirRot * new Vector3(0.5f, -0.5f, 0f);
                ofsDir[3] = dirRot * new Vector3(-0.5f, -0.5f, 0f);
                float dist = _vertices[ii].magnitude;
                for (int qi = 0; qi < 4; ++qi)
                {
                    incides[ii * 4 + qi] = ii * 4 + qi;
                    quadVertices[ii * 4 + qi] = _vertices[ii] + ofsDir[qi] * dist * quadScale;
                    uv[ii * 4 + qi] = ofsUv[qi];
                    colors[ii * 4 + qi] = _cvtxCols[(_cvtxCols.Length > ii) ? ii : 0];
                    normals[ii * 4 + qi] = -_vertices[ii].normalized; // to inside
                }
            }
            Mesh mesh = new Mesh();
            mesh.vertices = quadVertices;
            mesh.uv = uv;
            mesh.colors = colors;
            mesh.normals = normals;
            mesh.SetIndices(incides, MeshTopology.Quads, 0);
            //mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        public static Mesh CreateLineGrid(int _width, int _height, AxisType _type)
        {
            return CreateLineGrid(_width, _height, _type, new Color(0.5f, 0.5f, 0.5f, 1.0f), false);
        }
        public static Mesh CreateLineGrid(int _width, int _height, AxisType _type, Color _vertCol, bool _isUnitPerGrid)
        {
            Vector3[] vertices = new Vector3[(_width + 1) * (_height + 1) * 2];
            int[] incides = new int[(_width + 1) * (_height + 1) * 2];
            Vector2[] uv = new Vector2[(_width + 1) * (_height + 1) * 2];
            Color[] colors = new Color[(_width + 1) * (_height + 1) * 2];
            Vector3[] normals = new Vector3[(_width + 1) * (_height + 1) * 2];
            Vector4[] tangents = new Vector4[(_width + 1) * (_height + 1) * 2];

            int cnt = 0;
            for (int ix = 0; ix <= _width; ++ix)
            {
                vertices[cnt * 2 + 0] = new Vector3(((float)ix / (float)_width - 0.5f), -0.5f, 0.0f);
                vertices[cnt * 2 + 1] = new Vector3(((float)ix / (float)_width - 0.5f), 0.5f, 0.0f);
                if (_isUnitPerGrid)
                {
                    vertices[cnt * 2 + 0] = Vector3.Scale(vertices[cnt * 2 + 0], new Vector3(_width, _height, 0));
                    vertices[cnt * 2 + 1] = Vector3.Scale(vertices[cnt * 2 + 1], new Vector3(_width, _height, 0));
                }
                incides[cnt * 2 + 0] = cnt * 2 + 0;
                incides[cnt * 2 + 1] = cnt * 2 + 1;
                uv[cnt * 2 + 0] = new Vector2(vertices[cnt * 2 + 0].x + 0.5f, vertices[cnt * 2 + 0].y + 0.5f);
                uv[cnt * 2 + 1] = new Vector2(vertices[cnt * 2 + 1].x + 0.5f, vertices[cnt * 2 + 1].y + 0.5f);
                colors[cnt * 2 + 0] = colors[cnt * 2 + 1] = new Color(0.5f, 0.5f, 0.5f, 1.0f);
                normals[cnt * 2 + 0] = normals[cnt * 2 + 1] = new Vector3(0.0f, 0.0f, -1.0f);
                tangents[cnt * 2 + 0] = tangents[cnt * 2 + 1] = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
                cnt++;
            }
            for (int iy = 0; iy <= _height; ++iy)
            {
                vertices[cnt * 2 + 0] = new Vector3(-0.5f, ((float)iy / (float)_height - 0.5f), 0.0f);
                vertices[cnt * 2 + 1] = new Vector3(0.5f, ((float)iy / (float)_height - 0.5f), 0.0f);
                if (_isUnitPerGrid)
                {
                    vertices[cnt * 2 + 0] = Vector3.Scale(vertices[cnt * 2 + 0], new Vector3(_width, _height, 0));
                    vertices[cnt * 2 + 1] = Vector3.Scale(vertices[cnt * 2 + 1], new Vector3(_width, _height, 0));
                }
                incides[cnt * 2 + 0] = cnt * 2 + 0;
                incides[cnt * 2 + 1] = cnt * 2 + 1;
                uv[cnt * 2 + 0] = new Vector2(vertices[cnt * 2 + 0].x + 0.5f, vertices[cnt * 2 + 0].y + 0.5f);
                uv[cnt * 2 + 1] = new Vector2(vertices[cnt * 2 + 1].x + 0.5f, vertices[cnt * 2 + 1].y + 0.5f);
                colors[cnt * 2 + 0] = colors[cnt * 2 + 1] = new Color(0.5f, 0.5f, 0.5f, 1.0f);
                normals[cnt * 2 + 0] = normals[cnt * 2 + 1] = new Vector3(0.0f, 0.0f, -1.0f);
                tangents[cnt * 2 + 0] = tangents[cnt * 2 + 1] = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
                cnt++;
            }
            if (_type == AxisType.XZ)
            {
                for (int ii = 0; ii < vertices.Length; ++ii)
                {
                    vertices[ii] = new Vector3(vertices[ii].x, vertices[ii].z, vertices[ii].y);
                }
                for (int ii = 0; ii < normals.Length; ++ii)
                {
                    normals[ii] = new Vector3(0.0f, 1.0f, 0.0f);
                }
            }
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.SetIndices(incides, MeshTopology.Lines, 0);
            mesh.uv = uv;
            mesh.colors = colors;
            mesh.normals = normals;
            mesh.tangents = tangents;
            //		mesh.RecalculateNormals ();
            mesh.RecalculateBounds();
            return mesh;
        }

        public static Mesh CreateLineGridXY(int _width, int _height)
        {
            return CreateLineGridXY(_width, _height, new Color(0.5f, 0.5f, 0.5f, 1.0f), false);
        }
        public static Mesh CreateLineGridXY(int _width, int _height, Color _vertCol, bool _isUnitPerGrid)
        {
            return CreateLineGrid(_width, _height, AxisType.XY, new Color(0.5f, 0.5f, 0.5f, 1.0f), false);
        }

        public enum LineMeshType
        {
            LineStrip,
            Ring,
            Lines
        };
        public static Mesh CreateLine(Vector3[] _vertices, bool _isRing)
        {
            return CreateLine(_vertices, LineMeshType.LineStrip, new Color(0.5f, 0.5f, 0.5f, 1.0f));
        }
        public static Mesh CreateLine(Vector3[] _vertices, LineMeshType _lineMeshType, Color _color)
        {
            bool isRing = (_lineMeshType == LineMeshType.Ring);
            MeshTopology topology = (_lineMeshType == LineMeshType.Lines) ? MeshTopology.Lines : MeshTopology.LineStrip;
            int vertNum = _vertices.Length;
            int[] incides = new int[isRing ? vertNum + 1 : vertNum];
            Vector2[] uv = new Vector2[vertNum];
            Color[] colors = new Color[vertNum];
            Vector3[] normals = new Vector3[vertNum];

            for (int ii = 0; ii < _vertices.Length; ++ii)
            {
                Vector3 normal = new Vector3(0.0f, 1.0f, 0.0f);
                if (ii < (_vertices.Length - 1))
                {
                    normal = _vertices[ii + 1] - _vertices[ii];
                }
                incides[ii] = ii;
                uv[ii] = new Vector2(_vertices[ii].x + 0.5f, _vertices[ii].y + 0.5f);
                colors[ii] = _color;
                normals[ii] = normal;
            }
            if (isRing)
            {
                incides[vertNum] = 0;
            }
            Mesh mesh = new Mesh();
            mesh.vertices = _vertices;
            mesh.uv = uv;
            mesh.colors = colors;
            mesh.normals = normals;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.SetIndices(incides, topology, 0);
            return mesh;
        }

        public static Mesh CreateLineCircle(int _vertNum, AxisType _type = AxisType.XY)
        {
            return CreateLineCircle(_vertNum, 0.0f, _type, new Color(0.5f, 0.5f, 0.5f, 1.0f));
        }
        public static Mesh CreateLineCircle(int _vertNum, float _ofsDeg, AxisType _type, Color _color, float _starRate = 0.0f)
        {
            if (_starRate != 0.0f) _vertNum *= 2;
            Vector3[] vertices = new Vector3[_vertNum];
            for (int ii = 0; ii < _vertNum; ++ii)
            {
                float rr = 0.5f;
                if ((_starRate != 0.0f) && ((ii & 1) == 1))
                {
                    rr *= (1.0f - _starRate);
                }
                float deg = Mathf.PI * 2.0f * ((float)ii / (float)_vertNum) + _ofsDeg;
                float fx = Mathf.Cos(deg) * rr;
                float fy = Mathf.Sin(deg) * rr;
                switch (_type)
                {
                    default: vertices[ii] = new Vector3(fx, fy, 0.0f); break;
                    case AxisType.XY: vertices[ii] = new Vector3(fx, fy, 0.0f); break;
                    case AxisType.XZ: vertices[ii] = new Vector3(fx, 0.0f, fy); break;
                }
            }
            return CreateLine(vertices, LineMeshType.Ring, _color);
        }

        public static Mesh CreateTileMesh(int _divX, int _divY, AxisType _type = AxisType.XY)
        {
            return CreateTileMesh(_divX, _divY, _type, new Color(0.5f, 0.5f, 0.5f, 1.0f), false);
        }
        public static Mesh CreateTileMesh(int _divX, int _divY, AxisType _type, Color _vertCol, bool _isUnitPerGrid)
        {
            int vertNum = (_divX + 1) * (_divY + 1);
            int quadNum = _divX * _divY;
            int[] triangles = new int[quadNum * 6];
            Vector3[] vertices = new Vector3[vertNum];
            Vector2[] uv = new Vector2[vertNum];
            Color[] colors = new Color[vertNum];
            Vector3[] normals = new Vector3[vertNum];
            Vector4[] tangents = new Vector4[vertNum];

            for (int yy = 0; yy < (_divY + 1); ++yy)
            {
                for (int xx = 0; xx < (_divX + 1); ++xx)
                {
                    Vector2 uvPos = new Vector2((float)xx / (float)_divX, (float)yy / (float)_divY);
                    float fx = uvPos.x - 0.5f;
                    float fy = uvPos.y - 0.5f;
                    switch (_type)
                    {
                        default: vertices[yy * (_divX + 1) + xx] = new Vector3(fx, fy, 0.0f); break;
                        case AxisType.XY: vertices[yy * (_divX + 1) + xx] = new Vector3(fx, fy, 0.0f); break;
                        case AxisType.XZ: vertices[yy * (_divX + 1) + xx] = new Vector3(fx, 0.0f, fy); break;
                    }
                    if (_isUnitPerGrid)
                    {
                        switch (_type)
                        {
                            default: vertices[yy * (_divX + 1) + xx] = Vector3.Scale(vertices[yy * (_divX + 1) + xx], new Vector3(_divX, _divY, 1)); break;
                            case AxisType.XY: vertices[yy * (_divX + 1) + xx] = Vector3.Scale(vertices[yy * (_divX + 1) + xx], new Vector3(_divX, _divY, 1)); break;
                            case AxisType.XZ: vertices[yy * (_divX + 1) + xx] = Vector3.Scale(vertices[yy * (_divX + 1) + xx], new Vector3(_divX, 1, _divY)); break;
                        }
                    }
                    uv[yy * (_divX + 1) + xx] = uvPos;
                    colors[yy * (_divX + 1) + xx] = _vertCol;
                    normals[yy * (_divX + 1) + xx] = new Vector3(0.0f, 0.0f, -1.0f);
                    tangents[yy * (_divX + 1) + xx] = new Vector4(1.0f, 0.0f, 0.0f);
                    if ((xx < _divX) && (yy < _divY))
                    {
                        int[] sw = { 0, 0, 1, 1, 1, 0, 1, 1, 0, 0, 0, 1 };
                        for (int ii = 0; ii < 6; ++ii)
                        {
                            triangles[(yy * _divX + xx) * 6 + ii] = (yy + sw[ii * 2 + 1]) * (_divX + 1) + (xx + sw[ii * 2 + 0]);
                        }
                    }
                }
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;
            mesh.colors = colors;
            mesh.normals = normals;
            mesh.tangents = tangents;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            ;
            mesh.SetIndices(mesh.GetIndices(0), MeshTopology.Triangles, 0);
            return mesh;
        }

        public static Mesh CreateHeightMesh(float[,] _heightArr)
        {
            return CreateHeightMesh(_heightArr, new Color(0.5f, 0.5f, 0.5f, 1.0f), false);
        }
        public static Mesh CreateHeightMesh(float[,] _heightArr, Color _vertCol, bool _isUnitPerGrid)
        {
            int _divX = _heightArr.GetLength(0) - 1;
            int _divZ = _heightArr.GetLength(1) - 1;

            int vertNum = (_divX + 1) * (_divZ + 1);
            int quadNum = _divX * _divZ;
            int[] triangles = new int[quadNum * 6];
            Vector3[] vertices = new Vector3[vertNum];
            Vector2[] uv = new Vector2[vertNum];
            Color[] colors = new Color[vertNum];
            Vector3[] normals = new Vector3[vertNum];
            Vector4[] tangents = new Vector4[vertNum];

            for (int zz = 0; zz < (_divZ + 1); ++zz)
            {
                for (int xx = 0; xx < (_divX + 1); ++xx)
                {
                    float height = _heightArr[xx, zz];
                    Vector2 uvPos = new Vector2((float)xx / (float)_divX, (float)zz / (float)_divZ);
                    vertices[zz * (_divX + 1) + xx] = new Vector3(uvPos.x - 0.5f, height, uvPos.y - 0.5f);
                    if (_isUnitPerGrid)
                    {
                        vertices[zz * (_divX + 1) + xx] = Vector3.Scale(vertices[zz * (_divX + 1) + xx], new Vector3(_divX, 1f, _divZ));
                    }
                    uv[zz * (_divX + 1) + xx] = uvPos;
                    colors[zz * (_divX + 1) + xx] = _vertCol;
                    normals[zz * (_divX + 1) + xx] = new Vector3(0.0f, 1.0f, 0.0f);
                    tangents[zz * (_divX + 1) + xx] = new Vector4(1.0f, 0.0f, 0.0f);
                    if ((xx < _divX) && (zz < _divZ))
                    {
                        int[] sw = { 0, 0, 1, 1, 1, 0, 1, 1, 0, 0, 0, 1 };
                        for (int ii = 0; ii < 6; ++ii)
                        {
                            triangles[(zz * _divX + xx) * 6 + ii] = (zz + sw[ii * 2 + 1]) * (_divX + 1) + (xx + sw[ii * 2 + 0]);
                        }
                    }
                }
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;
            mesh.colors = colors;
            mesh.normals = normals;
            mesh.tangents = tangents;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            ;
            mesh.SetIndices(mesh.GetIndices(0), MeshTopology.Triangles, 0);
            return mesh;
        }

        public static Mesh CreateTubeMesh(int _divX, int _divZ, AxisType _type = AxisType.XY, bool _isInner = false)
        {
            AnimationCurve curve = AnimationCurve.Linear(-0.5f, 0.5f, 0.5f, 0.5f);
            return CreateTubeMesh(_divX, _divZ, curve, _type, new Color(0.5f, 0.5f, 0.5f, 1.0f), _isInner);
        }
        public static Mesh CreateTubeMesh(int _divX, int _divZ, AnimationCurve _cv, AxisType _type, Color _vertCol, bool _isInner)
        {
            return CreateTubeMesh(_divX, _divZ, new Rect(0, 0, 1, 1), _cv, _type, _vertCol, _isInner);
        }
        public static Mesh CreateTubeMesh(int _divX, int _divZ, float _sttDeg, float _sizeDeg, AnimationCurve _cv, AxisType _type, Color _vertCol, bool _isInner, Transform[] _boneTr = null)
        {
            return CreateTubeMesh(_divX, _divZ, _sttDeg, _sizeDeg, new Rect(0, 0, 1, 1), _cv, _type, _vertCol, _isInner, _boneTr);
        }
        public static Mesh CreateTubeMesh(int _divX, int _divZ, Rect _uvRect, AnimationCurve _cv, AxisType _type, Color _vertCol, bool _isInner, Transform[] _boneTr = null)
        {
            return CreateTubeMesh(_divX, _divZ, 0f, 360f, _uvRect, _cv, _type, _vertCol, _isInner, _boneTr);
        }
        public static Mesh CreateTubeMesh(int _divX, int _divZ, float _sttDeg, float _sizeDeg, Rect _uvRect, AnimationCurve _cv, AxisType _type, Color _vertCol, bool _isInner, Transform[] _boneTr)
        {
            int boneNum = (_boneTr != null) ? _boneTr.Length : 0;
            int nDiv = (_cv.length - 1) * _divZ;
            float sttTime = _cv.keys[0].time;
            float endTime = _cv.keys[_cv.length - 1].time;
            float ttlTIme = endTime - sttTime;
            float sttRad = _sttDeg * Mathf.PI * 2f;
            float sizeRadRate = _sizeDeg / 360f;
            int vertNum = (_divX + 1) * (nDiv + 1);
            int quadNum = _divX * nDiv;
            int[] triangles = new int[quadNum * 6];
            Vector3[] vertices = new Vector3[vertNum];
            Vector2[] uv = new Vector2[vertNum];
            Color[] colors = new Color[vertNum];
            Vector3[] normals = new Vector3[vertNum];
            Vector4[] tangents = new Vector4[vertNum];
            BoneWeight[] weights = new BoneWeight[vertNum];

            for (int zz = 0; zz < (nDiv + 1); ++zz)
            {
                for (int xx = 0; xx < (_divX + 1); ++xx)
                {
                    float tt = _cv.keys[zz / _divZ].time;
                    if (zz < nDiv)
                    {
                        tt += (_cv.keys[(zz / _divZ) + 1].time - tt) * ((float)(zz % _divZ) / (float)_divZ);
                    }
                    float nz = (tt - sttTime) / ttlTIme;
                    Vector2 uvPos = new Vector2((float)xx / (float)_divX, nz);

                    if (_type == AxisType.XY)
                    {
                        vertices[zz * (_divX + 1) + xx] = new Vector3(uvPos.x - 0.5f, 0.0f, tt);
                    }
                    else
                    {
                        vertices[zz * (_divX + 1) + xx] = new Vector3(uvPos.x - 0.5f, tt, 0.0f);
                    }
                    if (!_isInner)
                    {
                        uvPos.x = 1f - uvPos.x;
                    }
                    uvPos.x = _uvRect.x + (0.01f + uvPos.x * 0.98f) * _uvRect.width;
                    uvPos.y = _uvRect.y + (0.01f + uvPos.y * 0.98f) * _uvRect.height;
                    uv[zz * (_divX + 1) + xx] = uvPos;
                    if (!_isInner)
                    {
                        //						uv[zz*(_divX+1)+xx].x = 1f-uv[zz*(_divX+1)+xx].x;
                    }
                    colors[zz * (_divX + 1) + xx] = _vertCol;
                    normals[zz * (_divX + 1) + xx] = new Vector3(0.0f, 0.0f, 1.0f);
                    tangents[zz * (_divX + 1) + xx] = new Vector4(1.0f, 0.0f, 0.0f);
                    {
                        int axx = (xx == _divX) ? 0 : xx;
                        float p = vertices[zz * (_divX + 1) + axx].x * 2.0f * Mathf.PI;
                        float nowRad = sttRad + p * sizeRadRate;
                        //						float r = _cv.Evaluate(sttTime + ttlTIme*(nz/ttlTIme));
                        float r = _cv.Evaluate(tt);
                        vertices[zz * (_divX + 1) + axx].x = Mathf.Cos(nowRad) * r;
                        if (_type == AxisType.XY)
                        {
                            vertices[zz * (_divX + 1) + axx].y = Mathf.Sin(nowRad) * r;
                        }
                        else
                        {
                            vertices[zz * (_divX + 1) + axx].z = -Mathf.Sin(nowRad) * r;
                        }
                        normals[zz * (_divX + 1) + xx] = vertices[zz * (_divX + 1) + axx].normalized;
                        if (!_isInner)
                        {
                            normals[zz * (_divX + 1) + xx] *= -1f;
                        }
                    }
                    if ((xx < _divX) && (zz < nDiv))
                    {
                        int[] sw = { 0, 0, 1, 1, 1, 0, 1, 1, 0, 0, 0, 1 };
                        int[] sw0 = { 0, 0, -_divX + 1, 1, -_divX + 1, 0, -_divX + 1, 1, 0, 0, 0, 1 };
                        for (int ii = 0; ii < 6; ++ii)
                        {
                            int[] osw = (xx == _divX - 1) ? sw0 : sw;
                            if (!_isInner)
                            {
                                triangles[(zz * _divX + xx) * 6 + ii] = (zz + osw[ii * 2 + 0]) * (_divX + 1) + (xx + osw[ii * 2 + 1]);
                            }
                            else
                            {
                                triangles[(zz * _divX + xx) * 6 + ii] = (zz + osw[ii * 2 + 1]) * (_divX + 1) + (xx + osw[ii * 2 + 0]);
                            }
                        }
                    }
                    BoneWeight wt = new BoneWeight();
                    if (boneNum > 1)
                    {
                        float boneRate = (float)(boneNum - 1) * nz; //0f - (boneNum-1)
                        float boneId = Mathf.Min(Mathf.Floor(boneRate), (boneNum - 2)); //0,1,,(boneNum-2)
                        float rate = (boneRate - boneId); // 0,,1,0,,1, (boneNum-1 times)
                        wt.boneIndex0 = (int)boneId;
                        wt.boneIndex1 = (int)boneId + 1;
                        wt.weight0 = (1 - rate); //Mathf.Clamp01((1-rate)*2f-0.5f);
                        wt.weight1 = rate; //Mathf.Clamp01(rate*2f-0.5f);
                    }
                    weights[zz * (_divX + 1) + xx] = wt;
                }
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.boneWeights = weights;
            mesh.triangles = triangles;
            mesh.uv = uv;
            mesh.colors = colors;
            mesh.normals = normals;
            mesh.tangents = tangents;

            if (boneNum > 0)
            {
                Matrix4x4[] bindPoseArr = new Matrix4x4[boneNum];
                for (int bi = 0; bi < boneNum; ++bi)
                {
                    bindPoseArr[bi] = _boneTr[bi].worldToLocalMatrix * _boneTr[0].localToWorldMatrix;
                }
                mesh.bindposes = bindPoseArr;
            }

            mesh.SetIndices(mesh.GetIndices(0), MeshTopology.Triangles, 0);
            ;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            normals = mesh.normals;
            for (int zz = 0; zz < (_divZ + 1); ++zz)
            {
                Vector3 nml = (normals[zz * (_divX + 1) + 0] + normals[zz * (_divX + 1) + _divX]).normalized;
                normals[zz * (_divX + 1) + 0] = nml;
                normals[zz * (_divX + 1) + _divX] = nml;
            }
            mesh.normals = normals;

            return mesh;
        }

        public static Mesh CreatePoly(int _vertNum, AxisType _type = AxisType.XY, float _ofsDeg = 0.0f)
        {
            return CreatePoly(_vertNum, _type, new Color(0.5f, 0.5f, 0.5f, 1.0f), _ofsDeg, 0.0f);
        }
        public static Mesh CreatePoly(int _vertNum, AxisType _type, Color _color, float _ofsDeg = 0.0f, float _starRate = 0.0f)
        {
            if (_starRate != 0.0f) _vertNum *= 2;
            float ofsDegRate = _ofsDeg / 360.0f;
            Vector3[] verts = new Vector3[_vertNum + 1];
            Vector2[] uvs = new Vector2[_vertNum + 1];
            Vector3[] norms = new Vector3[_vertNum + 1];
            Vector4[] tgts = new Vector4[_vertNum + 1];
            Color[] cols = new Color[_vertNum + 1];
            verts[0] = new Vector3(0.0f, 0.0f, 0.0f);
            uvs[0] = new Vector2(0.5f, 0.5f);
            norms[0] = new Vector3(0.0f, 0.0f, -1.0f);
            tgts[0] = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
            cols[0] = _color;
            for (int ii = 0; ii < _vertNum; ++ii)
            {
                float rr = 0.5f;
                if ((_starRate != 0.0f) && ((ii & 1) == 1))
                {
                    rr *= (1.0f - _starRate);
                }
                float fx = Mathf.Cos(Mathf.PI * 2.0f * (((float)ii / (float)_vertNum) + ofsDegRate)) * rr;
                float fy = Mathf.Sin(Mathf.PI * 2.0f * (((float)ii / (float)_vertNum) + ofsDegRate)) * rr;
                if (_type == AxisType.XY)
                {
                    verts[ii + 1] = new Vector3(fx, fy, 0.0f);
                    norms[ii + 1] = new Vector3(0.0f, 0.0f, -1.0f);
                }
                else
                {
                    verts[ii + 1] = new Vector3(fx, 0.0f, fy);
                    norms[ii + 1] = new Vector3(0.0f, -1.0f, 0.0f);
                }
                uvs[ii + 1] = new Vector2(fx + 0.5f, fy + 0.5f);
                cols[ii + 1] = _color;
                tgts[ii + 1] = new Vector4(fx, fy, 0.0f, 0.0f);
            }

            int[] tris = new int[_vertNum * 3];
            for (int ii = 0; ii < _vertNum; ++ii)
            {
                tris[ii * 3 + 0] = 0;
                tris[ii * 3 + 1] = (ii < (_vertNum - 1)) ? (ii + 2) : 1;
                tris[ii * 3 + 2] = ii + 1;
            }

            Mesh mesh = new Mesh();
            mesh.vertices = verts;
            mesh.triangles = tris;
            mesh.uv = uvs;
            mesh.colors = cols;
            mesh.normals = norms;
            mesh.tangents = tgts;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            ;
            return mesh;
        }

        public static Mesh CreatePolyRing(int _vertNum, AxisType _type, float _minRad, float _maxRad)
        {
            return CreatePolyRing(_vertNum, _type, _minRad, _maxRad, new Color(0.5f, 0.5f, 0.5f, 1.0f));
        }
        public static Mesh CreatePolyRing(int _vertNum, AxisType _type, float _minRad, float _maxRad, Color _color)
        {
            Vector3[] vertices = new Vector3[(_vertNum + 1) * 2];
            int[] triangles = new int[(_vertNum + 1) * 6];
            Vector2[] uv = new Vector2[(_vertNum + 1) * 2];
            Color[] colors = new Color[(_vertNum + 1) * 2];
            Vector3[] normals = new Vector3[(_vertNum + 1) * 2];
            Vector4[] tangents = new Vector4[(_vertNum + 1) * 2];

            int cnt = 0;
            for (int ii = 0; ii <= _vertNum; ++ii)
            {
                float fx = Mathf.Cos(Mathf.PI * 2.0f * ((float)ii / (float)_vertNum));
                float fy = Mathf.Sin(Mathf.PI * 2.0f * ((float)ii / (float)_vertNum));

                if (_type == AxisType.XY)
                {
                    vertices[cnt * 2 + 0] = new Vector3(fx * _maxRad * 0.5f, fy * _maxRad * 0.5f, 0.0f);
                    vertices[cnt * 2 + 1] = new Vector3(fx * _minRad * 0.5f, fy * _minRad * 0.5f, 0.0f);
                }
                else
                { // XZ
                    vertices[cnt * 2 + 0] = new Vector3(fx * _maxRad * 0.5f, 0.0f, fy * _maxRad * 0.5f);
                    vertices[cnt * 2 + 1] = new Vector3(fx * _minRad * 0.5f, 0.0f, fy * _minRad * 0.5f);
                }

                triangles[cnt * 6 + 0] = cnt * 2 + 0;
                triangles[cnt * 6 + 1] = cnt * 2 + 1;
                triangles[cnt * 6 + 2] = (ii < (_vertNum)) ? (cnt * 2 + 2) : 0;
                triangles[cnt * 6 + 3] = cnt * 2 + 0;
                triangles[cnt * 6 + 4] = (ii > 0) ? (cnt * 2 - 1) : 0;
                triangles[cnt * 6 + 5] = cnt * 2 + 1;
                uv[cnt * 2 + 0] = new Vector2((float)ii / (float)_vertNum, 0.0f);
                uv[cnt * 2 + 1] = new Vector2((float)ii / (float)_vertNum, 1.0f);
                colors[cnt * 2 + 0] = colors[cnt * 2 + 1] = _color;
                normals[cnt * 2 + 0] = normals[cnt * 2 + 1] = new Vector3(0.0f, 0.0f, -1.0f);
                tangents[cnt * 2 + 0] = tangents[cnt * 2 + 1] = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
                cnt++;
            }
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            //		mesh.triangles = triangles;
            mesh.SetIndices(triangles, MeshTopology.Triangles, 0);
            mesh.uv = uv;
            mesh.colors = colors;
            mesh.normals = normals;
            mesh.tangents = tangents;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            ;
            return mesh;
        }

        public static Mesh CreateTriStrip(Vector3[] _verts)
        {
            return CreateTriStrip(_verts, new Color(0.5f, 0.5f, 0.5f, 1.0f));
        }
        public static Mesh CreateTriStrip(Vector3[] _verts, Color _color)
        {
            if (_verts.Length < 3)
            {
                return null;
            }
            int _vertNum = _verts.Length;
            Vector3[] verts = _verts;
            Vector2[] uvs = new Vector2[_vertNum];
            Vector3[] norms = new Vector3[_vertNum];
            Vector4[] tgts = new Vector4[_vertNum];
            Color[] cols = new Color[_vertNum];

            for (int ii = 0; ii < _vertNum; ++ii)
            {
                float tu = (float)(ii / 2) / (float)((_vertNum / 2) - 1);
                float tv = (float)(ii & 1);
                uvs[ii] = new Vector2(tu, tv);
                cols[ii] = _color;
                norms[ii] = new Vector3(0.0f, 0.0f, -1.0f);
                tgts[ii] = new Vector4(1f, 0f, 0f, 0f);
            }

            int[] tris = new int[(_vertNum - 2) * 3];
            for (int ii = 0; ii < _vertNum - 2; ++ii)
            {
                if ((ii & 1) == 0)
                {
                    tris[ii * 3 + 0] = ii + 0;
                    tris[ii * 3 + 1] = ii + 1;
                    tris[ii * 3 + 2] = ii + 2;
                }
                else
                {
                    tris[ii * 3 + 0] = ii + 2;
                    tris[ii * 3 + 1] = ii + 1;
                    tris[ii * 3 + 2] = ii + 0;
                }
            }

            Mesh mesh = new Mesh();
            mesh.vertices = verts;
            mesh.triangles = tris;
            mesh.uv = uvs;
            mesh.colors = cols;
            mesh.normals = norms;
            mesh.tangents = tgts;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            ;
            return mesh;
        }

        public static Mesh CreateSphere(int _divH, int _divV)
        {
            return CreateSphere(_divH, _divV, new Color(0.5f, 0.5f, 0.5f, 1.0f));
        }
        public static Mesh CreateSphere(int _divH, int _divV, Color _color)
        {
            Mesh mesh = new Mesh();

            float radius = 0.5f;
            // Longitude |||
            int nbLong = _divH;
            // Latitude ---
            int nbLat = _divV;

            #region Vertices
            Vector3[] vertices = new Vector3[(nbLong + 1) * nbLat + 2];
            int _vertNum = vertices.Length;
            Color[] cols = new Color[_vertNum];
            float _piLat = Mathf.PI;
            float _piLong = Mathf.PI;

            vertices[0] = Vector3.up * radius;
            for (int lat = 0; lat < nbLat; lat++)
            {
                float a1 = _piLat * (float)(lat + 1) / (nbLat + 1);
                float sin1 = Mathf.Sin(a1);
                float cos1 = Mathf.Cos(a1);

                for (int lon = 0; lon <= nbLong; lon++)
                {
                    float a2 = _piLong * 2f * (float)(lon == nbLong ? 0 : lon) / nbLong;
                    //                    float a2 = _piLong * 2f * (float)lon / nbLong;
                    float sin2 = Mathf.Sin(a2 + _piLong);
                    float cos2 = Mathf.Cos(a2 + _piLong);

                    vertices[lon + lat * (nbLong + 1) + 1] = new Vector3(sin1 * cos2, cos1, sin1 * sin2) * radius;
                }
            }
            vertices[vertices.Length - 1] = Vector3.up * -radius;
            #endregion

            #region Normales		
            Vector3[] normales = new Vector3[vertices.Length];
            for (int n = 0; n < vertices.Length; n++)
            {
                normales[n] = vertices[n].normalized;
                cols[n] = _color;
            }
            #endregion

            #region UVs
            Vector2[] uvs = new Vector2[vertices.Length];
            uvs[0] = Vector2.up;
            uvs[uvs.Length - 1] = Vector2.zero;
            for (int lat = 0; lat < nbLat; lat++)
                for (int lon = 0; lon <= nbLong; lon++)
                    uvs[lon + lat * (nbLong + 1) + 1] = new Vector2((float)lon / nbLong, 1f - (float)(lat + 1) / (nbLat + 1));
            #endregion

            #region Triangles
            int nbFaces = vertices.Length;
            int nbTriangles = nbFaces * 2;
            int nbIndexes = nbTriangles * 3;
            int[] triangles = new int[nbIndexes];

            //Top Cap
            int i = 0;
            for (int lon = 0; lon < nbLong; lon++)
            {
                triangles[i++] = lon + 2;
                triangles[i++] = lon + 1;
                triangles[i++] = 0;
            }

            //Middle
            for (int lat = 0; lat < nbLat - 1; lat++)
            {
                for (int lon = 0; lon < nbLong; lon++)
                {
                    int current = lon + lat * (nbLong + 1) + 1;
                    int next = current + nbLong + 1;

                    triangles[i++] = current;
                    triangles[i++] = current + 1;
                    triangles[i++] = next + 1;

                    triangles[i++] = current;
                    triangles[i++] = next + 1;
                    triangles[i++] = next;
                }
            }

            //Bottom Cap
            for (int lon = 0; lon < nbLong; lon++)
            {
                triangles[i++] = vertices.Length - 1;
                triangles[i++] = vertices.Length - (lon + 2) - 1;
                triangles[i++] = vertices.Length - (lon + 1) - 1;
            }
            #endregion

            mesh.vertices = vertices;
            mesh.normals = normales;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.tangents = CalcTangents(mesh); ;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            ;

            return mesh;
        }

        // http://answers.unity3d.com/questions/7789/calculating-tangents-vector4.html
        public static Vector4[] CalcTangents(Mesh _mesh)
        {
            long vertexCount = _mesh.vertexCount;
            long triangleCount = _mesh.triangles.Length;
            Vector3[] tan1 = new Vector3[vertexCount];
            Vector3[] tan2 = new Vector3[vertexCount];
            Vector4[] tangents = new Vector4[vertexCount];
            for (long a = 0; a < triangleCount; a += 3)
            {
                long i1 = _mesh.triangles[a + 0];
                long i2 = _mesh.triangles[a + 1];
                long i3 = _mesh.triangles[a + 2];

                Vector3 v1 = _mesh.vertices[i1];
                Vector3 v2 = _mesh.vertices[i2];
                Vector3 v3 = _mesh.vertices[i3];

                Vector2 w1 = _mesh.uv[i1];
                Vector2 w2 = _mesh.uv[i2];
                Vector2 w3 = _mesh.uv[i3];

                float x1 = v2.x - v1.x;
                float x2 = v3.x - v1.x;
                float y1 = v2.y - v1.y;
                float y2 = v3.y - v1.y;
                float z1 = v2.z - v1.z;
                float z2 = v3.z - v1.z;

                float s1 = w2.x - w1.x;
                float s2 = w3.x - w1.x;
                float t1 = w2.y - w1.y;
                float t2 = w3.y - w1.y;

                float r = 1.0f / (s1 * t2 - s2 * t1);

                Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
                Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

                tan1[i1] += sdir;
                tan1[i2] += sdir;
                tan1[i3] += sdir;

                tan2[i1] += tdir;
                tan2[i2] += tdir;
                tan2[i3] += tdir;
            }

            for (long a = 0; a < vertexCount; ++a)
            {
                Vector3 n = _mesh.normals[a];
                Vector3 t = tan1[a];

                Vector3.OrthoNormalize(ref n, ref t);
                tangents[a].x = t.x;
                tangents[a].y = t.y;
                tangents[a].z = t.z;

                tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
            }
            return tangents;
        }

        public static Mesh SetMeshColor(Mesh _nowMesh, Color _col)
        {
            if (_nowMesh != null)
            {
                Color[] cols = new Color[_nowMesh.vertexCount];
                for (int ii = 0; ii < _nowMesh.vertexCount; ++ii)
                {
                    cols[ii] = _col;
                }
                _nowMesh.colors = cols;
            }
            return _nowMesh;
        }

        public static Mesh MeshInvert(Mesh _mesh, bool _HFlip = true)
        {
            if (_mesh != null)
            {
                for (int s = 0; s < _mesh.subMeshCount; ++s)
                {
#if false
					int[] tris = _mesh.GetTriangles(s);
					for (int i = 0; i < tris.Length / 3; ++i)
					{
					int idx = tris[i * 3 + 1];
					tris[i * 3 + 1] = tris[i * 3 + 2];
					tris[i * 3 + 2] = idx;
					}
					_mesh.SetTriangles(tris, s);
#else
                    int[] idcs = _mesh.GetIndices(s);
                    for (int i = 0; i < idcs.Length / 3; ++i)
                    {
                        int idx = idcs[i * 3 + 1];
                        idcs[i * 3 + 1] = idcs[i * 3 + 2];
                        idcs[i * 3 + 2] = idx;
                    }
                    _mesh.SetIndices(idcs, _mesh.GetTopology(s), s);
#endif
                }
                // _mesh.RecalculateNormals();
                Vector3[] invNml = _mesh.normals.Clone() as Vector3[];
                for (int i = 0; i < invNml.Length; ++i)
                {
                    invNml[i] *= -1f;
                }
                _mesh.normals = invNml;

                if (_HFlip)
                {
                    _mesh.uv = VFlipUV(_mesh.uv);
                    _mesh.uv2 = VFlipUV(_mesh.uv2);
                    _mesh.uv3 = VFlipUV(_mesh.uv3);
                    _mesh.uv4 = VFlipUV(_mesh.uv4);
                }
            }
            return _mesh;
        }

        public static Vector2[] VFlipUV(Vector2[] _uvArr)
        {
            if (_uvArr != null)
            {
                for (int i = 0; i < _uvArr.Length; ++i)
                {
                    _uvArr[i].x = 1f - _uvArr[i].x;
                }
            }
            return _uvArr;
        }
    }
} // namespace TmLib

