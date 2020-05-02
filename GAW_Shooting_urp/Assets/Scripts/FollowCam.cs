using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float smoothing = 5f;

    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - target.position;
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 targetCanPos = target.position + offset;

        transform.position = Vector3.Lerp(transform.position, targetCanPos, smoothing * Time.deltaTime);
    }
}
