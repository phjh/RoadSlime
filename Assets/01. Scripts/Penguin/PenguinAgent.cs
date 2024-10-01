using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.VFX;

public class PenguinAgent : Agent
{
    public float moveSpeed = 5f;
    public float turnspeed = 180f;
    public GameObject heartPrefab;
    public GameObject regurgitatedFishPrefab;

    private PenguinArea penguinArea;
    private new Rigidbody rigidbody;
    private GameObject babyPenguin;

    private bool isFull;


    public override void Initialize()
    {
        penguinArea = transform.parent.Find("PenguinArea").GetComponent<PenguinArea>();
        babyPenguin = penguinArea.penguinBaby;
        rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        isFull = false;
        penguinArea.ResetArea();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //과측값의 개수는 총 8개다 (1 + 1 + 3 + 3
        sensor.AddObservation(isFull);
        sensor.AddObservation(Vector3.Distance(transform.position, babyPenguin.transform.position));
        sensor.AddObservation((babyPenguin.transform.position - transform.position).normalized);
        sensor.AddObservation(transform.forward);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var DiscreteActions = actions.DiscreteActions;

        //앞으로 가지 않을지(0), 앞으로 갈지(1)
        int forwardAmount = DiscreteActions[0];

        //회전을 안할지(0), 왼쪽으로회전(1), 오른쪽으로회전(2)
        int turnAmount = 0;
        if (DiscreteActions[1] == 1)
        {
            turnAmount = -1;
        }
        else if (DiscreteActions[1] == 2)
        {
            turnAmount = 1;
        }

        rigidbody.MovePosition(transform.position + transform.forward * forwardAmount * moveSpeed * Time.fixedDeltaTime);
        transform.Rotate(Vector3.up * turnAmount * turnspeed * Time.fixedDeltaTime);

        AddReward(-1f / MaxStep);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var DiscreteAcionsOut = actionsOut.DiscreteActions;

        if (Input.GetKey(KeyCode.W))
        {
            DiscreteAcionsOut[0] = 1;
        }
        else if(Input.GetKey(KeyCode.A))
        {
            DiscreteAcionsOut[1] = 1;

        }
        else if (Input.GetKey(KeyCode.D))
        {
            DiscreteAcionsOut[1] = 2;

        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Fish"))
        {
            EatFish(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("BabyPenguin"))
        {
            RegurgitateFish();
        }
    }

    private void EatFish(GameObject fishObject)
    {
        if (isFull) return;
        isFull = true;

        penguinArea.RemoveFishinList(fishObject);
        AddReward(1f);
    }

    private void RegurgitateFish()
    {
        if (!isFull) return;
        isFull = false;

        GameObject regurgitatedFish = Instantiate(regurgitatedFishPrefab);
        regurgitatedFish.transform.parent = transform.parent;
        regurgitatedFish.transform.localPosition = babyPenguin.transform.localPosition + Vector3.up * 0.01f;
        Destroy(regurgitatedFish, 4f);

        GameObject heart = Instantiate(heartPrefab);
        heart.transform.parent = transform.parent;
        heart.transform.localPosition = babyPenguin.transform.localPosition + Vector3.up;
        Destroy(heart, 4f);

        if(penguinArea.remainingFish <= 0)
        {
            EndEpisode();
        }

    }


}
