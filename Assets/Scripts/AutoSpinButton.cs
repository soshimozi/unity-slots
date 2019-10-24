using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSpinButton : MonoBehaviour
{
    [SerializeField]
    public GameObject reelManagerObject;

    [SerializeField]
    public Sprite inactiveSprite;
    [SerializeField]
    public Sprite activeSprite;

    //private ReelManager reelManager;

    //private CircleCollider2D collider;
    private SpriteRenderer spriteRenderer;

    private bool isDisabled = false;

    public event EventHandler OnSpin;

    // Start is called before the first frame update
    void Start()
    {
        //reelManager = reelManagerObject.GetComponent<ReelManager>();

        //collider = GetComponent<CircleCollider2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
        //if(reelManager.IsReady())
        //{
        //    spriteRenderer.color = Color.white;
        //    disabled = false;
        //}
    }

    private void OnMouseUpAsButton()
    {
        if(!isDisabled)
        OnSpin?.Invoke(this, new EventArgs());
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
