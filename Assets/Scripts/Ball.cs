using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    public float startingSpeed = 3f; // public attributes which can be set in the editor
    public float maxSpeed = 5f;
    private float speed;

    private bool isOutOfBounds = false;
    private PlayerSoundEffect soundEffects;

    private Vector3 direction;

    private Rigidbody rb;
    // Start is called before the first frame update
    void Start() {
        soundEffects = GetComponent<PlayerSoundEffect>();
        rb = GetComponent<Rigidbody>();
        Reset();
    }

    void FixedUpdate() {
        Vector3 movement = direction.normalized * 100 * (speed * Time.fixedDeltaTime);
        rb.velocity = movement;
    }

    void Update() {
       if (isOutOfBounds) {
            isOutOfBounds = false;
            Reset();
       }
    }

    void Reset() {
        transform.position = Vector3.zero;
        int randomAngle = UnityEngine.Random.Range(-20, 20);
        direction = Quaternion.Euler(0, randomAngle, 0) * Vector3.left;
        speed = startingSpeed;
    }

    Vector3 ComputeReflection(Collision other) {
        Vector3 normal = other.GetContact(0).normal.normalized;
        Vector3 collisionPoint = other.GetContact(0).point;
        
        float hitFactor = (collisionPoint.z - other.transform.position.z) / other.collider.bounds.size.z;
        hitFactor = Mathf.Max(-0.4f, Mathf.Min(0.4f, hitFactor)); // disallow very steep angles
        hitFactor = normal.x > 0 ? hitFactor * (-1): hitFactor;

        Vector3 reflection = Quaternion.Euler(0, hitFactor * 180, 0) * new Vector3(normal.x, 0, 0);
        return reflection;
    }

    void OnCollisionEnter(Collision other) {
        Vector3 reflection = Vector3.Reflect(direction, other.GetContact(0).normal);

        if (other.collider.CompareTag("Player")) {
            soundEffects.PlayPaddleClip();
            reflection = ComputeReflection(other);
            speed = Mathf.Min(maxSpeed, speed + 0.1f);
        }
        else if (other.collider.CompareTag("Enemy")) {
            soundEffects.PlayPaddleClip();
            reflection = ComputeReflection(other);
        }
        else if (other.collider.CompareTag("Wall")) {
            soundEffects.PlayWallClip();
        }
        else if (other.collider.CompareTag("ScoreLine")) {
            soundEffects.PlayScoreClip();
            isOutOfBounds = true;
        }

        this.direction = reflection;

    }

    public Vector3 GetDirection() {
        return this.direction;
    }
}


// talk about collision, trigger, isKinematic 
