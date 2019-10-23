using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Reel : MonoBehaviour
{
    [SerializeField]
    public float maxReelSpeed = 2.8f;

    [SerializeField]
    public int column;

    [SerializeField]
    public GameObject reelCell;

    [SerializeField]
    public float columnOffset = -3.5f;

    [SerializeField]
    public float cellWidth = 1.73f;

    [SerializeField]
    public float cellHeight = 1.48f;

    private List<GameObject> cells = new List<GameObject>();

    private float reelSpeed = 0.0f;
    private float reelFriction = 0.0f;

    private bool reelStopped = true;
    private bool spinningUp = false;

    public bool readyForScore;

    private float startTime = 0.0f;
    private float maxY;
    private float halfCellHeight;
    private int reelHeight;

    private readonly float[] weights = new float[] { 0.1f, 0.25f, 0.025f, 0.1f, 0.125f, 0.25f, 0.125f, 0.025f };

    private const int totalCells = 100;
    private const int visibleCells = 3;

    // Start is called before the first frame update
    void Start()
    {
        // reel predefined symbols
        int[] reelSymbols = new int[] { 0, 0, 0, 0, 1, 2, 1, 3, 4, 5 }; //{0,0,0,0,1,1,1,2,2,3,3,3,4,4,4,4,1,5,5,5,5,2,2,6,6,6,6,3,7,7,7,7};


        halfCellHeight = (cellHeight / 2.0f);

        var indexes = BuildIndexes(weights);

        maxY = (-cellHeight * (indexes.Length - 3));
        maxY += halfCellHeight;

        // instantiating symbols on reel
        for (int i = 0; i < indexes.Length; i++)
        {
            var cell = Instantiate(reelCell, new Vector3(0, i * cellHeight, 0), Quaternion.identity);
            cell.GetComponent<ReelCell>().symbolType = indexes[i];
            cells.Add(cell);

            cell.transform.SetParent(transform);
        }

        transform.position = new Vector3(columnOffset + (column * cellWidth), maxY);

        reelHeight = indexes.Length - 3;

        readyForScore = false;
    }

    public void SpinReel()
    {
        // dots per inch - TODO: get from actual sprites?
        var dpi = 100;

        spinningUp = true;
        reelStopped = true;
        readyForScore = false;
        reelFriction = 0.0f;

        var destinationY = (transform.position.y - (cellHeight / 2));

        transform
        .DOMoveY(destinationY, 0.5f)
        .SetEase(Ease.InOutExpo)
        .OnComplete(() =>
        {
            reelStopped = false;
            startTime = Time.time;
        });
    }

    IEnumerator Stop(float waitTime)
    {
        yield return new WaitForSeconds(waitTime/1000.0f);

        Debug.Log("stopping");
        reelFriction = 0.1f;
    }

    void Update()
    {
        if (reelStopped) return;

        if (spinningUp)
        {
            Debug.Log("spinning up");

            var dt = Time.time - startTime;
            dt = dt / 10.0f;

            reelSpeed += (dt * dt) * 4;

            Debug.Log("reelSpeed: " + reelSpeed);

            if (reelSpeed >= maxReelSpeed)
            {
                reelSpeed = maxReelSpeed;

                var randomTime = Random.Range(800, 2000);

                // fire a timer after this time
                //this.game.time.events.add(randomTime, this.stop, this);
                StartCoroutine(Stop(randomTime));

                spinningUp = false;
            }
        }
        else
        {
            reelSpeed -= reelFriction;
            if (reelSpeed <= 0)
            { 
                // we are stopped
                Debug.Log("stopped!");

                reelStopped = true;
                reelSpeed = 0.0f;
                reelFriction = 0.0f;

                var offset = Math.Abs(transform.position.y);

                var remainder = offset % cellHeight;

                // get closest y
                var closestY = (int)Math.Ceiling(offset / cellHeight);
                closestY -= 1;

                if (remainder > halfCellHeight) { closestY -= 1; }

                if (closestY > reelHeight) { closestY = 0; }

                closestY -= 1;

                if (closestY < 2) closestY = totalCells - closestY;


                var destinationY = -(closestY * cellHeight);
                destinationY += halfCellHeight;

                transform
                .DOMoveY(destinationY, 0.35f)
                .SetEase(Ease.OutBounce).
                OnComplete(() =>
               {
                   readyForScore = true;
               });
                    
                //tween.to({ y: destinationY }, 350, Phaser.Easing.Bounce.Out, false).start();
                //tween.onComplete.addOnce(() => { this.readyForScore = true }, this);
            }
        }

        if (!reelStopped)
        {

            transform.DOMoveY(transform.position.y + reelSpeed, 0.0f);
            if (transform.position.y >= -(halfCellHeight + cellHeight))
            {
                var diff = Math.Abs(transform.position.y + (halfCellHeight + cellHeight));
                transform.DOMoveY(maxY + diff - cellHeight, 0.0f);
            }
        }
    }

    int [] BuildIndexes(float[] weights)
    {
        var indexes = new LinkedList<int>();

        // 100 on each real, 3 visible which go on top

        var weightedRand = WeightedRand(weights);

        for(var i=0; i < totalCells - visibleCells; i++ )
        {
            indexes.AddLast(weightedRand());
        }

        var visibleCellList = new List<int>();
        for(var i=0; i< visibleCells; i++)
        {
            var index = weightedRand();
            indexes.AddLast(index);
            visibleCellList.Add(index);
        }


        indexes.AddLast(indexes.First.Value);

        for (var i=1; i<=visibleCells; i++)
        {
            indexes.AddFirst(visibleCellList[visibleCells - i]);
        }


        var list = new List<int>();
        while(indexes.First != null)
        {
            list.Add(indexes.First.Value);
            indexes.RemoveFirst();
        }

        return list.ToArray();
    }

    Func<int> WeightedRand(float [] spec)
    {
        var table = new List<int>();

        for(int i=0; i<spec.Length; i++)
        {
            for(int j=0; j < spec[i] * 1000; j++)
            {
                table.Add(i);
            }
        }

        return () => {
            var idx = (int)Math.Floor(Random.value * table.Count);
            return table[idx];
        };
    }

}
