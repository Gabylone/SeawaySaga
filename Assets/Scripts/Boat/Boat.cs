using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;

public class Boat : MonoBehaviour
{

    public bool moving = false;

    private protected TrailRenderer[] trailRenderers;

    public MinimapBoat minimapBoat;
    SpriteRenderer[] spriteRenderers;

    [Space]
    [Header("Boat Elements")]
    [SerializeField]
    private Transform boatMesh;
    public Transform _transform;
    public Animator animator;

    [Space]
    [Header("Boat Position Parameters")]
    public float startSpeed = 5f;

    public Vector3 targetPos;
    private Vector2 targetDir;

    public NavMeshAgent agent;

    public virtual void Start()
    {
        trailRenderers = GetComponentsInChildren<TrailRenderer>();
        SetSpeed(startSpeed);
    }

    public virtual void Update()
    {
        if (moving)
        {
            if (agent.isActiveAndEnabled && agent.remainingDistance <= agent.stoppingDistance && WorldTouch.Instance.touching == false)
            {
                EndMovenent();
            }

            foreach (var item in trailRenderers)
            {
                item.widthMultiplier = Mathf.Lerp(item.widthMultiplier, 3f, Time.deltaTime);
            }
        }
        else
        {
            foreach (var item in trailRenderers)
            {
                item.widthMultiplier = Mathf.Lerp(item.widthMultiplier, 0f, Time.deltaTime);
            }
        }

    }

    void CheckForBounds()
    {
        Vector2 p = (Vector2)GetTransform.position;

    }

    public float rotationSpeed = 10f;

    private void SetBoatRotation()
    {

        Vector3 targetDir = (targetPos - GetTransform.position).normalized;

        float targetAngle = Vector3.Angle(targetDir, Vector3.forward);
        if (Vector3.Dot(Vector3.right, targetDir) < 0)
            targetAngle = -targetAngle;

        Quaternion targetRot = Quaternion.Euler(0, targetAngle, 0);

        boatMesh.localRotation = Quaternion.RotateTowards(boatMesh.localRotation, targetRot, rotationSpeed * Time.deltaTime);

    }

    #region moving
    public virtual void SetTargetPos(Vector3 p)
    {
        if (agent.isOnNavMesh
            && agent.isActiveAndEnabled)
        {

        }
        else
        {
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(p);

        CancelInvoke("SetTargetPosDelay");
        Invoke("SetTargetPosDelay", 0.0001f);
        
        targetPos = p;

    }

    void SetTargetPosDelay()
    {
        moving = true;
    }

    public virtual void EndMovenent()
    {
        moving = false;

        if (agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }

    }
    #endregion

    #region map position 
    public virtual void UpdatePositionOnScreen()
    {
        foreach (TrailRenderer renderer in trailRenderers)
        {
            renderer.emitting = false;
            renderer.Clear();
        }
    }

    public virtual MinimapBoat GetMinimapBoat
    {
        get
        {
            return minimapBoat;
        }
    }

    public virtual BoatInfo GetBoatInfo()
    {
        return null;
    }

    
    public void SetSpeed(float newSpeed)
    {
        agent.speed = newSpeed;
    }
    #endregion

    public Transform GetTransform
    {
        get
        {
            if ( _transform == null)
            {
                _transform = transform;
            }

            return _transform;
        }
    }
}
