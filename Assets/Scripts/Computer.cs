using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : MonoBehaviour
{
    enum MoveOption
    {
        None,
        Bullseye,
        Split,
        Proliferate,
        Grid
    }

    [SerializeField]
    private Progresssor progressor;
    private MoveOption lastMoveOption;
    private Block[,] blocks;
    private List<Block> recentBlocks;

    private void Awake()
    {
        // create empty block array
        blocks = new Block[21, 21];
        for (int i = 0; i < 21; i++)
            for (int j = 0; j < 21; j++)
                blocks[i, j] = null;

        recentBlocks = new List<Block>();
    }

    public void CreatePuzzle(int moves)
    {
        progressor.SetColorMode(false);
        // set up first block
        progressor.CreateBlock(blocks, recentBlocks, 10, 10);

        for (int i = 0; i < moves; i++)
        {
            Debug.Log(i);
            MoveOption move;
            if (i == 0)
                move = MakeMove(true);
            else
                move = MakeMove(false);
        
            if (move == MoveOption.Proliferate)
            {
                progressor.Proliferate(blocks, recentBlocks, 3, false);
            }
            if (move == MoveOption.Grid)
                progressor.GridSpawn(blocks, recentBlocks);
            if (move == MoveOption.Split)
                progressor.Split(blocks, recentBlocks);
            if (move == MoveOption.Bullseye)
                progressor.DeleteNPlace(blocks, recentBlocks);
        }
        progressor.ClearRecent(recentBlocks);
    }

    private MoveOption MakeMove(bool firstTurn)
    {
        MoveOption nextMoveOption;
        if (firstTurn)
        {
            // disclude bullseye
            nextMoveOption = (MoveOption)Mathf.Round(Random.Range(2f, 4f));
        }
        else
            nextMoveOption = (MoveOption)Mathf.Round(Random.Range(1f, 4f));

        if (nextMoveOption != MoveOption.Proliferate && nextMoveOption == lastMoveOption)
            return MakeMove(firstTurn);
        else
        {
            lastMoveOption = nextMoveOption;
            Debug.Log(lastMoveOption.ToString());
            return nextMoveOption;
        }
    }

    public Block[,] GetBlocks() => blocks;
}
