using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteText : MonoBehaviour
{
    [SerializeField]
    public string spriteText;

    TextMesh text;

    // Start is called before the first frame update
    void Start()
    {
        var parent = transform.parent;

        var parentRenderer = parent.GetComponent<Renderer>();
        var renderer = GetComponent<Renderer>();
        renderer.sortingLayerID = parentRenderer.sortingLayerID;
        renderer.sortingOrder = parentRenderer.sortingOrder;

        text = GetComponent<TextMesh>();
        text.text = spriteText; // string.Format("{0}, {1}", pos.x, pos.y);
    }

    // Update is called once per frame
    void Update()
    {
        text.text = spriteText;        
    }
}
