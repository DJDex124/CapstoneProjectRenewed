using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    private GameObject enemySpawner;


    void Start()
    {
        enemySpawner = this.gameObject; 
        StartCoroutine(SpawnEnemyWithDelay(2f)); 
    }
    IEnumerator SpawnEnemyWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Instantiate(enemyPrefab, enemySpawner.transform.position, Quaternion.identity);
    }
   
    void Update()
    {
        
    }
}
