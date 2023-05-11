using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum CandyType
{
    Blue,
    Green,
    Purple,
    Red,
    Yellow,

    Special1
}

public class Candy : MonoBehaviour
{
    public CandyType type;
    public int x;
    public int y;

    public bool isMatched;

    private int blinkCount = 2;

    private float blinkDuration = 0.2f;

    public void Init(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.isMatched = false;
    }


    public void Remove()
    {
        StartCoroutine("BlinkRoutine");
        Destroy(gameObject, blinkCount * blinkDuration * 2f);
    }

    IEnumerator BlinkRoutine()
    {
        Renderer renderer = GetComponent<Renderer>();

        for (int i = 0; i < blinkCount; i++)
        {
            renderer.enabled = false;
            yield return new WaitForSeconds(blinkDuration);
            renderer.enabled = true;
            yield return new WaitForSeconds(blinkDuration);
        }

    }
}
