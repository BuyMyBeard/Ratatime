using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] private float FOV;
    [SerializeField] private float IdealJumpOffset;
    [SerializeField] private GameObject landPointVisualizer;
    [SerializeField] private GameObject launchPointVisualizer;
    [SerializeField] private bool IsDebug;


    Vector2 launchPoint, landPoint;

    private TargetingComponent targetingComponent;

    private void Awake()
    {
        targetingComponent = GetComponent<TargetingComponent>();
        targetingComponent.ReachedTarget += TargetingComponent_ReachedTarget;
        FindPath();
    }

    private void TargetingComponent_ReachedTarget(object sender, EventArgs e)
    {
        if (targetingComponent.target == launchPoint)
        {
            targetingComponent.SetTarget(landPoint);
            targetingComponent.mode = TargetingComponent.MoveModes.Jump;
        }
        else if (targetingComponent.target == landPoint)
            FindPath();
        else
            targetingComponent.mode = TargetingComponent.MoveModes.Jump;
    }

    public void FindPath()
    {
        var endpoint = FindEndpoint();

        var platform = FindPlatforms(endpoint);

        if (platform == null)
            return;

        SetLandPosition(platform);
        SetLaunchPosition();

        targetingComponent.SetTarget(launchPoint);
    }

    Vector2 FindEndpoint()
    {
        return GameObject.FindGameObjectWithTag("Player").transform.position;
    }

    Collider2D FindPlatforms(Vector2 endpoint)
    {
        LayerMask mask = LayerMask.GetMask("Platform");
        if (IsDebug)
        {
            Debug.DrawRay(transform.position, (endpoint - new Vector2(transform.position.x, transform.position.y)), Color.green, 10);
        }

        var hit = Physics2D.CircleCast(transform.position, FOV, endpoint - new Vector2(transform.position.x, transform.position.y), Mathf.Infinity, mask);
        return hit.collider == null ? null : hit.collider;
    }

    void SetLandPosition(Collider2D platform)
    {
        var platformWidth = platform.bounds.size.x;
        var platformHeight = platform.bounds.size.y;
        var center = platform.bounds.center;

        var surface = center.y - platformHeight / 2;

        var leftEdge = new Vector2(center.x - platformWidth / 2, surface);
        var rightEdge = new Vector2(center.x + platformWidth / 2, surface);

        if(Vector2.Distance(transform.position, leftEdge) >= Vector2.Distance(transform.position, rightEdge))
            landPoint = rightEdge;
        else
            landPoint = leftEdge;

        if (IsDebug)
        {
            var instance = Instantiate(landPointVisualizer, new Vector3(landPoint.x, landPoint.y, 0), Quaternion.identity);
            Destroy(instance, 10);
        }

    }

    void SetLaunchPosition()
    {
        LayerMask mask = LayerMask.GetMask("Platform");
        var currentPlatform = Physics2D.CircleCast(transform.position, 1f, Vector2.down, Mathf.Infinity, mask).collider;

        if (currentPlatform == null)
        {
            mask = LayerMask.GetMask("Ground");
            currentPlatform = Physics2D.CircleCast(transform.position, 1f, Vector2.down, Mathf.Infinity, mask).collider;
        }

        var closestPoint = currentPlatform.bounds.ClosestPoint(landPoint);

        var launchAdjustment = 0f;

        if (closestPoint.x == landPoint.x)
        {
            launchAdjustment = transform.position.x <= closestPoint.x ? -IdealJumpOffset : IdealJumpOffset;
        }
        else
        {

        }

        closestPoint.x += launchAdjustment;

        launchPoint = closestPoint;

        if (IsDebug)
        {
            var instance = Instantiate(launchPointVisualizer, new Vector3(launchPoint.x, launchPoint.y, 0), Quaternion.identity);
            Destroy(instance, 10);
        }

    }
}
