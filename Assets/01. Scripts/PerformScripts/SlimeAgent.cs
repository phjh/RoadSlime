using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

enum MoveDir
{
    None = 0,
    Front=1,
    Left,
    Right,
}

public class SlimeAgent : Agent
{
    private PerformMapArea area;

    public float cooltime = 0.5f;
    private float beforeTime = 0;
    public float BombTime = 4f;
    private float bombRecentTime = 0;

    public override void Initialize()
    {
        area = GetComponentInParent<PerformMapArea>();  
    }

    public override void OnEpisodeBegin()
    {
        area.ResetArea();
        area.TeleportToStart(this.transform);
        bombRecentTime = Time.time;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var DiscreteActions = actions.DiscreteActions;

        if(Time.time - bombRecentTime  > BombTime)
        {
            AddReward(-25);
            EndEpisode();
        }

        int forwardAmount = DiscreteActions[0];

        Move((MoveDir)forwardAmount);

        AddReward(-1f / MaxStep / 100f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var DiscreteAcionsOut = actionsOut.DiscreteActions;

        DiscreteAcionsOut[0] = 0;
        if (Input.GetKey(KeyCode.W))
        {
            DiscreteAcionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            DiscreteAcionsOut[0] = 2;

        }
        else if (Input.GetKey(KeyCode.D))
        {
            DiscreteAcionsOut[0] = 3;
        }
    }

    void Move(MoveDir dir)
    {
        if (Time.time - beforeTime < cooltime || dir == MoveDir.None)
            return;

        beforeTime = Time.time;

        Vector3 dirpos = transform.position;
        float angle = 0;

        switch (dir)
        {
            case MoveDir.Front:
                dirpos.z += 1;
                area.AddScore();
                bombRecentTime = Time.time;
                AddReward(1);
                break;
            case MoveDir.Left:
                dirpos.x -= 1;
                angle = -90f;
                break;
            case MoveDir.Right:
                dirpos.x += 1;
                angle = 90f;
                break;

        }

        transform.DORotateQuaternion(Quaternion.Euler(0, angle, 0),0.2f);

        Sequence seq = DOTween.Sequence();
        
        seq.Append(transform.DOMove(dirpos - (dirpos - transform.position - Vector3.up) / 2, 0.15f)).SetEase(Ease.InSine)
            .Append(transform.DOMove(dirpos, 0.15f)).SetEase(Ease.OutSine);

    }

    //Rock=0,
    //Road,
    //Fire,
    //Tree,
    //Water,
    //Raft,

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.CompareTag("Rock") || collision.gameObject.CompareTag("Fire") ||
        //    collision.gameObject.CompareTag("Tree") || collision.gameObject.CompareTag("Water") || collision.gameObject.CompareTag("Wall"))
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("Fire") || collision.gameObject.CompareTag("Wall")|| collision.gameObject.CompareTag("Water"))
        {
            AddReward(-10);
            DOTween.Clear();
            EndEpisode();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.CompareTag("Fire") || other.gameObject.CompareTag("Wall")|| other.gameObject.CompareTag("Water"))
        {
            AddReward(-10);
            DOTween.Clear();
            EndEpisode();
        }
    }

}
