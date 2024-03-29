﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntegerSprite : MonoBehaviour
{
    SpriteText tm;

    [SerializeField]
    public int number;

    // Start is called before the first frame update
    void Start()
    {
        tm = GetComponentInChildren<SpriteText>();
    }

    // Update is called once per frame
    void Update()
    {
        tm.spriteText = number.ToString();
    }
}
