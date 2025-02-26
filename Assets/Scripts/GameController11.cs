using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController11 : Singleton<GameController11>
{
    private GameObject[] balls;
    private GameObject[] goals;

    // Start is called before the first frame update
    void Start()
    {
        balls = GameObject.FindGameObjectsWithTag("Ball");
        goals = GameObject.FindGameObjectsWithTag("Finish");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckWin()
    {
        foreach (var goal in goals)
        {
            bool samePos = false;
            foreach (var ball in balls)
            {
                if (Vector3.Distance(ball.transform.position, goal.transform.position) < 0.01f)
                {
                    samePos = true;
                    break;
                }
            }
            if (!samePos) return;
        }

        Debug.Log("Win");
        GameManager11.Instance.GameWin();
    }
}
