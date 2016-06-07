using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameUtil;

public class DrawLine : MonoBehaviour
{
    [SerializeField] private float width;
    private GameObject targetObject;
    private Mesh mesh;
    private List<Vector2> l = new List<Vector2>();

    private void CreateMesh(Mesh mesh,IEnumerable<Vector2> vlist)
    {
        mesh.Clear();
        var vCnt = vlist.Count();
        var vertices = new List<Vector3>();
        var indices = new List<int>();
        for (int i = 0; i < vCnt-1; i++)
        {
            var currentPos = vlist.ElementAt(i);
            var nextPos = vlist.ElementAt(i + 1);
            //今と、一つ先のベクトルから、向いている方向を得る
            var vec = currentPos - nextPos;
            if(vec.magnitude < 0.01f)continue;  //あまり頂点の間が空いてないのであればスキップする
            var v = vec.normalized * width;

            //指定した横幅に広げる
            vertices.Add(new Vector3(currentPos.x + v.y,currentPos.y - v.x));
            vertices.Add(new Vector3(currentPos.x - v.y,currentPos.y + v.x));
        }

        for (int index = 0; index < vertices.Count-2; index+=2)
        {
            indices.Add(index);
            indices.Add(index+2);
            indices.Add(index+3);
            indices.Add(index+1);
        }

        mesh.SetVertices(vertices);
        mesh.SetIndices(indices.ToArray(),MeshTopology.Quads,0);
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            targetObject = new GameObject("MeshObject");
            var meshRenderer = targetObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = Resources.GetBuiltinResource<Material>("Sprites-Default.mat");
            var meshFilter = targetObject.AddComponent<MeshFilter>();
            mesh = new Mesh();
            meshFilter.mesh = mesh;
            l.Clear();
        }
        if (Input.GetMouseButton(0))
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //既にあるオブジェクトに当たりそうならそこで生成を辞める
            if (Physics2D.CircleCast(pos, 0.2f, Vector2.zero))
            {
                Debug.Log("Hit");
            }

            l.Add(pos);
            CreateMesh(mesh,l);
        }
        if (Input.GetMouseButtonUp(0) && targetObject != null)
        {
            if (mesh.vertexCount < 4)
            {
                Destroy(targetObject);
            }
            var rigibody = targetObject.AddComponent<Rigidbody2D>();
            rigibody.useAutoMass = true;
            var polyColliderPos = CreateMeshToPolyCollider(mesh);
            var polyCollider = targetObject.AddComponent<PolygonCollider2D>();
            polyCollider.SetPath(0,polyColliderPos.ToArray());
        }
    }

    private List<Vector2> CreateMeshToPolyCollider(Mesh mesh)
    {
        var polyColliderPos = new List<Vector2>();
        for (int index = 0; index < mesh.vertices.Length; index+=2)
        {
            var pos = mesh.vertices[index];
            polyColliderPos.Add(pos);
        }
        for (int index = mesh.vertices.Length-1; index > 0; index-=2)
        {
            var pos = mesh.vertices[index];
            polyColliderPos.Add(pos);
        }
        return polyColliderPos;
    }
}
