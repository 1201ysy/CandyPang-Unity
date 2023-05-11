using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Board : MonoBehaviour
{

    [SerializeField] private GameObject candyTile;
    [SerializeField] private Candy[] candies; // candies[0]
    [SerializeField] private Candy[] specialCandies;

    public Candy[,] candyMap; // candyMap[0, 3]
    public int width;
    public int height;

    private float moveDuration = 0.5f;
    private float newCandyMoveDuration = 0.2f;




    // Start is called before the first frame update
    void Start()
    {
        SetSize();
        InitCandyMap();
    }

    private void SetSize()
    {
        width = GameManager.instance.GetBoardWitdh();
        height = GameManager.instance.GetBoardHeight();
    }

    private void InitCandyMap()
    {

        candyMap = new Candy[width, height];

        Vector2 centerPos = GetCenterPosition();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 pos = new Vector2(x - centerPos.x, y - centerPos.y);
                Instantiate(candyTile, pos, Quaternion.identity);

                CreateRandomCandy(x, y, pos, true);
            }
        }
    }

    private Vector2 GetCenterPosition()
    {
        return new Vector2((width - 1) * 0.5f, (height - 1) * 0.5f);
    }

    private Candy CreateRandomCandy(int x, int y, Vector2 pos, bool checkMatch)
    {
        int index = Random.Range(0, candies.Length);

        if (checkMatch)
        {
            int retryCount = 0;
            while (MatchHorizontally(x, y, index) || MatchVertically(x, y, index))
            {
                index = Random.Range(0, candies.Length);
                retryCount++;
                //Debug.Log("Retry Count : " + retryCount);
            }
        }
        Candy candy = Instantiate(candies[index], pos, Quaternion.identity);
        candy.Init(x, y);

        candyMap[x, y] = candy;

        return candy;

    }

    private Candy CreateSpecialCandy(int x, int y, Vector2 pos)
    {
        int index = Random.Range(0, specialCandies.Length);

        Candy candy = Instantiate(specialCandies[index], pos, Quaternion.identity);
        candy.Init(x, y);

        candyMap[x, y] = candy;

        return candy;
    }

    private bool MatchHorizontally(int x, int y, int index)
    {
        if (x > 1)
        {
            if (candyMap[x - 1, y].type == candies[index].type && candyMap[x - 2, y].type == candies[index].type)
            {
                return true;
            }
        }

        return false;
    }

    private bool MatchVertically(int x, int y, int index)
    {
        if (y > 1)
        {
            if (candyMap[x, y - 1].type == candies[index].type && candyMap[x, y - 2].type == candies[index].type)
            {
                return true;
            }
        }

        return false;
    }

    public void StartRemoveCandiesRoutine()
    {
        StartCoroutine("RemoveCandiesRoutine");
    }

    IEnumerator RemoveCandiesRoutine()
    {
        MatchChecker matchChecker = FindObjectOfType<MatchChecker>();
        bool addSpecial = false;

        for (int i = 0; i < 100; i++)
        {
            if (matchChecker.candyList.Count > 3 && !matchChecker.containsSpecialCandy)
            {
                addSpecial = true;
            }
            else
            {
                addSpecial = false;
            }

            RemoveMatchedCandies(matchChecker);
            yield return new WaitForSeconds(0.8f);
            DropCandies();
            yield return new WaitForSeconds(0.8f);
            FillCandies(addSpecial);
            yield return new WaitForSeconds(0.8f);
            matchChecker.CheckAllMatches();
            if (matchChecker.candyList.Count == 0)
            {
                break;
            }
        }

        GameManager.instance.canMoveCandy = true;
        GameManager.instance.CheckGameOver();

    }

    private void DropCandies()
    {
        int emptySpace;
        Vector2 centerPos = GetCenterPosition();

        for (int x = 0; x < width; x++)
        {
            emptySpace = 0;
            for (int y = 0; y < height; y++)
            {
                if (candyMap[x, y] == null)
                {
                    emptySpace++;
                }
                else if (emptySpace > 0)
                {
                    Candy candy = candyMap[x, y];
                    candyMap[x, y] = null;

                    candyMap[x, y - emptySpace] = candy;
                    candy.y -= emptySpace;

                    Vector2 pos = new Vector2(x - centerPos.x, y - centerPos.y - emptySpace);

                    candy.transform.DOMove(pos, moveDuration).SetEase(Ease.OutBounce);
                }
            }
        }
    }

    private void FillCandies(bool addSpecial)
    {
        Vector2 centerPos = GetCenterPosition();
        int emptySpaceCount = GetEmptySpaceCount();

        int specialCandyIndex = Random.Range(0, emptySpaceCount);
        int currentCandyIndex = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (candyMap[x, y] == null)
                {
                    currentCandyIndex++;
                    Vector2 initPos = new Vector2(x - centerPos.x, y - centerPos.y + height);
                    Candy candy = null;
                    if (addSpecial && currentCandyIndex == specialCandyIndex)
                    {
                        candy = CreateSpecialCandy(x, y, initPos);
                    }
                    else
                    {
                        candy = CreateRandomCandy(x, y, initPos, false);
                    }

                    Vector2 pos = new Vector2(x - centerPos.x, y - centerPos.y);
                    candy.transform.DOMove(pos, newCandyMoveDuration);
                }
            }
        }
    }


    private int GetEmptySpaceCount()
    {
        int count = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (candyMap[x, y] == null)
                {
                    count++;
                }
            }
        }
        return count;
    }


    private void RemoveMatchedCandies(MatchChecker matchChecker)
    {
        //int specialCandyCnt = 0;
        foreach (Candy candy in matchChecker.candyList)
        {
            candyMap[candy.x, candy.y] = null;

            // if (candy.type == CandyType.Special1)
            // {
            //     specialCandyCnt++;
            // }
            // if (specialCandyCnt > 1)
            // {

            //     matchChecker.CheckSpecialMatches(candy, candy);
            //     specialCandyCnt = 0;

            // }

            if (candy.type == CandyType.Blue)
            {
                GameManager.instance.DecreaseBlue();
            }
            else if (candy.type == CandyType.Green)
            {
                GameManager.instance.DecreaseGreen();
            }
            else if (candy.type == CandyType.Purple)
            {
                GameManager.instance.DecreasePurple();
            }
            else if (candy.type == CandyType.Yellow)
            {
                GameManager.instance.DecreaseYellow();
            }
            else if (candy.type == CandyType.Red)
            {
                GameManager.instance.DecreaseRed();
            }

            candy.Remove();
        }
        matchChecker.candyList.Clear();
        matchChecker.containsSpecialCandy = false;
    }

}
