using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] float _length = 6;

    void Update()
    {
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, transform.position + _rigidbody.velocity * _length);
    }
}
