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

    [Header("Spawnable Characters")]
    public GameObject enemies1;
    public GameObject enemies2;
    public GameObject enemies3;
    public GameObject merchant;

    
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
        GameObject goToSpawn = tileMapPreFab[(int) Random.Range(0, tileMapPreFab.Count)];
        GameObject newGo = Instantiate(goToSpawn, col.spawnPoint.transform.position, Quaternion.identity);
        newGo.transform.parent = gridMap.transform;
        Transform[] spawnPoints = newGo.GetComponentsInChildren<Transform>();
        EvaluateSpawning(spawnPoints);
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

    public void EvaluateSpawning(Transform[] spawnPoints)
    {
        Player currentParty = FindObjectOfType<Player>();
        bool spawn1 = false;
        bool spawn2 = false;
        //bool spawn3 = false;

        switch (currentParty.playerCurrentLevel)
        {
            case 0:
                switch (currentParty.party.DoggosInUse())
                {
                    case 0:
                        spawn1 = true;
                        break;
                    case 1:
                        spawn1 = true;
                        spawn2 = true;
                        break;
                    case 2:
                        spawn1 = true;
                        spawn2 = true;
                        //spawn3 = true;
                        break;
                }
                break;
            case 1:
                switch (currentParty.party.DoggosInUse())
                {
                    case 0:
                        spawn2 = true;
                        break;
                    case 1:
                        spawn2 = true;
                        break;
                    case 2:
                        spawn2 = true;
                        //spawn3 = true;
                        break;
                }
                break;
            case 2:
                switch (currentParty.party.DoggosInUse())
                {
                    case 0:
                        spawn2 = true;
                        break;
                    case 1:
                        spawn2 = true;
                        //spawn3 = true;
                        break;
                    case 2:
                        //spawn3 = true;
                        break;
                }
                break;
            default:
                switch (currentParty.party.DoggosInUse())
                {
                    case 0:
                        spawn2 = true;
                        break;
                    case 1:
                        spawn2 = true;
                        //spawn3 = true;
                        break;
                    case 2:
                        spawn2 = true;
                        //spawn3 = true;
                        break;
                }
                break;
        }
        
        List<int> choices = new List<int>();

        for (int i = 0; i < spawnPoints.Length - 1; i++)
        {
            choices.Add(Random.Range(1, 3));
        }

        for (int i = 0; i < spawnPoints.Length - 1; i++)
        {
            if (spawnPoints[i].CompareTag("EnemySpawnPoint"))
            {
                switch (choices[i])
                {
                    case 1:
                        if (spawn1 == true)
                        {
                            GameObject newGo = Instantiate(enemies1, spawnPoints[i].transform.position, Quaternion.identity);
                            newGo.transform.parent = spawnPoints[i].transform.parent;
                        }
                        break;
                    case 2:
                        if (spawn2 == true)
                        {
                            GameObject newGo = Instantiate(enemies2, spawnPoints[i].transform.position, Quaternion.identity);
                            newGo.transform.parent = spawnPoints[i].transform.parent;
                        }
                        break;
                    /*case 3:
                        if (spawn3 == true)
                        {
                            GameObject newGo = Instantiate(enemies3, spawnPoints[i].transform.position, Quaternion.identity);
                            newGo.transform.parent = spawnPoints[i].transform.parent;
                        }
                        break;*/
                }
            }
        }

        foreach (var variable in spawnPoints)
        {
            if (variable.CompareTag("MerchantSpawnPoint"))
            {
                merchant.transform.position = variable.position;
            }
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
