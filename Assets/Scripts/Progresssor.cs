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

    private float colorLerp;
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
        ProgressColorGradient();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            Split();
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Proliferate(3, 0.1f);
            Proliferate(1, 0.1f);
            PickNextColor();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            GridSpawn();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            DeleteNPlace();
        }
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

    private void ClearRecent()
    {
        // returns blocks to real color instead of black
        foreach (Block block in recentBlocks)
            block.ClearFromRecent();
        recentBlocks.Clear();
    }

    // UP ARROW ABILITY
    // moves all blocks up one space
    private void Split()
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

        ClearRecent();

        // first row
        CreateBlock(6, 1);
        CreateBlock(6, 9);
        CreateBlock(6, 17);

        // second row
        CreateBlock(13, 3);
        CreateBlock(13, 11);
        CreateBlock(13, 19);

        ProgressColorGradient();
    }

    // RIGHT ARROW ABILITY
    // grow blocks out from most recent blocks
    private void Proliferate(int generations, float delay = 0f)
    {
        List<Block> newBlocks = new List<Block>(recentBlocks);
        ClearRecent();
        //nextColor = new Color(nextColor.r + 0.1f, nextColor.g, nextColor.b);
        foreach (Block newBlock in newBlocks)
        {
            // left
            if (newBlock.Row() > 0 && blocks[newBlock.Row() - 1, newBlock.Col()] == null)
                StartCoroutine(ProliferateHelper(newBlock, -1, 0, generations, generations, delay));
            // right
            if (newBlock.Row() < 20 && blocks[newBlock.Row() + 1, newBlock.Col()] == null)
                StartCoroutine(ProliferateHelper(newBlock, 1, 0, generations, generations, delay));
            // up
            if (newBlock.Col() < 20 && blocks[newBlock.Row(), newBlock.Col() + 1] == null)
                StartCoroutine(ProliferateHelper(newBlock, 0, 1, generations, generations, delay));
            // down
            if (newBlock.Col() > 0 && blocks[newBlock.Row(), newBlock.Col() - 1] == null)
                StartCoroutine(ProliferateHelper(newBlock, 0, -1, generations, generations, delay));
        }
        ProgressColorGradient();
    }

    private IEnumerator ProliferateHelper(Block baseBlock, int rowModifier, int colModifier, float generationCounter, float totalGenerations, float delay)
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

        Block newBlock = CreateBlock(baseBlock.Row() + rowModifier, baseBlock.Col() + colModifier, false);
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
        StartCoroutine(ProliferateHelper(newBlock, rowModifier, colModifier, --generationCounter, totalGenerations, delay));
    }

    private void GridSpawn()
    {
        ClearRecent();

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

        ProgressColorGradient();
    }

    private void DeleteNPlace()
    {
        ClearRecent();

        for (int i = 7; i < 14; i++)
        {
            for (int j = 7; j < 14; j++)
                RemoveBlock(i, j);
        }
        CreateBlock(10, 10);

        ProgressColorGradient();
    }
}
