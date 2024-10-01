using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MummyILAgent : Agent
{
    public float moveSpeed = 10f, turnSpeed = 50f;

    private ColorTargeter colorTargeter;
    private new Rigidbody rigidbody;
    private Vector3 originPosition;

    public Material goodMaterial, badMaterial;
    private Material originMaterial;
    private Renderer floorRenderer;


    public override void Initialize()
    {
        colorTargeter = transform.parent.GetComponent<ColorTargeter>();
        rigidbody = GetComponent<Rigidbody>();
        originPosition = transform.localPosition;
        floorRenderer = transform.parent.Find("Floor").GetComponent<Renderer>();
        originMaterial = floorRenderer.material;
    }

    public override void OnEpisodeBegin()
    {
        colorTargeter.TargetingColor();

        rigidbody.velocity = rigidbody.angularVelocity = Vector3.zero;
        transform.localPosition = originPosition;
        transform.localRotation = Quaternion.identity;
    }

    public override void CollectObservations(VectorSensor sensor)
    {

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var DiscreteActions = actions.DiscreteActions;

        Vector3 direction = Vector3.zero;
        Vector3 rotationAxis = Vector3.zero;

        //DiscreteAction[0]: ����(0), ����(1),����(2)

        switch(DiscreteActions[0])
        {
            case 1:
                direction = Vector3.forward;
                break;
            case 2:
                direction = Vector3.back;
                break;
        }

        //DiscreteAction[1]: ����(0), ��ȸ��(1),��ȸ��(2)

        switch (DiscreteActions[1])
        {
            case 1:
                rotationAxis = Vector3.down;
                break;
            case 2:
                rotationAxis = Vector3.up;
                break;
        }

        transform.Rotate(rotationAxis, turnSpeed * Time.fixedDeltaTime);
        rigidbody.MovePosition(transform.position + transform.rotation * direction * moveSpeed * Time.fixedDeltaTime);

        AddReward(-0.01f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var DiscreteActionsOut = actionsOut.DiscreteActions;

        if (Input.GetKey(KeyCode.W))
        {
            DiscreteActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            DiscreteActionsOut[0] = 2;
        }
        if (Input.GetKey(KeyCode.A))
        {
            DiscreteActionsOut[1] = 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            DiscreteActionsOut[1] = 2;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(colorTargeter.targetColor.ToString()))
        {
            StartCoroutine(ChangeFloorColor(goodMaterial));
            AddReward(1f);
            EndEpisode();
        }
        else if (collision.collider.CompareTag("Hint"))
        {

        }
        else
        {
            StartCoroutine(ChangeFloorColor(badMaterial));
            AddReward(-1f);
            EndEpisode();
        }
    }

    private IEnumerator ChangeFloorColor(Material changeMateral)
    {
        floorRenderer.material = changeMateral;
        yield return new WaitForSeconds(0.2f);
        floorRenderer.material = originMaterial;
    }

}
