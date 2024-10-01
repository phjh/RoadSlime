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

    //ȯ���� ó�� ���۵� ��, �ѹ��� ����Ǵ� �Լ� ( Start�� ����� ����)
    public override void Initialize()
    {
        ballRigidbody = ballTransform.GetComponent<Rigidbody>();
    }

    //�� ���Ǽҵ尡 ���۵ɶ� ȣ��Ǵ� �Լ�, Ȳ���� ���¸� �ʱ�ȭ �ϴ� ����
    public override void OnEpisodeBegin()
    {
        //Floor�� x��,z�� �������� �������ϰ� ��¦ ������
        transform.rotation = new Quaternion(0, 0, 0, 0);
        transform.Rotate(new Vector3(1, 0, 0), Random.Range(-10f, 10f));
        transform.Rotate(new Vector3(0, 0, 1), Random.Range(-10f, 10f));

        //Ball�� Velocity�� ��ġ�� �ʱ�ȭ�Ѵ�
        ballRigidbody.velocity = new Vector3(0, 0, 0);
        ballTransform.localPosition = new Vector3(Random.Range(-1.5f, 1.5f), 1.5f, Random.Range(-1.5f, 1.5f));
    }

    //������Ʈ�� ������ �����ϴ� �Լ�
    public override void CollectObservations(VectorSensor sensor)
    {
        //Vector Observation (���Ͱ���) : ���� ���� ������Ű�°�
        //�����Ǵ� ���� ������ 8����
        sensor.AddObservation(transform.rotation.x);
        sensor.AddObservation(transform.rotation.z);
        sensor.AddObservation(ballTransform.transform.position - transform.position);
        sensor.AddObservation(ballRigidbody.velocity);
    }

    // �ൿ���� �̿��ؼ� ������Ʈ�� �ൿ�� �����ϴ� �Լ�
    public override void OnActionReceived(ActionBuffers actions)
    {
        //Continuous Action(���������ൿ) : 2���� ���� ����ؼ� �ൿ�� ����
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

    //�� �ʿ��� ���� �ƴϴ�
    // ����� ���� �������� ������Ʈ�� �ൿ�� �����ϴ� ����� �����ϴ� �Լ�
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var ContinuousActionsOut = actionsOut.ContinuousActions;
        ContinuousActionsOut[0] = -Input.GetAxis("Horizontal");
        ContinuousActionsOut[1] = Input.GetAxis("Vertical");

    }

}
