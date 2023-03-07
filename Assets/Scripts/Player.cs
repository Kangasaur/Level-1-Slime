using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Character
{
    public int actions = 3;

    public void SetCenterPos(Vector3 pos)
    {
        centerPos = pos;
    }

    public override void Die()
    {
        base.Die();
        SceneManager.LoadScene("Death");
    }
}
