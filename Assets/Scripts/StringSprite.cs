using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class StringSprite : MonoBehaviour
{
    SpriteText tm;

    [SerializeField]
    public string spriteString;

    // Start is called before the first frame update
    void Start()
    {
        tm = GetComponentInChildren<SpriteText>();
    }

    // Update is called once per frame
    void Update()
    {
        tm.spriteText = spriteString;
    }
}
