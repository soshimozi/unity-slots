using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelCell : MonoBehaviour
{
    public Sprite[] sArray;

    public int symbolType;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = sArray[symbolType];    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
