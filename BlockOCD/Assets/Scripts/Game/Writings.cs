using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Writings : MonoBehaviour
{
    public List<GameObject> writingObjects;
    void Start()
    {
        GameEvent.ShowWriting += ShowWriting;
    }

    private void OnDisable()
    {
        GameEvent.ShowWriting -= ShowWriting;
    }

    private void ShowWriting()
    {
        var index = Random.Range(0, writingObjects.Count);
        writingObjects[index].SetActive(true);
    }
}
