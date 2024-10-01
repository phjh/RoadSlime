using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MummyRayAgent : Agent
{
    public float moveSpeed = 30f;
    public float turnSpeed = 100f;

    public Material goodMaterial;
    public Material badMaterial;
    Material orginMaterial;
    public MeshRenderer renderers;

    ItemSpawner itemSpawner;

    public override void Initialize()
    {
        itemSpawner = transform.parent.GetComponent<ItemSpawner>(); 
        orginMaterial = renderers.material;
    }

    public override void OnEpisodeBegin()
    {
        itemSpawner.SpawnItems();
        transform.localPosition = new Vector3(Random.Range(-22f, 22f), 0.05f, Random.Range(-22f, 22f));
        transform.localRotation = Quaternion.Euler(Vector3.up * Random.Range(0f, 360f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //DiscreteAction  : 이산정인행동 - 정해진 개수의 선택지 중에서 행동을 선택
        //2개의 행동값을 사용
        var DiscreteActions = actions.DiscreteActions;
        Vector3 direction = Vector3.zero;
        Vector3 rotation = Vector3.zero;

        switch (DiscreteActions[0])
        {
            case 0: direction = Vector3.zero;
                break;
            case 1: direction = Vector3.forward;
                break;
            case 2: direction = -Vector3.forward;
                break;
        }

        switch (DiscreteActions[1])
        {
            case 1:
                direction = Vector3.left;
                break;
            case 2:
                direction = Vector3.right;
                break;
        }

        transform.Rotate(rotation, turnSpeed * Time.fixedDeltaTime);
        transform.localPosition += direction * moveSpeed * Time.fixedDeltaTime;

        AddReward(-11f / 1000);

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var DiscreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.W))
        {
            DiscreteActionsOut[0] = 1;
        }
        if(Input.GetKey(KeyCode.S))
        {
            DiscreteActionsOut[0] = 2;
        }
        if (Input.GetKey(KeyCode.A))
        {
            DiscreteActionsOut[1] = 1;
        }
        if(Input.GetKey(KeyCode.D))
        {
            DiscreteActionsOut[1] = 2;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("GoodItem"))
        {
            Destroy(collision.gameObject);
            AddReward(1f);
            StartCoroutine(ChangeFloorColor(goodMaterial));
        }
        if (collision.collider.CompareTag("BadItem"))
        {
            AddReward(-1f);
            EndEpisode();
            StartCoroutine(ChangeFloorColor(badMaterial));
        }
        if (collision.collider.CompareTag("Wall"))
        {
            AddReward(-0.1f);
            EndEpisode();
        }
    }

    private IEnumerator ChangeFloorColor(Material changeMat)
    {
        renderers.material = changeMat;
        yield return new WaitForSeconds(0.2f);
        renderers.material = orginMaterial;
    }

}
