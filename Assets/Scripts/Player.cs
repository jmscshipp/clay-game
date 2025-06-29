using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Block[,] blocks;
    private List<Block> recentBlocks;
    [SerializeField]
    private Progresssor progressor;
    private bool allowInput = false;

    // Start is called before the first frame update
    void Awake()
    {
        // create empty block array
        blocks = new Block[21, 21];
        for (int i = 0; i < 21; i++)
            for (int j = 0; j < 21; j++)
                blocks[i, j] = null;

        recentBlocks = new List<Block>();
    }

    public void Begin()
    {
        // set up first block
        progressor.CreateBlock(blocks, recentBlocks, 10, 10);
        progressor.SetColorMode(true);
        allowInput = true;
    }

    public void Reset()
    {
        GameManager.Instance().Reset();
        progressor.ResetBlocks(blocks, recentBlocks);
        Begin();
    }

    // Update is called once per frame
    void Update()
    {
        if (!allowInput)
            return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            progressor.Split(blocks, recentBlocks);
            GameManager.Instance().MakeMove();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            progressor.Grow(blocks, recentBlocks, 3, true);
            WaitForMove();
            GameManager.Instance().MakeMove();
            //progressor.PickNextColor();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            progressor.Grid(blocks, recentBlocks);
            GameManager.Instance().MakeMove();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            progressor.Bullseye(blocks, recentBlocks);
            GameManager.Instance().MakeMove();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }
    }

    public void AllowPlayerInput(bool allow = false)
    {
        allowInput = allow;
    }

    public IEnumerator WaitForMove()
    {
        allowInput = false;
        yield return new WaitForSeconds(0.3f);
        allowInput = true;
    }

    public Block[,] GetBlocks() => blocks;
}
