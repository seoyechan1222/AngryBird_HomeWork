using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    public GameObject boomPrefab;
    public GameObject boomPrefab2;
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("enemy")) // 적의 태그가 "enemy"인지 확인
        {
            Instantiate(boomPrefab, collision.transform.position, Quaternion.identity);
            Instantiate(boomPrefab2, collision.transform.position, Quaternion.identity);
            Destroy(collision.gameObject); // 적 개체 제거
        }
    }
}