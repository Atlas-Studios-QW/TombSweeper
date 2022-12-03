using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAnimation : MonoBehaviour
{
    [Header("Settings")]
    public int MoveInterval = 2;
    public int Direction = 4;

    private void Start()
    {
        StartCoroutine(NextMove());
    }

    private IEnumerator NextMove()
    {
        GetComponent<PlayerMovement>().MovePlayer(Direction);
        yield return new WaitForSeconds(MoveInterval);
        StartCoroutine(NextMove());
    }
}
