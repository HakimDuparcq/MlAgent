using System;
using System.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MlAgentSeeker : Agent
{
    [SerializeField] private GameObject hider;
    [SerializeField] private float speed;
    
    [SerializeField] private MeshRenderer ground;
    [SerializeField] private Material startMaterial;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;

    [SerializeField] private int updateFrameNumber;
    [SerializeField] private float timeDeltatime;
    
    [Space(10)]
    [SerializeField] private int fixedupdateFrameNumber;
    [SerializeField] private float timeFixedDeltatime;

    [Space(10)] 
    [SerializeField] private int onActionReceive;
    [SerializeField] private int onCollectObservation;
    
    public override void OnEpisodeBegin()
    {
        StartCoroutine(ChangeGroundMatAfter(0.2f));
        transform.localPosition = new Vector3(Random.Range(-4,4), 0,Random.Range(-4,4));
        hider.transform.localPosition = new Vector3(Random.Range(-4,4), 0,Random.Range(-4,4));

    }
    public override void CollectObservations(VectorSensor sensor)
    {
        onCollectObservation++;
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(hider.transform.localPosition);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        onActionReceive++;
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        transform.position += new Vector3(moveX, 0, moveZ) * (Time.deltaTime * speed);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void Update()
    {
        updateFrameNumber++;
        timeDeltatime = Time.deltaTime;
    }

    private void FixedUpdate()
    {
        fixedupdateFrameNumber++;
        timeFixedDeltatime = Time.fixedDeltaTime;
    }

    private void OnTriggerEnter(Collider _col)
    {
        if (_col.GetComponent<Wall>())
        {
            ground.material = loseMaterial;
            SetReward(-1);
            EndEpisode();
        }
        else if (_col.GetComponent<MlAgentHider>())
        {
            ground.material = winMaterial;
            SetReward(1);
            EndEpisode();
        }
        
    }

    private IEnumerator ChangeGroundMatAfter(float _timer)
    {
        yield return new WaitForSeconds(_timer);
        ground.material = startMaterial;
    }
}
