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

    public class MRUKDestructibleMesh : MonoBehaviour
    {
        public GameObject prefabToInstantiate;

        public void InitializeMRUKVoronoiGeneration()
        {
            GameObject effectMesh = GameObject.Find("GLOBAL_MESH_EffectMesh");
            if (effectMesh != null) effectMesh.SetActive(false);

            // Find all Transform components in the scene, as every GameObject has a Transform component
            Transform[] allTransforms = FindObjectsOfType<Transform>();

            foreach (Transform tr in allTransforms)
            {
                // Check if the GameObject's name contains "CEILING_EffectMesh" or "WALL_FACE_EffectMesh"
                if (tr.name.ToLower().Contains("wall_face_effectmesh"))
                {
                    // Add the MRUKDestroyWalls component to the GameObject
                    tr.gameObject.AddComponent<MRUKDestroyWalls>();
                }
                if (tr.name.ToLower().Contains("ceiling_effectmesh"))
                {
                    tr.gameObject.SetActive(false);
                }
                if (tr.name.ToLower().Contains("floor_effectmesh"))
                {
                    tr.gameObject.tag = "Ground";
                    tr.gameObject.GetComponent<Collider>().enabled = true;
                }
                if (tr.name.ToLower().Contains("voronoicell"))
                {
                    GameManager.instance.totoal_voronoi_cells++;
                }
            }
        }

       
    }
}