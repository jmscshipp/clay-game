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
    private List<MoveOption> lastPuzzle;
    private void Awake()
    {
        // create empty block array
        blocks = new Block[21, 21];
        for (int i = 0; i < 21; i++)
            for (int j = 0; j < 21; j++)
                blocks[i, j] = null;

        recentBlocks = new List<Block>();
        lastPuzzle = new List<MoveOption>();
    }

    public void CreatePuzzle(int moves)
    {
        lastMoveOption = MoveOption.None;
        progressor.SetColorMode(false);
        progressor.ResetBlocks(blocks, recentBlocks);
        // set up first block
        progressor.CreateBlock(blocks, recentBlocks, 10, 10);

        List<MoveOption> newPuzzle = new List<MoveOption>();
        for (int i = 0; i < moves; i++)
        {
            MoveOption move;
            if (i == 0)
                move = MakeMove(true, moves);
            else
                move = MakeMove(false, moves);

            if (move == MoveOption.Proliferate)
                progressor.Grow(blocks, recentBlocks, 3, false);
            else if (move == MoveOption.Grid)
                progressor.Grid(blocks, recentBlocks);
            else if (move == MoveOption.Split)
                progressor.Split(blocks, recentBlocks);
            else if (move == MoveOption.Bullseye)
                progressor.Bullseye(blocks, recentBlocks);
            newPuzzle.Add(move);
        }
        Debug.Log("     lastPuzzle");
        string puzzlestr = "";
        foreach (MoveOption move in lastPuzzle)
        {
            puzzlestr = puzzlestr + ", " + move.ToString();
        }
        Debug.Log(puzzlestr);
        puzzlestr = "";
        Debug.Log("     newPuzzle");
        foreach (MoveOption move in newPuzzle)
        {
            puzzlestr = puzzlestr + ", " + move.ToString();
        }
        Debug.Log(puzzlestr);

        if (lastPuzzle.Count != newPuzzle.Count)
        {
            lastPuzzle = newPuzzle;
            progressor.ClearRecent(recentBlocks);
            return;
        }

        // if we just did this exact puzzle, make a different one
        for (int i = 0; i < lastPuzzle.Count; i++)
        {
            if (lastPuzzle[i] != newPuzzle[i])
            {
                lastPuzzle = newPuzzle;
                progressor.ClearRecent(recentBlocks);
                return;
            }
        }

        // if moves from last puzzle are the same as this one, remake it
        CreatePuzzle(moves);
    }

    private MoveOption MakeMove(bool firstTurn, int moveNum)
    {
        MoveOption nextMoveOption;
        if (firstTurn)
        {
            // disclude bullseye
            nextMoveOption = (MoveOption)Mathf.Round(Random.Range(2f, 4f));
        }
        else
            nextMoveOption = (MoveOption)Mathf.Round(Random.Range(1f, 4f));

        // prevent second move from being bullseye in 2 move puzzles
        if (moveNum == 2 && nextMoveOption == MoveOption.Bullseye && lastMoveOption != MoveOption.None)
            return MakeMove(firstTurn, moveNum);
        if (nextMoveOption != MoveOption.Proliferate && nextMoveOption == lastMoveOption)
            return MakeMove(firstTurn, moveNum);
        else
        {
            lastMoveOption = nextMoveOption;
            return nextMoveOption;
        }
    }

    public Block[,] GetBlocks() => blocks;
}
