using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : Enemy
{
    public override void ChooseMoveDirection()
    {
        Vector3 direction = -GetDirectionToPlayer();
        if (Physics.CheckBox(transform.position + direction * 2 + Vector3.down, Vector3.one * 0.25f))
        {
            Transform movementHex = Physics.OverlapBox(transform.position + direction * 2 + Vector3.down, Vector3.one * 0.25f)[0].transform;
            if (!GameManager.instance.GetOccupier(movementHex.gameObject))
            {
                MoveToHex(movementHex);
            }
        }
    }

    public override void SetTargets()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        foreach (HexMaterials hex in GameManager.instance.hexes)
        {
            if (GameManager.instance.HexDistance(hex.transform, player.transform) <= 1)
            {
                targets.Add(hex.gameObject);
                hex.Target(true);
            }
            if (GameManager.instance.GetOccupier(hex.gameObject) == player)
            {
                targets.Add(hex.gameObject);
                hex.Target(true);
                centerPos = hex.transform.position + new Vector3(0f, 1.692f, 1.12f);
            }
        }
    }
}
