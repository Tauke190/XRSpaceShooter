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

using UnityEngine;

namespace BlackWhale.DestructibleMeshSystem
{

    public class MRUKDestroyWalls : MonoBehaviour
    {
        private MRUKDestructibleMesh destructibleMesh;
        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;

        void Start()
        {
            destructibleMesh = FindObjectOfType<MRUKDestructibleMesh>();
            meshRenderer = GetComponent<MeshRenderer>();
            meshCollider = GetComponent<MeshCollider>();
            CreateAndFitPrefab();
        }

        private void CreateAndFitPrefab()
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();

            GameObject instantiatedPrefab = Instantiate(destructibleMesh.prefabToInstantiate, transform);
          

            Bounds bounds = meshFilter.sharedMesh.bounds;

            float prefabInitialSizeX = 10f;
            float prefabInitialSizeZ = 10f;

            float scaleX = bounds.size.x / prefabInitialSizeX;
            float scaleZ = bounds.size.y / prefabInitialSizeZ;
            float scaleY = 0.001f;

            instantiatedPrefab.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
            instantiatedPrefab.transform.localPosition = new Vector3(0, 0, 0);
            instantiatedPrefab.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

            if (meshRenderer != null) meshRenderer.enabled = false;
            if (meshCollider != null) meshCollider.enabled = false;
        }
    }
}