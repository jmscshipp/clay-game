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
    }

    private IEnumerator TextBumpAnim()
    {
        canvasAnimator.SetBool("bump", true);
        yield return new WaitForSeconds(0.1f);
        canvasAnimator.SetBool("bump", false);
    }
}
