using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;

public class Boat : MonoBehaviour
{

    public bool moving = false;

    public MinimapBoat minimapBoat;

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
        SetSpeed(startSpeed);

    }

    public virtual void Update()
    {

        if (moving)
        {
            if (agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance <= agent.stoppingDistance)
            {
                EndMovenent();
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

        moving = true;
        targetPos = p;

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
        foreach (TrailRenderer renderer in GetComponentsInChildren<TrailRenderer>())
        {
            renderer.emitting = false;
            renderer.Clear();
        }

        Invoke("UpdatePositionOnScreenDelay", 0.1f);
    }

    public MinimapBoat GetMinimapBoat
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

    void UpdatePositionOnScreenDelay()
    {
        foreach (TrailRenderer renderer in GetComponentsInChildren<TrailRenderer>())
        {
            renderer.emitting = true;
        }
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
