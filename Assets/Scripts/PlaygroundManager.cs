using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlaygroundManager : GameManager
{
    public override void MakeMove()
    {
        player.AllowPlayerInput();
        AudioManager.Instance().PlaySound("move");
        StartCoroutine(TextBumpAnim());
        StartCoroutine(DelayedPlayerAllow());
    }

    private IEnumerator DelayedPlayerAllow()
    {
        yield return new WaitForSeconds(0.4f);
        player.AllowPlayerInput(true);
    }

}
