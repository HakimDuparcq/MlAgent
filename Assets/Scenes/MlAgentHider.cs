using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Serialization;

public class MlAgentHider : Agent
{
    
    [SerializeField] private GameObject seeker;
    [SerializeField] private float speed;

    private int episodeFrameDuration;
    
    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-4,4), 0,Random.Range(-4,4));
        seeker.transform.localPosition = new Vector3(Random.Range(-4,4), 0,Random.Range(-4,4));
        episodeFrameDuration = 0;
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(seeker.transform.localPosition);
        episodeFrameDuration++;
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        transform.position += new Vector3(moveX, 0, moveZ) * (Time.deltaTime * speed);
        
        AddReward(0.2f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }
    
    private void OnTriggerEnter(Collider _col)
    {
        if (_col.GetComponent<Wall>())
        {
            AddReward(-2);
            EndEpisode();
        }
        else if (_col.GetComponent<MlAgentSeeker>())
        {
            AddReward(-1);
            EndEpisode();
        }
    }
}
