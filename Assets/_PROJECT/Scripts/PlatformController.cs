using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{

    [SerializeField] Transform knight;

    public Transform level_parent;

    [SerializeField]
    GameObject platform_parent = default;

    [SerializeField]
    GameObject[] platform_prefabs = default;

    public GameObject starting_platform;

    List<GameObject> active_platforms = new List<GameObject>();

    //Spawning
    [SerializeField]
    GameObject spawner = default;
    bool spawnable = true;

    private void Awake()
    {
        //Adds first platform to active platforms list
        active_platforms.Add(platform_parent.transform.GetChild(0).gameObject);
        starting_platform = platform_parent.transform.GetChild(0).gameObject;

        //Spawns the rest of starting platforms
        for (int i = 0; i < 10; i++)
        {
            int index = Random.Range(1, platform_prefabs.Length);

            SpawnPlatform();
        }

    }


    //Recenter level z coord after it becomes too large
    public void LevelRecenter()
    {

        float distance = -level_parent.position.z;

        //Level 
        level_parent.position = new Vector3(level_parent.position.x, level_parent.position.y, 0);

        //Level children
        knight.Translate(Vector3.back * distance);
        foreach (Transform child in platform_parent.transform)
            child.Translate(Vector3.back * distance);
        spawner.transform.Translate(Vector3.back * distance);

    }

    void OnTriggerEnter(Collider other)
    {
        //Destroys platforms
        if (other.gameObject.CompareTag("Platform"))
        {
            Destroy(other.gameObject);
            active_platforms.Remove(other.gameObject);
        }

        //Spawns a platform and moves the spawner
        if (spawnable)
        {
            spawnable = false;

            SpawnPlatform();
        }
    }

    int index;
    int last_index;

    void SpawnPlatform()
    {
        index = Random.Range(1, platform_prefabs.Length);
        if (index == last_index)
            index = Random.Range(1, platform_prefabs.Length);

        active_platforms.Add(Instantiate(platform_prefabs[index], spawner.transform.position, Quaternion.identity, platform_parent.transform));
        last_index = index;

        spawner.transform.Translate(0, 0, 4);

        spawnable = true;
    }

}
