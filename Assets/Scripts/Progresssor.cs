using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Progresssor : MonoBehaviour
{
    private float distanceIncrement = 36; // also the size of each block
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
        blocks = new Block[20, 20];
        for (int i = 0; i < 20; i++)
            for (int j = 0; j < 20; j++)
                blocks[i, j] = null;

        recentBlocks = new List<Block>();

        // preliminary color setup
        goalColorIndex = 1;
        currentColor = colorOptions[0];
        goalColor = colorOptions[goalColorIndex];

        // set up first block
        blocks[10, 10] = CreateBlock(10, 10);
        recentBlocks.Add(blocks[10, 10]);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            ShiftUp();
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Proliferate(3);
            Proliferate(1);
            PickNextColor();
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

    private Block CreateBlock(int row, int col)
    {
        GameObject blockObj = Instantiate(blockPrefab, shifter);
        blockObj.transform.localPosition = new Vector3(row * 36f, col * 36f);
        Block newBlock = blockObj.GetComponent<Block>();
        if (blocks[row, col] != null)
            RemoveBlock(row, col);

        blocks[row, col] = newBlock;
        newBlock.AssignIndexes(row, col);
        newBlock.AssignColor(currentColor);
        return newBlock;
    }

    private void RemoveBlock(int row, int col)
    {
        Block block = blocks[row, col];
        blocks[row, col] = null;
        if (recentBlocks.Contains(block))
            recentBlocks.Remove(block);
        Destroy(block.gameObject);
    }

    // UP ARROW ABILITY
    // moves all blocks up one space
    private void ShiftUp()
    {
        // delete top row of blocks
        for (int i = 0; i < 20; i++)
        {
            if (blocks[i, 19] != null)
            {
                RemoveBlock(i, 19);
                blocks[i, 19] = null;
            }
        }

        // move each row up 1
        for (int i = 19; i > 0; i--)
        {
            for (int j = 0; j < 20; j++)
            {
                if (blocks[j, i - 1] != null)
                {
                    blocks[j, i] = blocks[j, i - 1];
                    blocks[j, i].AssignIndexes(i, j + 1);
                    blocks[j, i].transform.localPosition += new Vector3(0f, distanceIncrement);
                    blocks[j, i - 1] = null;
                }
            }
        }
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
                ProliferateHelper(newBlock, -1, 0, generations);
            // right
            if (newBlock.Row() < 19 && blocks[newBlock.Row() + 1, newBlock.Col()] == null)
                ProliferateHelper(newBlock, 1, 0, generations);
            // up
            if (newBlock.Col() < 19 && blocks[newBlock.Row(), newBlock.Col() + 1] == null)
                ProliferateHelper(newBlock, 0, 1, generations);
            // down
            if (newBlock.Col() > 0 && blocks[newBlock.Row(), newBlock.Col() - 1] == null)
                ProliferateHelper(newBlock, 0, -1, generations);
        }
    }

    private void ProliferateHelper(Block baseBlock, int rowModifier, int colModifier, int generationCounter)
    {
        if (generationCounter == 0)
            return;

        // boundary checks based on direction
        if (rowModifier > 0 && baseBlock.Row() == 19)
            return;
        if (rowModifier < 0 && baseBlock.Row() == 0)
            return;
        if (colModifier > 0 && baseBlock.Col() == 19)
            return;
        if (colModifier < 0 && baseBlock.Col() == 0)
            return;

        Block newBlock = CreateBlock(baseBlock.Row() + rowModifier, baseBlock.Col() + colModifier);
        newBlock.AssignColor(Color.Lerp(currentColor, goalColor, generationCounter / 3f));

        // last block in this chain
        if (generationCounter == 1)
            recentBlocks.Add(newBlock);

        ProliferateHelper(newBlock, rowModifier, colModifier, --generationCounter);
    }
}
