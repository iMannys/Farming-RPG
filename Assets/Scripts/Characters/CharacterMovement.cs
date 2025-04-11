using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CharacterMovement : MonoBehaviour
{

    protected NavMeshAgent agent;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();

    }
    
    public void ToggleMovement(bool enabled)
    {
        agent.enabled = enabled;
    }

    public bool IsMoving()
    {
        //If the agent is disabled it is automaticaly false
        if (!agent.enabled) return false;
        float v = agent.velocity.sqrMagnitude;
        return v > 0.25;
    }

    public void MoveTo(NPCLocationState locationState)
    {
        SceneTransitionManager.Location locationToMoveTo = locationState.location;

        SceneTransitionManager.Location currentLocation = SceneTransitionManager.Instance.currentLocation;
        //Check if location is the same
        if (locationToMoveTo == currentLocation)
        {
            //Check if the coord is the same
            NavMeshHit hit;
            //Sammple the nearest valid position on the NavMesh
            NavMesh.SamplePosition(locationState.coord, out hit, 10f, NavMesh.AllAreas);

            //If the npc is already where he should be just carry on
            if (Vector3.Distance(transform.position, hit.position) < 1) return;
            if (agent != null && agent.isOnNavMesh)
            {
                agent.SetDestination(hit.position);
            }
            else
            {
                Debug.Log("NavMeshAgent is not on a valid NavMesh. Destination not set.");
            }

            return;
        }

        SceneTransitionManager.Location nextLocation = LocationManager.GetNextLocation(currentLocation, locationToMoveTo);


        //Find the exit point
        Vector3 destination = LocationManager.Instance.GetExitPosition(nextLocation).position;
        agent.SetDestination(destination);

    }

    public void MoveTo(Vector3 pos)
    {
        agent.SetDestination(pos);
    }


}
