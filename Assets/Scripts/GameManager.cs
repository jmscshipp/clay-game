using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private Player player;
    private Computer computer;

    private int puzzleMoveSize = 4;
    private int moves;
    [SerializeField]
    private TMP_Text moveDisplayText;
    [SerializeField]
    private Animator canvasAnimator;
    private static GameManager instance;
    private bool succeeded = false;
    private bool failed = false;

    public static GameManager Instance()
    {
        return instance;
    }
    private void Awake()
    {
        // setting up singleton
        if (instance != null && instance != this)
            Destroy(this);
        instance = this;

        player = gameObject.GetComponent<Player>();
        computer = gameObject.GetComponent<Computer>();
    }

    private void Start()
    {
        StartRound();
    }

    private void Update()
    {
        if (Input.anyKeyDown && succeeded)
        {
            StartRound();
            succeeded = false;
        }
        else if (Input.anyKeyDown && failed)
        {
            Reset();
            failed = false;
        }
    }
    void StartRound()
    {
        moves = puzzleMoveSize;
        moveDisplayText.text = moves + "/" + puzzleMoveSize;
        computer.CreatePuzzle(puzzleMoveSize);
        player.Begin();
    }

    public void Reset()
    {
        moves = puzzleMoveSize;
        moveDisplayText.text = moves + "/" + puzzleMoveSize;
    }

    public void MakeMove()
    {
        moves--;
        moveDisplayText.text = moves + "/" + puzzleMoveSize;
        AudioManager.Instance().PlaySound("move");
        StartCoroutine(TextBumpAnim());
        StartCoroutine(CheckSolution());
    }

    private IEnumerator TextBumpAnim()
    {
        canvasAnimator.SetBool("bump", true);
        yield return new WaitForSeconds(0.1f);
        canvasAnimator.SetBool("bump", false);
    }

    private IEnumerator CheckSolution()
    {
        yield return new WaitForSeconds(1f);
        Block[,] computerBlocks = computer.GetBlocks();
        Block[,] playerBlocks = player.GetBlocks();

        for (int i = 0; i < 21; i ++)
        {
            for (int j = 0; j < 21; j++)
            {
                // computer and player don't match for this block
                if ((computerBlocks[i, j] != null && playerBlocks[i, j] == null)
                    || (computerBlocks[i, j] == null && playerBlocks[i, j] != null))
                {
                    if (moves == 0)
                        Fail();
                    yield break;
                }
            }
        }
        // made it through all blocks without a mismatch!
        Success();
    }

    private void Success()
    {
        player.StopPlayerInput();
        succeeded = true;
        AudioManager.Instance().PlaySound("success");
    }

    private void Fail()
    {
        player.StopPlayerInput();
        failed = true;
        AudioManager.Instance().PlaySound("fail");
    }
}
