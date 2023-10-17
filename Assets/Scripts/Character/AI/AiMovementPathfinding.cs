using UnityEngine;
using System.Collections;
using Pathfinding;

public class AiMovementPathfinding : MonoBehaviour {
    private Seeker seeker;
    private CharacterController _myCharacterController;

    private Path currentPath;
    private int currentWaypoint = 0;

    private float repathRate = 0.1f;
    private float lastRepath = float.NegativeInfinity;

    [SerializeField]
    private float nextWaypointDistance = .5f;

    private Transform myChaseTarget;
    public bool isChasing = false;

    // TODO this should be related to mySize + targetSize
    private const float CHASE_MIN_DISTANCE = 1.5f;

    private void Awake() {
        seeker = GetComponent<Seeker>();
        _myCharacterController = GetComponent<CharacterController>();
    }

    public void SetChaseTarget(Transform target) {
        myChaseTarget = target;
        isChasing = true;
        MoveToTarget(target.position);
    }

    public void MoveToTarget(Vector3 targetLocation) {
        currentPath = null;
        currentWaypoint = 0;
        seeker.StartPath(transform.position, targetLocation, OnPathfindingComplete);
    }

    public void Stop() {
        currentPath = null;
        myChaseTarget = null;
        isChasing = false;
        _myCharacterController.InputDirection = Vector2.zero;
    }

    private void OnPathfindingComplete(Path path) {
        if (path.error) {
            return;
        }

        currentPath = path;
    }

    private void Update() {
        FollowPath();
    }

    private void FollowPath() {
        if (myChaseTarget) {
            if (Time.time > lastRepath + repathRate && seeker.IsDone()) {
                lastRepath = Time.time;

                MoveToTarget(myChaseTarget.position);
            }
        }

        if (currentPath == null) {
            return;
        }

        if (currentWaypoint > currentPath.vectorPath.Count) {
            return;
        }

        if (currentWaypoint == currentPath.vectorPath.Count) {
            currentWaypoint++;
            _myCharacterController.InputDirection = Vector2.zero;
            //reset path here?
            return;
        }

        Vector3 currentPathPoint = currentPath.vectorPath[currentWaypoint];

        Vector3 direction = (currentPathPoint - transform.position).normalized;
        _myCharacterController.InputDirection = direction;

        if (myChaseTarget) {
            float distanceToTarget = Vector3.Distance(transform.position, myChaseTarget.transform.position);

            if (distanceToTarget < CHASE_MIN_DISTANCE) {
                isChasing = false;
                _myCharacterController.InputDirection = Vector2.zero;
            } else if (!isChasing && distanceToTarget >= CHASE_MIN_DISTANCE) {
                isChasing = true;
            }
        }

        if ((transform.position - currentPathPoint).sqrMagnitude < nextWaypointDistance * nextWaypointDistance) {
            currentWaypoint++;
            return;
        }
    }
}