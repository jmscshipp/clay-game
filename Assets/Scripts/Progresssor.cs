using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Progresssor : MonoBehaviour
{
    private float distanceIncrement = 34.286f; // also the size of each block
    [SerializeField]
    private GameObject blockPrefab;
    private Block[,] blocks;
    [SerializeField]
    private RectTransform shifter;

    private List<Block> recentBlocks;

    private Color currentColor; // current increment towards goal color
    private Color goalColor; // current increment towards goal color
    private int goalColorIndex; // goal color 

    [SerializeField]
    private Color[] colorOptions = new Color[3];

    // Start is called before the first frame update
    void Start()
    {
        // create empty block array
        blocks = new Block[21, 21];
        for (int i = 0; i < 21; i++)
            for (int j = 0; j < 21; j++)
                blocks[i, j] = null;

        recentBlocks = new List<Block>();

        // preliminary color setup
        goalColorIndex = 1;
        currentColor = colorOptions[0];
        goalColor = colorOptions[goalColorIndex];

        // set up first block
        CreateBlock(10, 10);
        recentBlocks.Add(blocks[10, 10]);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            Shift();
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Proliferate(3);
            Proliferate(1);
            PickNextColor();
        }
        if (Input.GetKeyDown (KeyCode.UpArrow))
        {
            GridSpawn();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            DeleteNPlace();
        }
    }

    private void PickNextColor()
    {
        currentColor = colorOptions[goalColorIndex];
        goalColorIndex++;
        if (goalColorIndex > colorOptions.Length - 1)
            goalColorIndex = 0;
        goalColor = colorOptions[goalColorIndex];
    }

    private Block CreateBlock(int row, int col, bool includeInRecent = true)
    {
        GameObject blockObj = Instantiate(blockPrefab, shifter);
        blockObj.transform.localPosition = new Vector3(row * distanceIncrement, col * distanceIncrement);
        Block newBlock = blockObj.GetComponent<Block>();
        if (blocks[row, col] != null)
            RemoveBlock(row, col);

        blocks[row, col] = newBlock;
        newBlock.AssignIndexes(row, col);
        newBlock.AssignColor(currentColor);
        if (includeInRecent)
            recentBlocks.Add(newBlock);
        return newBlock;
    }

    private void RemoveBlock(int row, int col)
    {
        if (blocks[row, col] == null)
            return;
        Block block = blocks[row, col];
        blocks[row, col] = null;
        if (recentBlocks.Contains(block))
            recentBlocks.Remove(block);
        Destroy(block.gameObject);
    }

    // UP ARROW ABILITY
    // moves all blocks up one space
    private void Shift()
    {
        // delete side columns of blocks
        for (int i = 0; i < 21; i++)
        {
            // left side
            for (int j = 0; j < 3; j++)
            {
                if (blocks[j, i] != null)
                    RemoveBlock(j, i);
            }

            // right side
            for (int j = 20; j > 17; j--)
            {
                if (blocks[j, i] != null)
                    RemoveBlock(j, i);
            }
        }

        // move each column left 3
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 21; j++)
            {
                if (blocks[i + 3, j] != null)
                {
                    blocks[i, j] = blocks[i + 3, j];
                    blocks[i, j].AssignIndexes(i, j);
                    blocks[i, j].transform.localPosition += new Vector3(-distanceIncrement * 3, 0f);
                    blocks[i + 3, j] = null;
                }
            }
        }

        // move each column right 3
        for (int i = 20; i > 13; i--)
        {
            for (int j = 0; j < 21; j++)
            {
                if (blocks[i - 3, j] != null)
                {
                    blocks[i, j] = blocks[i - 3, j];
                    blocks[i, j].AssignIndexes(i, j);
                    blocks[i, j].transform.localPosition += new Vector3(distanceIncrement * 3, 0f);
                    blocks[i - 3, j] = null;
                }
            }
        }

        // delete center row
        for (int i = 0; i < 21; i++)
            RemoveBlock(10, i);
    }

    // RIGHT ARROW ABILITY
    // grow blocks out from most recent blocks
    private void Proliferate(int generations)
    {
        List<Block> newBlocks = new List<Block>(recentBlocks);
        recentBlocks.Clear();
        //nextColor = new Color(nextColor.r + 0.1f, nextColor.g, nextColor.b);
        foreach (Block newBlock in newBlocks)
        {
            // left
            if (newBlock.Row() > 0 && blocks[newBlock.Row() - 1, newBlock.Col()] == null)
                ProliferateHelper(newBlock, -1, 0, generations, generations);
            // right
            if (newBlock.Row() < 20 && blocks[newBlock.Row() + 1, newBlock.Col()] == null)
                ProliferateHelper(newBlock, 1, 0, generations, generations);
            // up
            if (newBlock.Col() < 20 && blocks[newBlock.Row(), newBlock.Col() + 1] == null)
                ProliferateHelper(newBlock, 0, 1, generations, generations);
            // down
            if (newBlock.Col() > 0 && blocks[newBlock.Row(), newBlock.Col() - 1] == null)
                ProliferateHelper(newBlock, 0, -1, generations, generations);
        }
    }

    private void ProliferateHelper(Block baseBlock, int rowModifier, int colModifier, float generationCounter, float totalGenerations)
    {
        if (generationCounter == 0)
            return;

        // boundary checks based on direction
        if (rowModifier > 0 && baseBlock.Row() == 20)
            return;
        if (rowModifier < 0 && baseBlock.Row() == 0)
            return;
        if (colModifier > 0 && baseBlock.Col() == 20)
            return;
        if (colModifier < 0 && baseBlock.Col() == 0)
            return;

        Block newBlock = CreateBlock(baseBlock.Row() + rowModifier, baseBlock.Col() + colModifier, false);
        newBlock.AssignColor(Color.Lerp(currentColor, goalColor, generationCounter / totalGenerations));

        // last block in this chain
        if (generationCounter == 1)
            recentBlocks.Add(newBlock);

        ProliferateHelper(newBlock, rowModifier, colModifier, --generationCounter, totalGenerations);
    }

    private void GridSpawn()
    {
        recentBlocks.Clear();

        // first row
        CreateBlock(4, 4);
        CreateBlock(10, 4);
        CreateBlock(16, 4);

        // second row
        CreateBlock(4, 10);
        CreateBlock(10, 10);
        CreateBlock(16, 10);

        // third row
        CreateBlock(4, 16);
        CreateBlock(10, 16);
        CreateBlock(16, 16);
    }

    private void DeleteNPlace()
    {
        recentBlocks.Clear();

        for (int i = 7; i < 14; i++)
        {
            for (int j = 7; j < 14; j++)
                RemoveBlock(i, j);
        }
        CreateBlock(10, 10);
    }
}
