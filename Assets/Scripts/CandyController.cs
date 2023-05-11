using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CandyController : MonoBehaviour
{

    private Board board;

    private MatchChecker matchChecker;

    private Vector2 mouseDownPos;
    private Vector2 mouseUpPos;

    private Candy firstCandy;

    private Vector2 firstCandyPos;
    private Candy secondCandy;

    private Vector2 secondCandyPos;

    private float moveDuration = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        matchChecker = FindObjectOfType<MatchChecker>();

    }

    // Update is called once per frame
    void Update()
    {

        if (GameManager.instance.canMoveCandy == false)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            mouseDownPos = Input.mousePosition;
            //Debug.Log("Mouse down pos : " + mouseDownPos);
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(mouseDownPos);
            //Debug.Log("Mouse pos : " + mouseDownPos);

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null)
            {
                firstCandy = hit.collider.GetComponent<Candy>();
                firstCandyPos = firstCandy.transform.position;
                // if (firstCandy != null)
                // {
                //     Debug.Log("firstCandy pos : " + firstCandy.x + "," + firstCandy.y);
                // }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            mouseUpPos = Input.mousePosition;
            //Debug.Log("Mouse up pos : " + mouseDownPos);

            float angle = Mathf.Atan2(mouseUpPos.y - mouseDownPos.y, mouseUpPos.x - mouseDownPos.x) * Mathf.Rad2Deg;
            // Debug.Log("Angle : " + angle);
            // -45(315) ~ 45 : Right
            // 45 ~ 135 : Up
            // 135 ~ -135(270) : Left
            // -135(270) ~ -45(315) : Down
            secondCandy = GetSecondCandy(angle);

            if (firstCandy != null && secondCandy != null)
            {
                GameManager.instance.canMoveCandy = false;
                GameManager.instance.DecreaseMove();
                secondCandyPos = secondCandy.transform.position;


                firstCandy.transform.DOMove(secondCandyPos, moveDuration * 0.8f);
                secondCandy.transform.DOMove(firstCandyPos, moveDuration * 0.8f);
                SwapCandies();
                StartCoroutine("CheckMatchRoutine");
            }
            else
            {
                ResetCandyInfo();
            }

        }



    }

    private Candy GetSecondCandy(float angle)
    {
        if (firstCandy == null)
        {
            return null;
        }

        Candy candy = null;
        if (angle > -45 && angle <= 45 && firstCandy.x + 1 < board.width)
        {
            //Right
            candy = board.candyMap[firstCandy.x + 1, firstCandy.y];

        }
        else if (angle > 45 && angle <= 135 && firstCandy.y + 1 < board.height)
        {
            //Up
            candy = board.candyMap[firstCandy.x, firstCandy.y + 1];
        }
        else if ((angle > 135 || angle <= -135) && firstCandy.x - 1 >= 0)
        {
            //Left
            candy = board.candyMap[firstCandy.x - 1, firstCandy.y];
        }
        else if (angle > -135 && angle <= -45 && firstCandy.y - 1 >= 0)
        {
            //Down
            candy = board.candyMap[firstCandy.x, firstCandy.y - 1];
        }

        return candy;
    }


    IEnumerator CheckMatchRoutine()
    {
        yield return new WaitForSeconds(moveDuration * 2f);
        matchChecker.CheckAllMatches();

        matchChecker.CheckSpecialMatches(firstCandy, secondCandy);

        if (firstCandy.isMatched == false && secondCandy.isMatched == false)
        {
            // no matches; undo move
            firstCandy.transform.DOMove(firstCandyPos, moveDuration);
            secondCandy.transform.DOMove(secondCandyPos, moveDuration);
            SwapCandies();

            yield return new WaitForSeconds(moveDuration);
            GameManager.instance.canMoveCandy = true;
            GameManager.instance.CheckGameOver();
        }
        else
        {
            // Remove matched
            board.StartRemoveCandiesRoutine();
        }

        ResetCandyInfo();
    }

    private void SwapCandies()
    {
        // firstCandy.transform.position = secondCandyPos;
        // secondCandy.transform.position = firstCandyPos;

        int tmpX = firstCandy.x;
        int tmpY = firstCandy.y;

        firstCandy.x = secondCandy.x;
        firstCandy.y = secondCandy.y;

        secondCandy.x = tmpX;
        secondCandy.y = tmpY;

        board.candyMap[firstCandy.x, firstCandy.y] = firstCandy;
        board.candyMap[secondCandy.x, secondCandy.y] = secondCandy;

    }



    private void ResetCandyInfo()
    {
        firstCandy = null;
        firstCandyPos = Vector2.zero;
        secondCandy = null;
        secondCandyPos = Vector2.zero;
    }
}
