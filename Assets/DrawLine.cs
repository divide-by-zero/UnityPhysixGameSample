using UnityEngine;
using System.Collections.Generic;

public class DrawLine : MonoBehaviour
{
    [SerializeField] private float width;
    private GameObject targetObject;
    private Mesh mesh;
    private List<Vector2> vlist = new List<Vector2>();

    private void CreateMesh(Mesh mesh,List<Vector2> vlist)
    {
        mesh.Clear();

        var vCnt = vlist.Count;
        var vertices = new List<Vector3>();
        for (int i = 0; i < vCnt-1; i++)
        {
            var currentPos = vlist[i];
            var nextPos = vlist[i + 1];
            var vec = currentPos - nextPos;//今と、一つ先のベクトルから、進行方向を得る
            if(vec.magnitude < 0.01f)continue;  //あまり頂点の間が空いてないのであればスキップする
            var v =  new Vector2(-vec.y,vec.x).normalized * width; //90度回転させてから正規化*widthで左右への幅ベクトルを得る

            //指定した横幅に広げる
            vertices.Add(currentPos-v);
            vertices.Add(currentPos+v);
        }

        var indices = new List<int>();
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
            vlist.Clear();
        }
        if (Input.GetMouseButton(0) && targetObject != null)
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //既にあるオブジェクトに当たりそうならそこで生成を辞める
            if (Physics2D.CircleCast(pos, 0.2f, Vector2.zero))
            {
                Finish();
                return;
            }

            vlist.Add(pos);
            CreateMesh(mesh,vlist);
        }
        if (Input.GetMouseButtonUp(0) && targetObject != null)
        {
            Finish();
        }
    }

    private void Finish()
    {
        if (mesh.vertexCount < 4)
        {
            Destroy(targetObject);
            return;
        }
        var rigibody = targetObject.AddComponent<Rigidbody2D>();
        rigibody.useAutoMass = true;
        var polyColliderPos = CreateMeshToPolyCollider(mesh);
        var polyCollider = targetObject.AddComponent<PolygonCollider2D>();
        polyCollider.SetPath(0,polyColliderPos.ToArray());
        targetObject = null;
    }

    private List<Vector2> CreateMeshToPolyCollider(Mesh mesh)
    {
        var polyColliderPos = new List<Vector2>();
        //偶数を小さい順に
        for (int index = 0; index < mesh.vertices.Length; index+=2)
        {
            var pos = mesh.vertices[index];
            polyColliderPos.Add(pos);
        }
        //奇数を大きい順に
        for (int index = mesh.vertices.Length-1; index > 0; index-=2)
        {
            var pos = mesh.vertices[index];
            polyColliderPos.Add(pos);
        }
        return polyColliderPos;
    }
}
