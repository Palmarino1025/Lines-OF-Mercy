using UnityEngine;
using System.Collections.Generic;

public class MinimapFollow : MonoBehaviour
{
    [Header("Player Follow")]
    public Transform player;
    public float height = 30f;

    [Header("NPC Marker Setup")]
    public Transform npcRoot;              
    public GameObject markerPrefab;        
    public float minimapRange = 50f;
    public float markerHeightOffset = -5f; 

    private Dictionary<Transform, GameObject> npcMarkers = new Dictionary<Transform, GameObject>();

    void Start()
    {
        CacheNPCs();
    }

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 newPos = player.position;
        newPos.y = height;
        transform.position = newPos;

        UpdateMarkers();
    }

    void CacheNPCs()
    {
        if (npcRoot == null || markerPrefab == null) return;

        npcMarkers.Clear();

        foreach (Transform group in npcRoot)
        {
            //group folders (Mob, Civilian, Police, Pedestrians)
            foreach (Transform npcParent in group)
            {
                if (npcParent == null) continue;

                //skip broken/missing entries
                if (npcParent.name.Contains("Missing Prefab")) continue;

                GameObject marker = Instantiate(markerPrefab);
                marker.name = npcParent.name + "_MinimapMarker";
                npcMarkers.Add(npcParent, marker);
            }
        }
    }

    void UpdateMarkers()
    {
        foreach (var pair in npcMarkers)
        {
            Transform npc = pair.Key;
            GameObject marker = pair.Value;

            if (npc == null || marker == null) continue;

            Vector3 offset = npc.position - player.position;
            float distance = new Vector2(offset.x, offset.z).magnitude;

            if (distance > minimapRange)
            {
                marker.SetActive(false);
                continue;
            }

            marker.SetActive(true);

            Vector3 markerPos = new Vector3(
                npc.position.x,
                height + markerHeightOffset,
                npc.position.z
            );

            marker.transform.position = markerPos;
            marker.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }
}