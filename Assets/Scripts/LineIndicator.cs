﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineIndicator : MonoBehaviour
{
    [SerializeField]
    public Sprite activeSprite;

    [SerializeField]
    public Sprite inactiveSprite;

    [SerializeField]
    public bool isActive;

    [SerializeField]
    public int lineNumber;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();    
    }

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.sprite = isActive ? activeSprite : inactiveSprite;

    }
}
