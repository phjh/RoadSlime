using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public enum BlockType
{
//  Coin,
    Rock=0,
    Road,
    Fire,
    Tree,
    Water,
    Raft,
    End,
}

public enum LineType
{
    //여기에 줄 타입을 넣는다
    //그리고 그타입에 따라 블럭 세팅
    GrassLand=0,
    River = 1,

}

[Serializable]
public struct LineInfo
{
    public List<BlockType> line;
    public List<GameObject> obj;
}

[Serializable]
public struct MapLine
{
    public List<BlockType> type;
    public List<GameObject> obj;
}

public class PerformMapArea : MonoBehaviour
{
    private List<Dictionary<int, GameObject>> mapLists = new();
    public List<LineInfo> mapline;
    public List<MapLine> maps = new List<MapLine>();
    private int score = 0;
    public TextMeshProUGUI text;
    public Transform objList;
    private Dictionary<int, BlockType> BlockConverter = new();

    void Start()
    {
        ConnectBlocks();
        ResetMap();
    }

    private void ConnectBlocks()
    {
        for(int i = 0; i < mapline.Count; i++)
        {
            Dictionary<int, GameObject> map = new();
            for (int j = 0; j < mapline[i].line.Count; j++) 
            {
                map.Add(j,mapline[i].obj[j]);
                BlockConverter.Add(i * 100 + j, mapline[i].line[j]);
            }
            mapLists.Add(map);
        }
    }

    public void AddScore()
    {
        score++;
        if (score % 10 == 0)
            AddMap(10);
        text.text = score.ToString();
    }

    public void ResetArea()
    {
        RemoveMap();
        ResetMap();
        ResetScore();
    }

    private void ResetScore()
    {
        score = 0;
        text.text = score.ToString();
    }

    private void ResetMap()
    {
        maps.Clear();
        AddMap(40);
    }

    public void TeleportToStart(Transform obj)
    {
        obj.position = new Vector3(transform.position.x + 10, -0.1f, -1);
    }

    private void AddMap(int lines)
    {
        for(int i=0;i< lines; i++)
        {
            MapLine line = new MapLine();
            line.obj = new();
            line.type = new();
            int randbiome = 0;
            randbiome = UnityEngine.Random.Range(0, 4) / 3;
            for (int j = 0; j < 20; j++) 
            {
                int rand = UnityEngine.Random.Range(0, mapline[randbiome].line.Count);
                if (maps.Count <= 2)
                    rand = 0;

                if (j != 0 && maps.Count > 2 && j != 19)
                    if (isBlocked(j, line))
                        rand = 0;
                    

                var obj = Instantiate(mapLists[randbiome][rand], objList.transform);       
                obj.transform.position = new Vector3(transform.position.x + j, -0.499f, maps.Count);
                line.obj.Add(obj);
                line.type.Add(BlockConverter[rand]);
            }
            maps.Add(line);
        }

    }

    //좌우 쭉 탐색후 앞으로 가는게 있는지 없는지로 코드 수정 필요.
    private bool isBlocked(int j, MapLine nowLine)
    {
        int mapIndex = maps.Count - 2;

        if (!(maps[mapIndex].type[j + 1] == BlockType.Fire || maps[mapIndex].type[j + 1] == BlockType.Water))
            return false;

        while (!(maps[mapIndex].type[j] == BlockType.Water || maps[mapIndex].type[j] == BlockType.Fire) && j > 1) 
        {
            j--;
            if (!(nowLine.type[j] == BlockType.Water || nowLine.type[j] == BlockType.Fire))
                return false;
        }
        return true;    
    }


    private void RemoveMap()
    {
        foreach(var map in maps)
        {
            foreach(var obj in map.obj)
            {
                Destroy(obj);
            }
        }

    }

}
    
