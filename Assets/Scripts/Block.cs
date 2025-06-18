using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    private int row, col;
    private Image graphics;
    private Color color;

    private void Awake()
    {
        graphics = GetComponent<Image>();
    }

    public void AssignIndexes(int row, int col)
    {
        this.row = row;
        this.col = col;
    }

    public void AssignColor(Color color)
    {
        this.color = color;
        graphics.color = Color.black;
    }

    public void ClearFromRecent()
    {
        graphics.color = this.color;
    }

    public void Dissapear()
    {
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        for (int i = 0; i < 2; i++)
        {
            graphics.color = new Color(graphics.color.r, graphics.color.g, graphics.color.b, graphics.color.a - 0.34f);
            yield return new WaitForSeconds(0.1f);
        }
        Destroy(this.gameObject);
    }

    public int Row() => row;
    public int Col() => col;
}
