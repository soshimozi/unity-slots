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

    private ReelManager reelManager;

    private CircleCollider2D collider;
    private SpriteRenderer spriteRenderer;

    private bool disabled = false;

    // Start is called before the first frame update
    void Start()
    {
        reelManager = reelManagerObject.GetComponent<ReelManager>();

        collider = GetComponent<CircleCollider2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(reelManager.IsReady())
        {
            spriteRenderer.color = Color.white;
            disabled = false;
        }
    }

    private void OnMouseUpAsButton()
    {
        if (!disabled)
        {
            reelManager.PullHandle();
            spriteRenderer.color = Color.gray;
            disabled = true;
        }
    }

    private void OnMouseUp()
    {
        spriteRenderer.sprite = inactiveSprite;
    }

    private void OnMouseDown()
    {
        if(!disabled)
        spriteRenderer.sprite = activeSprite;
    }
}
