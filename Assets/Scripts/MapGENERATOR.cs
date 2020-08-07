using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MapGENERATOR : MonoBehaviour
{
    public GameObject gridMap;
    public List<GameObject> tileMapPreFab;
    public float toMoveColliders;
    public float toMoveSpawnPoints;
    public BoxCollider2D topCollider;
    public BoxCollider2D bottomCollider;
    public List<Spawn> colliders;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnMap(Collider2D collider2D, GameObject player)
    {
        Spawn col = colliders.Find(x => x.collider == collider2D);
        GameObject GOToSpawn = tileMapPreFab[(int) Random.Range(0, tileMapPreFab.Count)];
        GameObject newGo = Instantiate(GOToSpawn, col.spawnPoint.transform.position, Quaternion.identity);
        newGo.transform.parent = gridMap.transform;
        
        Vector2 tempCol = new Vector2(col.collider.gameObject.transform.position.x, col.collider.gameObject.transform.position.y);
        Vector2 tempSpawn = new Vector2(col.spawnPoint.gameObject.transform.position.x, col.spawnPoint.gameObject.transform.position.y);

        switch (col.direction)
        {
            /*case Spawn.Directions.Top:
                col.collider.gameObject.transform.position = new Vector3(tempCol.x, tempCol.y - toMoveColliders);
                col.spawnPoint.gameObject.transform.position = new Vector3(tempSpawn.x, tempSpawn.y - toMoveSpawnPoints);
                break;
                
            case Spawn.Directions.Down:
                break;*/
            
            case Spawn.Directions.Left:
                col.collider.gameObject.transform.position = new Vector3(tempCol.x - toMoveColliders, tempCol.y);
                col.spawnPoint.gameObject.transform.position = new Vector3(tempSpawn.x - toMoveSpawnPoints, tempSpawn.y);
                topCollider.size = new Vector2(topCollider.size.x, topCollider.size.y + toMoveColliders);
                topCollider.offset = new Vector2(topCollider.offset.x, topCollider.offset.y + 2f);
                
                bottomCollider.size = new Vector2(bottomCollider.size.x, bottomCollider.size.y + toMoveColliders);
                bottomCollider.offset = new Vector2(bottomCollider.offset.x, bottomCollider.offset.y - 2f);
                break;
            case Spawn.Directions.Right:
                col.collider.gameObject.transform.position = new Vector3(tempCol.x + toMoveColliders, tempCol.y);
                col.spawnPoint.gameObject.transform.position = new Vector3(tempSpawn.x + toMoveSpawnPoints, tempSpawn.y);
                topCollider.size = new Vector2(topCollider.size.x , topCollider.size.y - toMoveColliders);
                topCollider.offset = new Vector2(topCollider.offset.x , topCollider.offset.y + 2f);
                
                bottomCollider.size = new Vector2(bottomCollider.size.x , bottomCollider.size.y + toMoveColliders);
                bottomCollider.offset = new Vector2(bottomCollider.offset.x , bottomCollider.offset.y + 2f);
                break;
        }
    }
}

[System.Serializable]
public class Spawn
{
    public Collider2D collider;
    public GameObject spawnPoint;
    public enum Directions {Left, Right, Top, Down };
    public Directions direction;
}
