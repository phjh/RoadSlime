using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
public class FloorAgent : Agent
{
    public Transform ballTransform;
    private Rigidbody ballRigidbody;

    //환경이 처음 시작될 때, 한번만 실행되는 함수 ( Start와 비슷한 역할)
    public override void Initialize()
    {
        ballRigidbody = ballTransform.GetComponent<Rigidbody>();
    }

    //각 에피소드가 시작될때 호출되는 함수, 황경의 상태를 초기화 하는 역할
    public override void OnEpisodeBegin()
    {
        //Floor를 x축,z축 기준으로 무작위하게 살짝 히ㅗ전
        transform.rotation = new Quaternion(0, 0, 0, 0);
        transform.Rotate(new Vector3(1, 0, 0), Random.Range(-10f, 10f));
        transform.Rotate(new Vector3(0, 0, 1), Random.Range(-10f, 10f));

        //Ball은 Velocity의 위치를 초기화한다
        ballRigidbody.velocity = new Vector3(0, 0, 0);
        ballTransform.localPosition = new Vector3(Random.Range(-1.5f, 1.5f), 1.5f, Random.Range(-1.5f, 1.5f));
    }

    //에이전트의 관측을 설정하는 함수
    public override void CollectObservations(VectorSensor sensor)
    {
        //Vector Observation (벡터관측) : 값을 통해 관측시키는것
        //관측되는 값의 개수는 8개다
        sensor.AddObservation(transform.rotation.x);
        sensor.AddObservation(transform.rotation.z);
        sensor.AddObservation(ballTransform.transform.position - transform.position);
        sensor.AddObservation(ballRigidbody.velocity);
    }

    // 행동값을 이용해서 에이전트의 행동을 설정하는 함수
    public override void OnActionReceived(ActionBuffers actions)
    {
        //Continuous Action(연속적인행동) : 2개의 값을 사용해서 행동을 설정
        var ContinuousActions = actions.ContinuousActions;
        float z_rotation = Mathf.Clamp(ContinuousActions[0], -1f, 1f);
        float x_rotation = Mathf.Clamp(ContinuousActions[1], -1f, 1f);

        transform.Rotate(new Vector3(0, 0, 1), z_rotation);
        transform.Rotate(new Vector3(1, 0, 0), x_rotation);

        if(ballTransform.position.y - transform.position.y < -2)
        {
            SetReward(-1f);
            EndEpisode();
        }
        else if(Mathf.Abs(ballTransform.position.x - transform.position.x) > 2.5)
        {
            SetReward(-1f);
            EndEpisode();
        }
        else if (Mathf.Abs(ballTransform.position.z - transform.position.z) > 2.5)
        {
            SetReward(-1f);
            EndEpisode();
        }
        else
        {
            SetReward(0.1f);
        }
    }

    //꼭 필요한 것은 아니다
    // 사람이 직접 수동으로 에이전트의 행동을 제어하는 방법을 설정하는 함수
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var ContinuousActionsOut = actionsOut.ContinuousActions;
        ContinuousActionsOut[0] = -Input.GetAxis("Horizontal");
        ContinuousActionsOut[1] = Input.GetAxis("Vertical");

    }

}
