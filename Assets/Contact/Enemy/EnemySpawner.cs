using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject jetEnemy;
    [SerializeField] private float innerRadius;
    [SerializeField] private float outerRadius;
    private float ratio;

    void Start()
    {
        ratio = innerRadius / outerRadius;
        // Spawn one enemy for testing
        SpawnEnemy();
    }

    public Vector3 RandomDonutPosition()
    {
        float radius = Mathf.Sqrt(Random.Range(ratio * ratio, 1f)) * outerRadius;
        Vector3 tempPoint = Random.insideUnitCircle.normalized * radius;
        Vector3 donutPos = new Vector3(tempPoint.x, 0f, tempPoint.y);
        return donutPos;
    }

    void SpawnEnemy()
    {
        Vector3 spawnPoint = RandomDonutPosition();

        Instantiate(jetEnemy, spawnPoint, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(Vector3.zero, innerRadius);
        Gizmos.DrawWireSphere(Vector3.zero, outerRadius);
    }
}
