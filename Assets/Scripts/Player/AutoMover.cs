using System.Collections;
using UnityEngine;

public class AutoMover : MonoBehaviour
{
    [SerializeField] float speed = 2f;
    [SerializeField] float moveDuration = 3f;

    private bool isMoving = false;

    void Start()
    {
        StartCoroutine(StopAfterSeconds(moveDuration));
    }

    void Update()
    {
        if (isMoving)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    IEnumerator StopAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        isMoving = false;
    }
}
