using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    protected Player player;
    private Computer computer;

    private int puzzleMoveSize = 2;
    private int moves;
    [SerializeField]
    private TMP_Text moveDisplayText;
    [SerializeField]
    private TMP_Text levelCompleteText;
    [SerializeField]
    private Animator canvasAnimator;
    private static GameManager instance;
    private int succeededCounter = 0;
    private bool succeeded = false;
    private bool failed = false;
    [SerializeField]
    private bool playground = false;

    [SerializeField]
    private GameObject markerPrefab;
    [SerializeField]
    private RectTransform shifter;
    private List<GameObject> markers = new List<GameObject>();

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
        Reset();
        if (!playground)
            StartRound();
        else
            player.Begin();
    }

    private void Update()
    {
        if (Input.anyKeyDown && succeeded)
        {
            Reset();
            player.Reset();
            StartRound();
            succeeded = false;
        }
        else if (Input.anyKeyDown && failed)
        {
            for (int i = 0; i < markers.Count; i++)
                Destroy(markers[i]);
            markers.Clear();
            Reset();
            player.Reset();
            failed = false;
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
            player.Reset();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }
    void StartRound()
    {
        computer.CreatePuzzle(puzzleMoveSize);
        player.Begin();
    }

    public void Reset()
    {
        moves = puzzleMoveSize;
        moveDisplayText.text = moves + "/" + puzzleMoveSize;
    }

    public virtual void MakeMove()
    {
        moves--;
        moveDisplayText.text = moves + "/" + puzzleMoveSize;
        player.AllowPlayerInput();
        AudioManager.Instance().PlaySound("move");
        StartCoroutine(DelayedCheckSolution());
        StartCoroutine(TextBumpAnim());
    }

    protected IEnumerator TextBumpAnim()
    {
        canvasAnimator.SetBool("bump", true);
        yield return new WaitForSeconds(0.1f);
        canvasAnimator.SetBool("bump", false);
    }

    private IEnumerator DelayedCheckSolution()
    {
        yield return new WaitForSeconds(0.4f);
        CheckSolution();
    }

    private void CheckSolution()
    {
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
                    player.AllowPlayerInput(true);
                    if (moves == 0)
                        Fail();
                    return;
                }
            }
        }
        // made it through all blocks without a mismatch!
        Success();
    }

    private void Success()
    {
        if (succeededCounter > 3)
        {
            succeededCounter = -1;
            puzzleMoveSize++;
            levelCompleteText.text = levelCompleteText.text.Remove(levelCompleteText.text.Length - 4, 4);
            levelCompleteText.text = $"{levelCompleteText.text}<color=#DDA853>*</color>";
        }
        else
            levelCompleteText.text = levelCompleteText.text + "*";

        player.AllowPlayerInput();
        succeeded = true;
        AudioManager.Instance().PlaySound("success");
        succeededCounter++;
    }

    private void Fail()
    {
        Debug.Log("FAILED");
        player.AllowPlayerInput();
        failed = true;

        // display markers
        Block[,] computerBlocks = computer.GetBlocks();
        for (int i = 0; i < 21; i++)
        {
            for (int j = 0; j < 21; j++)
            {
                if (computerBlocks[i, j] != null)
                    markers.Add(Instantiate(markerPrefab, computerBlocks[i, j].transform.position, Quaternion.identity, shifter));
            }
        }

        AudioManager.Instance().PlaySound("fail");
    }
}
