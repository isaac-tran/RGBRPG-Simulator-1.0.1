using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetListButton : MonoBehaviour
{
    [SerializeField] private int indexInTargetList;

    public int GetIndexInTargetList() { return indexInTargetList; }

    public void SetIndexInTargetList(int newIndex)
    {
        indexInTargetList = newIndex;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
