using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Player player;
    private Computer computer;

    private int puzzleMoveSize = 1;
    private void Awake()
    {
        player = gameObject.GetComponent<Player>();
        computer = gameObject.GetComponent<Computer>();
    }

    private void Start()
    {
        StartRound();
    }

    void StartRound()
    {
        computer.CreatePuzzle(puzzleMoveSize);
        player.Begin();
    }
}
