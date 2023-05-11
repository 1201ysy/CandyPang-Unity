using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchChecker : MonoBehaviour
{
    public List<Candy> candyList = new List<Candy>();
    private int[,] D = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };

    public bool containsSpecialCandy = false;
    public void CheckAllMatches()
    {
        candyList.Clear();

        Board board = FindObjectOfType<Board>();
        Candy[,] candyMap = board.candyMap;

        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height - 2; y++)
            {
                if (candyMap[x, y].type == candyMap[x, y + 1].type && candyMap[x, y].type == candyMap[x, y + 2].type)
                {
                    //candyMap[x, y].isMatched = true;
                    //candyMap[x, y + 1].isMatched = true;
                    //candyMap[x, y + 2].isMatched = true;

                    candyList.Add(candyMap[x, y]);
                    //candyList.Add(candyMap[x, y + 1]);
                    //candyList.Add(candyMap[x, y + 2]);
                    y += 2;
                }
            }
        }

        for (int y = 0; y < board.height; y++)
        {
            for (int x = 0; x < board.width - 2; x++)
            {
                if (candyMap[x, y].type == candyMap[x + 1, y].type && candyMap[x, y].type == candyMap[x + 2, y].type)
                {
                    //candyMap[x, y].isMatched = true;
                    //candyMap[x + 1, y].isMatched = true;
                    //candyMap[x + 2, y].isMatched = true;

                    candyList.Add(candyMap[x, y]);
                    //candyList.Add(candyMap[x + 1, y]);
                    //candyList.Add(candyMap[x + 2, y]);
                    x += 2;
                }
            }
        }

        if (candyList.Count > 0)
        {
            candyList = candyList.Distinct().ToList();
            CheckAllConnected();
            candyList = candyList.Distinct().ToList();
        }
    }


    private void CheckAllConnected()
    {
        Board board = FindObjectOfType<Board>();
        Candy[,] candyMap = board.candyMap;
        Queue<Candy> q = new Queue<Candy>();
        HashSet<Candy> visited = new HashSet<Candy>();


        foreach (Candy candy in candyList)
        {
            q.Enqueue(candy);
            visited.Add(candy);
        }

        bool specialCandyMatch = false;
        while (q.Count != 0)
        {
            Candy curr = q.Dequeue();

            curr.isMatched = true;
            candyList.Add(curr);

            if (curr.type == CandyType.Special1)
            {
                specialCandyMatch = true;
                break;
            }

            for (int i = 0; i < 4; i++)
            {
                int nx = curr.x + D[i, 0];
                int ny = curr.y + D[i, 1];

                if (nx < 0 || nx >= board.width || ny < 0 || ny >= board.height)
                {
                    continue;
                }

                if (curr.type != candyMap[nx, ny].type)
                {
                    continue;
                }

                if (visited.Contains(candyMap[nx, ny]))
                {
                    continue;
                }

                visited.Add(candyMap[nx, ny]);
                q.Enqueue(candyMap[nx, ny]);
            }

        }
        if (specialCandyMatch)
        {

            for (int x = 0; x < board.width; x++)
            {
                for (int y = 0; y < board.height; y++)
                {

                    candyMap[x, y].isMatched = true;
                    candyList.Add(candyMap[x, y]);

                }
            }
        }

    }

    public void CheckSpecialMatches(Candy firstCandy, Candy secondCandy)
    {
        if (firstCandy.type != CandyType.Special1 && secondCandy.type != CandyType.Special1)
        {
            return;
        }

        Board board = FindObjectOfType<Board>();
        Candy[,] candyMap = board.candyMap;
        CandyType targetType = CandyType.Special1;
        bool matchAll = false;

        if (firstCandy.type == CandyType.Special1 && secondCandy.type != CandyType.Special1)
        {
            targetType = secondCandy.type;
            firstCandy.isMatched = true;
            candyList.Add(firstCandy);
        }
        else if (firstCandy.type != CandyType.Special1 && secondCandy.type == CandyType.Special1)
        {
            targetType = firstCandy.type;
            secondCandy.isMatched = true;
            candyList.Add(secondCandy);
        }
        else
        {
            // both special
            matchAll = true;
        }

        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height; y++)
            {
                if (candyMap[x, y].type == targetType || matchAll)
                {
                    candyMap[x, y].isMatched = true;
                    candyList.Add(candyMap[x, y]);
                }
            }
        }

        if (candyList.Count > 0)
        {
            candyList = candyList.Distinct().ToList();
        }

        containsSpecialCandy = true;
    }
}
