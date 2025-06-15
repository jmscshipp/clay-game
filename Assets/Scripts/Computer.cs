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

    private MoveOption lastMoveOption;

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreatePuzzle(int moves)
    {
        for (int i = 0; i < moves; i++)
        {
            MakeMove
        }
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
            return nextMoveOption;
        }
    }

    private void CheckSolution()
    {

    }
}
