using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButton : MonoBehaviour
{
    [SerializeField]
    public Sprite inactiveSprite;

    [SerializeField]
    public Sprite activeSprite;

    private SpriteRenderer spriteRenderer;

    private bool isDisabled = false;

    public event EventHandler OnClick;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
    }

    private void OnMouseUpAsButton()
    {
        if(!isDisabled)
        OnClick?.Invoke(this, new EventArgs());
    }

    public void SetDisable(bool disable)
    {
        isDisabled = disable;

        if(isDisabled)
        {
            spriteRenderer.color = Color.gray;
        }
        else
        {
            spriteRenderer.color = Color.white;
        }

    }

    private void OnMouseUp()
    {
        spriteRenderer.sprite = inactiveSprite;
    }

    private void OnMouseDown()
    {
        if(!isDisabled)
        spriteRenderer.sprite = activeSprite;
    }
}
