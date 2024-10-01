using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
public class MummyGoAgent : Agent
{
    public Material redMat;
    public Material blueMat;
    public Material baseMat;
    public MeshRenderer floor;

    public Transform targetTrm;
    private Rigidbody _rigidbody;


    public override void Initialize()
    {
        _rigidbody = GetComponent<Rigidbody>();
        baseMat = floor.material;
    }

    public override void OnEpisodeBegin()
    {
        _rigidbody.velocity = Vector3.zero;
        transform.localPosition = new Vector3(Random.Range(-3f, 3f), 0.05f, Random.Range(-3f, 3f));
        targetTrm.localPosition = new Vector3(Random.Range(-3f, 3f), 0.5f, Random.Range(-3f, 3f));
        StartCoroutine(RecoverFloor());
    }

    private IEnumerator RecoverFloor()
    {
        yield return new WaitForSeconds(0.2f);
        floor.material = baseMat;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(targetTrm.position);
        sensor.AddObservation(transform.position);
        sensor.AddObservation(_rigidbody.velocity.x);
        sensor.AddObservation(_rigidbody.velocity.z);
    }

    // 행동값을 이용해서 에이전트의 행동을 설정하는 함수
    public override void OnActionReceived(ActionBuffers actions)
    {
        var ContinuousActions = actions.ContinuousActions;

        Vector3 direction = ((Vector3.forward * ContinuousActions[0]) + (Vector3.right* ContinuousActions[1])).normalized;
        _rigidbody.AddForce(direction * 50);

        SetReward(-0.1f);
    }

    //꼭 필요한 것은 아니다
    // 사람이 직접 수동으로 에이전트의 행동을 제어하는 방법을 설정하는 함수

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var ContinuousActionsOut = actionsOut.ContinuousActions;
        ContinuousActionsOut[0] = -Input.GetAxis("Horizontal");
        ContinuousActionsOut[1] = Input.GetAxis("Vertical");

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            floor.material = redMat;
            SetReward(-1f);
            EndEpisode();
        }
        if (collision.collider.CompareTag("Target"))
        {
            floor.material = blueMat;
            SetReward(1f);
            EndEpisode();
        }
    }

}
