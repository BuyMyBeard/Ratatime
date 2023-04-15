using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideCollisionComponent : MonoBehaviour
{
    Vector2 colliderSize;
    private void Awake()
    {

        colliderSize = GetComponentInChildren<Collider2D>().bounds.size;

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            Vector2 closestPoint = collision.ClosestPoint(transform.position);
            Vector2 depthShift = (Vector2)transform.position - closestPoint;
        
            float widthShift = depthShift.x < 0 ? -colliderSize.x / 2 : colliderSize.x / 2;
            transform.Translate(depthShift.x + widthShift , 0, 0);
            Debug.Log(depthShift.x);

            Debug.Log($"({depthShift.x}, {depthShift.y}");
        }

    }
}
