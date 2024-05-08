/*
 * Copyright (c) 2024 Black Whale Studio. All rights reserved.
 *
 * This software is the intellectual property of Black Whale Studio. Direct use, copying, or distribution of this code in its original or only slightly modified form is strictly prohibited. Significant modifications or derivations are required for any use.
 *
 * If this code is intended to be used in a commercial setting, you must contact Black Whale Studio for explicit permission.
 *
 * For the full licensing terms and conditions, visit:
 * https://blackwhale.dev/
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT ANY WARRANTIES OR CONDITIONS.
 *
 * For questions or to join our community, please visit our Discord: https://discord.gg/55gtTryfWw
 */

using MIConvexHull;
using System.Collections.Generic;
using UnityEngine;

namespace BlackWhale.DestructibleMeshSystem
{

    public class MRUKVoronoiCellMesh : MonoBehaviour
    {
        [SerializeField] private Material meshMaterial;
        [SerializeField] private bool includeShadow = true;
        [SerializeField] private Vector3 shadowOffset = new(0, -10f, 0f);
        [SerializeField] private Material shadowMaterial;

        public GameObject CreateMeshForCell(DefaultTriangulationCell<DefaultVertex> cell)
        {
            GameObject cellObject = new GameObject("VoronoiCell");
            cellObject.transform.SetParent(transform, false);

            MeshFilter meshFilter = cellObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = cellObject.AddComponent<MeshRenderer>();
            MeshCollider meshCollider = cellObject.AddComponent<MeshCollider>();
            Mesh mesh = new Mesh();

            List<Vector3> vertices = GetVerticesForCell(cell);

            int[] triangles = TriangulateCell(vertices);

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            meshFilter.mesh = mesh;
            meshRenderer.material = meshMaterial;
            meshCollider.convex = true;
            meshCollider.sharedMesh = mesh;

            if (includeShadow) CreateShadowForCell(cellObject, mesh);

            return cellObject;
        }

        private void CreateShadowForCell(GameObject parentObject, Mesh originalMesh)
        {
            GameObject shadowObject = new GameObject("Shadow");
            shadowObject.transform.SetParent(parentObject.transform, false);

            Mesh shadowMesh = new Mesh();
            shadowMesh.vertices = ApplyOffsetToVertices(originalMesh.vertices, shadowOffset);
            shadowMesh.triangles = originalMesh.triangles;
            shadowMesh.RecalculateNormals();

            MeshFilter meshFilter = shadowObject.AddComponent<MeshFilter>();
            meshFilter.mesh = shadowMesh;

            MeshRenderer meshRenderer = shadowObject.AddComponent<MeshRenderer>();
            meshRenderer.material = shadowMaterial;
        }

        private Vector3[] ApplyOffsetToVertices(Vector3[] originalVertices, Vector3 offset)
        {
            Vector3[] offsetVertices = new Vector3[originalVertices.Length];
            for (int i = 0; i < originalVertices.Length; i++)
            {
                offsetVertices[i] = originalVertices[i] + offset;
            }
            return offsetVertices;
        }

        private static List<Vector3> GetVerticesForCell(DefaultTriangulationCell<DefaultVertex> cell)
        {
            List<Vector3> vertices = new List<Vector3>();
            foreach (var vertex in cell.Vertices)
            {
                vertices.Add(new Vector3((float)vertex.Position[0], 0, (float)vertex.Position[1]));
            }
            return vertices;
        }

        private static int[] TriangulateCell(List<Vector3> vertices)
        {
            List<int> triangles = new List<int>();
            if (vertices.Count < 3) return triangles.ToArray();

            for (int i = 1; i < vertices.Count - 1; i++)
            {
                triangles.Add(0);
                triangles.Add(i);
                triangles.Add(i + 1);
            }
            return triangles.ToArray();
        }
    }
}