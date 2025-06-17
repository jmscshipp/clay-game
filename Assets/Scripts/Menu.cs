using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField]
    Progresssor progressor;

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

    // Start is called before the first frame update
    void Start()
    {
        progressor.SetColorMode(true);
        // C
        progressor.CreateBlock(blocks, recentBlocks, 3, 11);
        progressor.CreateBlock(blocks, recentBlocks, 4, 11);
        progressor.CreateBlock(blocks, recentBlocks, 5, 11);
        progressor.CreateBlock(blocks, recentBlocks, 3, 12);
        progressor.CreateBlock(blocks, recentBlocks, 3, 13);
        progressor.CreateBlock(blocks, recentBlocks, 3, 14);
        progressor.CreateBlock(blocks, recentBlocks, 4, 14);
        progressor.CreateBlock(blocks, recentBlocks, 5, 14);
        // L
        progressor.CreateBlock(blocks, recentBlocks, 7, 11);
        progressor.CreateBlock(blocks, recentBlocks, 8, 11);
        progressor.CreateBlock(blocks, recentBlocks, 9, 11);
        progressor.CreateBlock(blocks, recentBlocks, 7, 12);
        progressor.CreateBlock(blocks, recentBlocks, 7, 13);
        progressor.CreateBlock(blocks, recentBlocks, 7, 14);
        // A
        progressor.CreateBlock(blocks, recentBlocks, 11, 11);
        progressor.CreateBlock(blocks, recentBlocks, 11, 12);
        progressor.CreateBlock(blocks, recentBlocks, 12, 12);
        progressor.CreateBlock(blocks, recentBlocks, 11, 13);
        progressor.CreateBlock(blocks, recentBlocks, 11, 14);
        progressor.CreateBlock(blocks, recentBlocks, 12, 14);
        progressor.CreateBlock(blocks, recentBlocks, 13, 11);
        progressor.CreateBlock(blocks, recentBlocks, 13, 12);
        progressor.CreateBlock(blocks, recentBlocks, 13, 13);
        progressor.CreateBlock(blocks, recentBlocks, 13, 14);
        // Y
        progressor.CreateBlock(blocks, recentBlocks, 15, 13);
        progressor.CreateBlock(blocks, recentBlocks, 15, 14);
        progressor.CreateBlock(blocks, recentBlocks, 17, 13);
        progressor.CreateBlock(blocks, recentBlocks, 17, 14);
        progressor.CreateBlock(blocks, recentBlocks, 16, 11);
        progressor.CreateBlock(blocks, recentBlocks, 16, 12);
        progressor.CreateBlock(blocks, recentBlocks, 16, 13);

        StartCoroutine(DelayedGrow());
    }

    private IEnumerator DelayedGrow()
    {
        yield return new WaitForSeconds(2f);
        AudioManager.Instance().PlaySound("success");
        progressor.Grow(blocks, recentBlocks, 3, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadChallengeScene()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadPlaygroundScene()
    {
        SceneManager.LoadScene(2);
    }
}
