using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour {

    //Parallax Scroll Variables
    public Camera cam;//the camera
    public Transform subject;//the subject (usually the player character)


    //Instance variables
    float zPosition;
    Vector2 startPos;


    //Properties
    float twoAspect => cam.aspect * 2;
    float tileWidth => (twoAspect > 3 ? twoAspect : 3);
    float viewWidth => loopSpriteRenderer.sprite.rect.width / loopSpriteRenderer.sprite.pixelsPerUnit;
    Vector2 travel => (Vector2)cam.transform.position - startPos; //2D distance travelled from our starting position
    float distanceFromSubject => transform.position.z - subject.position.z;
    float clippingPlane => (cam.transform.position.z + (distanceFromSubject > 0 ? cam.farClipPlane : cam.nearClipPlane));
    float parallaxFactor => Mathf.Abs(distanceFromSubject) / clippingPlane;


    //User options
    public bool xAxis = true; //parallax on x?
    public bool yAxis = true; //parallax on y?
    public bool infiniteLoop = false; //are we looping?


    //Loop requirement
    public SpriteRenderer loopSpriteRenderer;


    // Start is called before the first frame update
    void Awake() {
        cam = Camera.main;
        startPos = transform.position;
        zPosition = transform.position.z;

        if (loopSpriteRenderer != null && infiniteLoop) {
            float spriteSizeX = loopSpriteRenderer.sprite.rect.width / loopSpriteRenderer.sprite.pixelsPerUnit;
            float spriteSizeY = loopSpriteRenderer.sprite.rect.height / loopSpriteRenderer.sprite.pixelsPerUnit;

            loopSpriteRenderer.drawMode = SpriteDrawMode.Tiled;
            loopSpriteRenderer.size = new Vector2(spriteSizeX * tileWidth, spriteSizeY);
            transform.localScale = Vector3.one;
        }
    }


    // Update is called once per frame
    void Update() {
        Vector2 newPos = startPos + travel * parallaxFactor;
        transform.position = new Vector3(xAxis ? newPos.x : startPos.x, yAxis ? newPos.y : startPos.y, zPosition);

        if (infiniteLoop) {
            Vector2 totalTravel = cam.transform.position - transform.position;
            float boundsOffset = (viewWidth / 2) * (totalTravel.x > 0 ? 1 : -1);
            float screens = (int)((totalTravel.x + boundsOffset) / viewWidth);
            transform.position += new Vector3(screens * viewWidth, 0);
        }
    }

}
