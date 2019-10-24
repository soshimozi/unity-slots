using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using System;

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

    [SerializeField]
    public GameObject indicator;

    [SerializeField]
    public Sprite [] activeIndicatorSprites;

    [SerializeField]
    public Sprite[] inactiveIndicatorSprites;

    [SerializeField]
    public GameObject[] winLines;

    [SerializeField]
    public GameObject autoSpinButtonObject;

    private AutoSpinButton autoSpinButton;

    private int stoppedCount;
    private GameObject[] reelObjects;
    private List<Reel> reels = new List<Reel>();

    private int selectedPayLines;

    private bool autoSpin;
    private bool readyToSpin;
    private bool spinning;
    private bool readyToShowResults;
    private bool isDoubleDown;

    private int ticketCount;
    private int currentBetAmount;
    private int maxBet;
    private string totalWinningsText;

    private LineIndicator[] leftIndicators = new LineIndicator[9];
    private LineIndicator[] rightIndicators = new LineIndicator[9];

    private readonly Bet currentBet = new Bet();

    const float top = 2.532f;
    const float rightColumnEdge = 4.91f;
    const float verticalSpacing = 0.472f;
    const float leftColumnEdge = -5.07f;

    readonly int[][] paylineCheckPatterns = {
        new [] { 0, 0, 0, 0, 0 },
        new [] { 0, 0, 1, 0, 0 },
        new [] { 0, 1, 2, 1, 0 },
        new [] { 1, 0, 0, 0, 1 },
        new [] { 1, 1, 1, 1, 1 },
        new [] { 1, 2, 2, 2, 1 },
        new [] { 2, 1, 0, 1, 2 },
        new [] { 2, 2, 1, 2, 2 },
        new [] { 2, 2, 2, 2, 2 }
    };

    readonly int[] paylineCounts = { 1, 3, 5, 7, 9 };

    readonly int[,] payoutTable = new[,]
    {
        {15, 45, 200},
        {5, 20, 100},
        {45, 200, 1200},
        {15, 45, 200},
        {10, 30, 150},
        {5, 20, 100},
        {10, 30, 150},
        {45, 200, 1200}
    };


    // Start is called before the first frame update
    void Start()
    {
        ClearWinLines();
        reelObjects = BuildReels().ToArray();
        AddIndicatorIcons();

        autoSpin = false;
        readyToSpin = true;
        spinning = false;

        ticketCount = 100000;
        currentBetAmount = 1;
        maxBet = 15;
        totalWinningsText = "";

        autoSpinButton = autoSpinButtonObject.GetComponent<AutoSpinButton>();
        autoSpinButton.SetDisable(false);

        autoSpinButton.OnSpin += AutoSpinButton_OnSpin;
    }

    private void AddIndicatorIcons()
    {
        for (var i = 0; i < 9; i++)
        {
            AddIndicators(i);
        }
    }

    private List<GameObject> BuildReels()
    {
        var reelList = new List<GameObject>();
        for (var i = 0; i < reelCount; i++)
        {
            var reelObject = Instantiate(reel);

            var reelScript = reelObject.GetComponent<Reel>();
            reelScript.column = i;

            reelScript.columnOffset = reelColumnOffset;
            reelScript.cellWidth = reelCellWidth;
            reelScript.cellHeight = reelCellHeight;

            reels.Add(reelScript);

            reelList.Add(reelObject);

            reelScript.ReelStopped += ReelScript_ReelStopped;
        }

        return reelList;
    }

    private void AutoSpinButton_OnSpin(object sender, EventArgs e)
    {
        if(readyToSpin)
        {
            autoSpinButton.SetDisable(true);
            PullHandle(true);
        }
    }

    private void AddIndicators(int lineNumber)
    {
        rightIndicators[lineNumber] = CreateLineIndicator(lineNumber, rightColumnEdge);
        leftIndicators[lineNumber] = CreateLineIndicator(lineNumber, leftColumnEdge);
    }

    private int [] GetWinLines()
    {
        List<int> winLines = new List<int>();
        for (var i = 0; i < paylineCounts[currentBet.PayLinesIndex]; i++)
        {
            var matchCount = CheckPayLine(paylineCheckPatterns[i]);

            //// we have a winner
            if (matchCount > 1)
            {
                winLines.Add(i);
            }
        }


        return winLines.ToArray();
    }

    private int CheckPayLine(int[] pattern)
    {
        var displayCells = new List<int[]>();
        for (var reelIndex = 0; reelIndex < reelCount; reelIndex++)
        {
            displayCells.Add(reels[reelIndex].GetDisplayedCells());
        }

        var lastCell = displayCells[0][pattern[0]];

        var matchCount = 0;
        for (var i = 1; i < reelCount; i++)
        {
            if (lastCell != displayCells[i][pattern[i]])
            {
                break;
            }

            lastCell = displayCells[i][pattern[i]];
            matchCount++;
        }

        return matchCount;
    }

    private LineIndicator CreateLineIndicator(int lineNumber,float x)
    {
        var indicatorObject = Instantiate(indicator);

        var lineIndicator = indicatorObject.GetComponent<LineIndicator>();

        lineIndicator.activeSprite = activeIndicatorSprites[lineNumber];
        lineIndicator.inactiveSprite = inactiveIndicatorSprites[lineNumber];

        lineIndicator.isActive = false;

        lineIndicator.transform.DOMoveX(x, 0);
        lineIndicator.transform.DOMoveY(top - (lineNumber * verticalSpacing), 0);

        lineIndicator.lineNumber = lineNumber;

        return lineIndicator;
    }

    private void SetReadyToSpin()
    {
        autoSpinButton.SetDisable(false);
        readyToSpin = true;
    }

    private void CalculateScore()
    {
        int[,] tableu = new int[5, 3];

        for (var i = 0; i < 5; i++)
        {
            var values = reels[i].GetDisplayedCells();

            for(var j=0; j< 3; j++)
            {
                tableu[i, j] = values[j];
            }
        }


        //var winLines = GetWinLines();
        //if (winLines.Length == 0) return;


        Debug.Log(string.Format("{0}, {1}, {2}, {3}, {4}", tableu[0, 0], tableu[1, 0], tableu[2, 0], tableu[3, 0], tableu[4, 0]));
        Debug.Log(string.Format("{0}, {1}, {2}, {3}, {4}", tableu[0, 1], tableu[1, 1], tableu[2, 1], tableu[3, 1], tableu[4, 1]));
        Debug.Log(string.Format("{0}, {1}, {2}, {3}, {4}", tableu[0, 2], tableu[1, 2], tableu[2, 2], tableu[3, 2], tableu[4, 2]));

        StartCoroutine(DisplayWinLines());
        //StopCoroutine(cr);

        // we should set up a timer to turn on each one in turn
        // until we pull the handle again
        //winLines[1].SetActive(true);
        //winLines[3].SetActive(true);
        //winLines[6].SetActive(true);
        //winLines[5].SetActive(true);

        //leftIndicators[1].isActive = true;
        //rightIndicators[1].isActive = true;

        //leftIndicators[3].isActive = true;
        //rightIndicators[3].isActive = true;

        //leftIndicators[5].isActive = true;
        //rightIndicators[5].isActive = true;

        //leftIndicators[6].isActive = true;
        //rightIndicators[6].isActive = true;

    }

    IEnumerator DisplayWinLines()
    {
        var winLines = GetWinLines();
        if (winLines.Length == 0)
        {
            // no win lines, so we can spin immediately
            SetReadyToSpin();
            yield break;
        }

        // this is a problem
        if (winLines.Length > paylineCounts[currentBet.PayLinesIndex])
        {
            SetReadyToSpin();
            yield break;
        }

        var currentPayline = 0;

        while (!spinning)
        {
            if (spinning)
            {
                break;
            }

            DisplayWinLine(winLines[currentPayline]);
            yield return new WaitForSeconds(1.5f);
            HideWinLine(winLines[currentPayline]);

            currentPayline++;

            if (currentPayline >= winLines.Length)
            {
                currentPayline = 0;

                // we have cycled through once, so ready to spin now
                if (!readyToSpin && !spinning)
                {
                    SetReadyToSpin();
                }
            }
        }

        yield break;

        //DisplayWinLine(1);
        //    yield return new WaitForSeconds(1.5f);
        //    HideWinLine(1);

        //    //if (!readyToSpin) break;

        //    DisplayWinLine(3);
        //    yield return new WaitForSeconds(1.5f);
        //    HideWinLine(3);

        //   // if (!readyToSpin) break;

        //    DisplayWinLine(5);
        //    yield return new WaitForSeconds(1.5f);
        //    HideWinLine(5);

        //   // if (!readyToSpin) break;

        //    DisplayWinLine(6);
        //    yield return new WaitForSeconds(1.5f);
        //    HideWinLine(6);

        //    SetReadyToSpin();

        //while (readyToSpin)
        //{
        //    if (!readyToSpin) yield break;

        //    DisplayWinLine(1);
        //    yield return new WaitForSeconds(1.5f);
        //    HideWinLine(1);

        //    //if (!readyToSpin) break;

        //    if (!readyToSpin) yield break;
        //    DisplayWinLine(3);
        //    yield return new WaitForSeconds(1.5f);
        //    HideWinLine(3);

        //    // if (!readyToSpin) break;

        //    if (!readyToSpin) yield break;
        //    DisplayWinLine(5);
        //    yield return new WaitForSeconds(1.5f);
        //    HideWinLine(5);

        //    // if (!readyToSpin) break;

        //    if (!readyToSpin) yield break;
        //    DisplayWinLine(6);
        //    yield return new WaitForSeconds(1.5f);
        //    HideWinLine(6);

        //    if (!readyToSpin) yield break;
        //}


        //if (!readyToSpin) yield break;
    }

    void DisplayWinLine(int lineIndex)
    {
        leftIndicators[lineIndex].isActive = true;
        rightIndicators[lineIndex].isActive = true;
        winLines[lineIndex].SetActive(true);
    }

    void HideWinLine(int lineIndex)
    {
        leftIndicators[lineIndex].isActive = false;
        rightIndicators[lineIndex].isActive = false;
        winLines[lineIndex].SetActive(false);
    }

    private void ReelScript_ReelStopped(object sender, System.EventArgs e)
    {
        stoppedCount += 1;

        if(stoppedCount >= reels.Count)
        {
            spinning = false;

            // get score here
            CalculateScore();
        }
    }

    private bool IsReady()
    {
        var reelsReady = reels.Sum((reel) => { return reel.readyForScore ? 1 : 0; });
        return reelsReady == reelCount;
    }

    private void ClearIndicators()
    {
        foreach(var indicator in rightIndicators)
        {
            indicator.isActive = false;
        }

        foreach (var indicator in leftIndicators)
        {
            indicator.isActive = false;
        }

    }

    private void ClearWinLines()
    {
        for (var i = 0; i < winLines.Length; i++)
        {
            winLines[i].SetActive(false);
        }
    }

    public void PullHandle(bool autoSpin)
    {
        ClearWinLines();

        ClearIndicators();

        readyToShowResults = false;
        readyToSpin = false;

        spinning = true;

        currentBet.Amount = paylineCounts[selectedPayLines] * currentBetAmount;
        currentBet.IsDoubleDown = isDoubleDown;
        currentBet.PayLinesIndex = 4;

        stoppedCount = 0;
        for (var i = 0; i < reelCount; i++)
        {
            reels[i].SpinReel();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
