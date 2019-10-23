using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ReelManager : MonoBehaviour
{
    [SerializeField]
    public GameObject reel;

    [SerializeField]
    public int reelCount;

    [SerializeField]
    public float reelColumnOffset = -3.5f;

    [SerializeField]
    public float reelCellWidth = 1.73f;

    [SerializeField]
    public float reelCellHeight = 1.48f;

    [SerializeField]
    public GameObject gameWindow;

    private GameObject[] reelObjects;
    private List<Reel> reels = new List<Reel>();

    [SerializeField]
    public GameObject indicator;

    [SerializeField]
    public Sprite [] activeIndicatorSprites;

    [SerializeField]
    public Sprite[] inactiveIndicatorSprites;

    [SerializeField]
    public GameObject[] winLines;

    // Start is called before the first frame update
    void Start()
    { 
        var reelList = new List<GameObject>();

        for(var i=0; i<winLines.Length; i++)
        {
            winLines[i].SetActive(false);
        }

        for(var i=0; i<reelCount; i++)
        {
            var reelObject = Instantiate(reel);

            var reelScript = reelObject.GetComponent<Reel>();
            reelScript.column = i;

            reelScript.columnOffset = reelColumnOffset;
            reelScript.cellWidth = reelCellWidth;
            reelScript.cellHeight = reelCellHeight;

            reels.Add(reelScript);

            reelList.Add(reelObject);
            //reelObject.transform.SetParent(gameWindow.transform);
        }

        reelObjects = reelList.ToArray();

    }

    public bool IsReady()
    {
        var reelsReady = reels.Sum((reel) => { return reel.readyForScore ? 1 : 0; });
        return reelsReady == reelCount;
    }

    public void PullHandle()
    {
        for(var i=0; i<winLines.Length; i++)
        {
            winLines[i].SetActive(false);
        }

        for(var i=0; i<reelCount; i++)
        {
            reels[i].SpinReel();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(IsReady())
        {
            winLines[1].SetActive(true);
            winLines[3].SetActive(true);
            winLines[6].SetActive(true);
            winLines[5].SetActive(true);

            Debug.Log("Ready for score");
        }
    }
}
