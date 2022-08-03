using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour {

    public Transform _transform;
    Animator _animator;

    public bool move = false;
    public float speed = 1f;

    private void Start()
    {
        _transform = GetComponent<Transform>();

        _animator = GetComponent<Animator>();

        Invoke("StartDelay", Random.Range(0, WaveManager.Instance.maxStartDelay));

    }

    public void UpdateMovement()
    {
        _transform.Translate(Vector3.right * speed * Time.deltaTime, Space.World);
    }

    public void StartDelay()
    {
        Invoke("ResetPos", WaveManager.Instance.rate);
    }

    public void ResetPos()
    {
        gameObject.SetActive(true);
        _transform.position = WaveManager.Instance.GetRandomPos();
        move = true;
        speed = Random.Range(WaveManager.Instance.minSpeed, WaveManager.Instance.maxSpeed);

        Invoke("ResetPosDelay", WaveManager.Instance.duration);
    }

    void ResetPosDelay()
    {

        move = false;

        gameObject.SetActive(false);

        Invoke("ResetPos", WaveManager.Instance.rate);
    }
}
