using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Cinemachine;
using UnityEngine;

public class AnimatedDoorComponent : MonoBehaviour, ITimeShifter
{
    SpriteRenderer sprite;
    bool future;
    [SerializeField] Transform cinematicFollow;
    [SerializeField] Vector2 movement;
    [SerializeField] float timeToReach;
    [SerializeField] Sprite[] sprites;

    bool ended = false;
    private void Awake()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        cinematicFollow.position = transform.position;
    }
    public void ShiftToFuture()
    {
        future = true;
        if (ended)
            sprite.sprite = sprites[3];
        else
            sprite.sprite = sprites[2];
    }

    public void ShiftToPast()
    {
        future = false;
        if (ended)
            sprite.sprite = sprites[1];
        else
            sprite.sprite = sprites[0];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (future)
            sprite.sprite = sprites[3];
        else
            sprite.sprite= sprites[1];
        EndGame();
    }

    private void EndGame()
    {
        ended = true;
        CinemachineVirtualCamera camera = FindObjectOfType<CinemachineVirtualCamera>();
        camera.Follow = cinematicFollow;
        StartCoroutine(MoveStraightOverTime(movement, timeToReach));
        FindObjectOfType<PlayerMoveComponent>().End();
        FindObjectOfType<Game_Manager>().End();
    }
    IEnumerator MoveStraightOverTime(Vector2 translation, float time)
    {
        for (float timeElapsed = 0; timeElapsed < time; timeElapsed += Time.deltaTime)
        {
            yield return null;
            cinematicFollow.Translate(Time.deltaTime / time * translation, Space.World);
        }
    }

}
