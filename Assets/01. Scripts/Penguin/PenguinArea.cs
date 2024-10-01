using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PenguinArea : MonoBehaviour
{
    public PenguinAgent penguinAgent;
    public GameObject penguinBaby;
    public Fish fishprefab;
    public TextMeshPro cumulaliveRewardText;

    private List<GameObject> fishList;


    public int remainingFish
    {
        get { return fishList.Count; }
    }

    public static Vector3 ChooseRandomPosition(Vector3 center, float minAngle, float maxAngle, float minRadius, float maxRadius)
    {
        float randomAngle;
        float randomRadius;

        randomAngle = Random.Range(minAngle, maxAngle);
        randomRadius = Random.Range(minRadius, maxRadius);

        return center + Quaternion.Euler(0f, randomAngle, 0f) * Vector3.forward * randomRadius;
    }

    private void PlacePenguin()
    {
        Rigidbody rigidbody = penguinAgent.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        penguinAgent.transform.position = ChooseRandomPosition(transform.parent.position, 0f, 360f, 0f, 9f) + Vector3.up * 0.5f;
        penguinAgent.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

    }

    private void PlaceBaby()
    {
        Rigidbody rigidbody = penguinBaby.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        penguinAgent.transform.position = ChooseRandomPosition(transform.parent.position, -45f, 45, 4f, 9f) + Vector3.up * 0.5f;
        penguinAgent.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 180), 0f);

    }

    private void SpawnFish(int count)
    {
        for(int i = 0; i < count; i++)
        {
            GameObject fishObject = Instantiate(fishprefab.gameObject);
            fishObject.transform.position = ChooseRandomPosition(transform.parent.position, 100f, 260f, 2f, 13f) + Vector3.up * 0.5f;
            fishObject.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            fishObject.transform.SetParent(transform);

            fishList.Add(fishObject);
        }

    }

    private void RemoveAllFish()
    {
        if(fishList != null)
        {
            foreach(GameObject obj in fishList)
            {
                Destroy(obj);
            }
        }
        fishList = new List<GameObject>();
    }

    public void RemoveFishinList(GameObject fishObject)
    {
        fishList.Remove(fishObject);
        Destroy(fishObject);
    }

    public void ResetArea()
    {
        RemoveAllFish();
        PlacePenguin();
        PlaceBaby();
        SpawnFish(5);
    }

    private void FixedUpdate()
    {
        cumulaliveRewardText.text = penguinAgent.GetCumulativeReward().ToString("0.00");
    }

}
