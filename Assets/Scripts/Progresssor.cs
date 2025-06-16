using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Progresssor : MonoBehaviour
{
    private float distanceIncrement = 30.95f; // also the size of each block
    [SerializeField]
    private GameObject blockPrefab;

    [SerializeField]
    private RectTransform shifter;

    private float colorLerp;
    private Color currentColor; // current increment towards goal color
    private Color goalColor; // current increment towards goal color
    private int goalColorIndex; // goal color 

    private Color[] colorOptions = new Color[3];

    [SerializeField]
    private Color[] playerColorOptions = new Color[3];

    [SerializeField]
    private Color[] computerColorOptions = new Color[3];


    public void SetColorMode(bool playerMode)
    {
        if (playerMode)
        {
            colorOptions = playerColorOptions;
            currentColor = colorOptions[0];
            goalColorIndex = 1;
            goalColor = colorOptions[goalColorIndex];
        }
        else
        {
            colorOptions = computerColorOptions;
            currentColor = colorOptions[0];
            goalColorIndex = 1;
            goalColor = colorOptions[goalColorIndex];
        }
        ProgressColorGradient();
    }

    private void ProgressColorGradient()
    {
        if (currentColor == goalColor)
        {
            colorLerp = 0f;
            PickNextColor();
            ProgressColorGradient();
        }
        colorLerp += 0.2f;
        currentColor = Color.Lerp(currentColor, goalColor, colorLerp);
    }

    private void PickNextColor()
    {
        currentColor = colorOptions[goalColorIndex];
        goalColorIndex++;
        if (goalColorIndex > colorOptions.Length - 1)
            goalColorIndex = 0;
        goalColor = colorOptions[goalColorIndex];
    }

    public Block CreateBlock(Block[,] blocks, List<Block> recentBlocks, int row, int col, bool includeInRecent = true)
    {
        GameObject blockObj = Instantiate(blockPrefab, shifter);
        blockObj.transform.localPosition = new Vector3(row * distanceIncrement, col * distanceIncrement);
        Block newBlock = blockObj.GetComponent<Block>();
        if (blocks[row, col] != null)
            RemoveBlock(blocks, recentBlocks, row, col);

        blocks[row, col] = newBlock;
        newBlock.AssignIndexes(row, col);
        newBlock.AssignColor(currentColor);
        if (includeInRecent)
            recentBlocks.Add(newBlock);
        return newBlock;
    }

    public void RemoveBlock(Block[,] blocks, List<Block> recentBlocks, int row, int col)
    {
        if (blocks[row, col] == null)
            return;
        Block block = blocks[row, col];
        blocks[row, col] = null;
        if (recentBlocks.Contains(block))
            recentBlocks.Remove(block);
        Destroy(block.gameObject);
    }

    public void ClearRecent(List<Block> recentBlocks)
    {
        // returns blocks to real color instead of black
        foreach (Block block in recentBlocks)
            block.ClearFromRecent();
        recentBlocks.Clear();
    }

    // UP ARROW ABILITY
    // moves all blocks up one space
    public void Split(Block[,] blocks, List<Block> recentBlocks)
    {
        // delete side columns of blocks
        for (int i = 0; i < 21; i++)
        {
            // left side
            for (int j = 0; j < 3; j++)
            {
                if (blocks[j, i] != null)
                    RemoveBlock(blocks, recentBlocks, j, i);
            }

            // right side
            for (int j = 20; j > 17; j--)
            {
                if (blocks[j, i] != null)
                    RemoveBlock(blocks, recentBlocks, j, i);
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
            RemoveBlock(blocks, recentBlocks, 10, i);

        ClearRecent(recentBlocks);

        // first row
        CreateBlock(blocks, recentBlocks, 6, 1);
        CreateBlock(blocks, recentBlocks, 6, 9);
        CreateBlock(blocks, recentBlocks, 6, 17);

        // second row
        CreateBlock(blocks, recentBlocks, 13, 3);
        CreateBlock(blocks, recentBlocks, 13, 11);
        CreateBlock(blocks, recentBlocks, 13, 19);

        ProgressColorGradient();
    }

    // RIGHT ARROW ABILITY
    // grow blocks out from most recent blocks
    public void Proliferate(Block[,] blocks, List<Block> recentBlocks, int generations, bool player)
    {
        List<Block> newBlocks = new List<Block>(recentBlocks);
        ClearRecent(recentBlocks);
        if (player)
        {
            foreach (Block newBlock in newBlocks)
            {
                // left
                if (newBlock.Row() > 0 && blocks[newBlock.Row() - 1, newBlock.Col()] == null)
                    StartCoroutine(ProliferateHelper(blocks, recentBlocks, newBlock, -1, 0, generations, generations, 0.1f));
                // right
                if (newBlock.Row() < 20 && blocks[newBlock.Row() + 1, newBlock.Col()] == null)
                    StartCoroutine(ProliferateHelper(blocks, recentBlocks, newBlock, 1, 0, generations, generations, 0.1f));
                // up
                if (newBlock.Col() < 20 && blocks[newBlock.Row(), newBlock.Col() + 1] == null)
                    StartCoroutine(ProliferateHelper(blocks, recentBlocks, newBlock, 0, 1, generations, generations, 0.1f));
                // down
                if (newBlock.Col() > 0 && blocks[newBlock.Row(), newBlock.Col() - 1] == null)
                    StartCoroutine(ProliferateHelper(blocks, recentBlocks, newBlock, 0, -1, generations, generations, 0.1f));
            }
        }
        else
        {
            foreach (Block newBlock in newBlocks)
            {
                // left
                if (newBlock.Row() > 0 && blocks[newBlock.Row() - 1, newBlock.Col()] == null)
                    ProliferateHelperReg(blocks, recentBlocks, newBlock, -1, 0, generations, generations);
                // right
                if (newBlock.Row() < 20 && blocks[newBlock.Row() + 1, newBlock.Col()] == null)
                    ProliferateHelperReg(blocks, recentBlocks, newBlock, 1, 0, generations, generations);
                // up
                if (newBlock.Col() < 20 && blocks[newBlock.Row(), newBlock.Col() + 1] == null)
                    ProliferateHelperReg(blocks, recentBlocks, newBlock, 0, 1, generations, generations);
                // down
                if (newBlock.Col() > 0 && blocks[newBlock.Row(), newBlock.Col() - 1] == null)
                    ProliferateHelperReg(blocks, recentBlocks, newBlock, 0, -1, generations, generations);
            }
        }
        //nextColor = new Color(nextColor.r + 0.1f, nextColor.g, nextColor.b);

        ProgressColorGradient();
    }

    private IEnumerator ProliferateHelper(Block[,] blocks, List<Block> recentBlocks, Block baseBlock, int rowModifier, int colModifier, float generationCounter, float totalGenerations, float delay)
    {
        if (generationCounter == 0)
            yield break;

        // boundary checks based on direction
        if (rowModifier > 0 && baseBlock.Row() == 20)
            yield break;
        if (rowModifier < 0 && baseBlock.Row() == 0)
            yield break;
        if (colModifier > 0 && baseBlock.Col() == 20)
            yield break;
        if (colModifier < 0 && baseBlock.Col() == 0)
            yield break;

        Block newBlock = CreateBlock(blocks, recentBlocks, baseBlock.Row() + rowModifier, baseBlock.Col() + colModifier, false);
        //newBlock.AssignColor(Color.Lerp(currentColor, goalColor, generationCounter / totalGenerations));
        newBlock.AssignColor(currentColor);
        newBlock.ClearFromRecent();

        // last block in this chain
        if (generationCounter == 1)
        {
            //newBlock.AssignColor(Color.Lerp(currentColor, goalColor, generationCounter / totalGenerations));
            newBlock.AssignColor(currentColor);
            recentBlocks.Add(newBlock);
        }

        yield return new WaitForSeconds(delay);
        StartCoroutine(ProliferateHelper(blocks, recentBlocks, newBlock, rowModifier, colModifier, --generationCounter, totalGenerations, delay));
    }

    // non-coroutine version without delays, for computer use
    private void ProliferateHelperReg(Block[,] blocks, List<Block> recentBlocks, Block baseBlock, int rowModifier, int colModifier, float generationCounter, float totalGenerations)
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

        Block newBlock = CreateBlock(blocks, recentBlocks, baseBlock.Row() + rowModifier, baseBlock.Col() + colModifier, false);
        //newBlock.AssignColor(Color.Lerp(currentColor, goalColor, generationCounter / totalGenerations));
        newBlock.AssignColor(currentColor);
        newBlock.ClearFromRecent();

        // last block in this chain
        if (generationCounter == 1)
        {
            //newBlock.AssignColor(Color.Lerp(currentColor, goalColor, generationCounter / totalGenerations));
            newBlock.AssignColor(currentColor);
            recentBlocks.Add(newBlock);
        }

        ProliferateHelperReg(blocks, recentBlocks, newBlock, rowModifier, colModifier, --generationCounter, totalGenerations);
    }
    public void GridSpawn(Block[,] blocks, List<Block> recentBlocks)
    {
        ClearRecent(recentBlocks);

        // first row
        CreateBlock(blocks, recentBlocks, 4, 4);
        CreateBlock(blocks, recentBlocks, 10, 4);
        CreateBlock(blocks, recentBlocks, 16, 4);

        // second row
        CreateBlock(blocks, recentBlocks, 4, 10);
        CreateBlock(blocks, recentBlocks, 10, 10);
        CreateBlock(blocks, recentBlocks, 16, 10);

        // third row
        CreateBlock(blocks, recentBlocks, 4, 16);
        CreateBlock(blocks, recentBlocks, 10, 16);
        CreateBlock(blocks, recentBlocks, 16, 16);

        ProgressColorGradient();
    }

    public void DeleteNPlace(Block[,] blocks, List<Block> recentBlocks)
    {
        ClearRecent(recentBlocks);

        for (int i = 7; i < 14; i++)
        {
            for (int j = 7; j < 14; j++)
                RemoveBlock(blocks, recentBlocks, i, j);
        }
        CreateBlock(blocks, recentBlocks, 10, 10);

        ProgressColorGradient();
    }

    public void ResetBlocks(Block[,] blocks, List<Block> recentBlocks)
    {
        for (int i = 0; i < 21; i++)
        {
            for (int j = 0; j < 21; j++)
                RemoveBlock(blocks, recentBlocks, i, j);
        }
    }
}
