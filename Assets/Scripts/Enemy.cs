using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [HideInInspector] public List<GameObject> targets;

    public virtual void SetTargets()
    {
        //Target hexes adjacent to the enemy object
        foreach (HexMaterials hex in GameManager.instance.hexes)
        {
            if (GameManager.instance.HexDistance(hex.transform, transform) == 1 && GameManager.instance.GetOccupier(hex.gameObject) != gameObject)
            {
                targets.Add(hex.gameObject);
                hex.Target(true);
            }
            if (GameManager.instance.GetOccupier(hex.gameObject) == gameObject) centerPos = hex.transform.position + Vector3.up * 1f;
        }
    }

    public virtual void ChooseMoveDirection()
    {
        Vector3 direction = GetDirectionToPlayer();
        //Debug.Log(direction);
        if (Physics.CheckBox(transform.position + direction * 2 + Vector3.down, Vector3.one * 0.25f))
        {
            Transform movementHex = Physics.OverlapBox(transform.position + direction * 2 + Vector3.down, Vector3.one * 0.25f)[0].transform;
            if (!GameManager.instance.GetOccupier(movementHex.gameObject))
            {
                MoveToHex(movementHex);
            }
        }
    }

    protected Vector3 GetDirectionToPlayer()
    {
        Transform player = FindObjectOfType<Player>().transform;
        Vector3 rawDirection = transform.position - player.position;
        float newAngle = Mathf.Round(Vector3.SignedAngle(Vector3.forward, rawDirection, Vector3.up) /60f) * 60f;
        Vector3 direction = new Vector3(-Mathf.Sin(newAngle * Mathf.Deg2Rad), 0f, -Mathf.Cos(newAngle * Mathf.Deg2Rad));
        return direction;

    }

    public override void Die()
    {
        ClearTargets();
        base.Die();
    }

    public void ClearTargets()
    {
        foreach (GameObject target in targets)
        {
            target.GetComponent<HexMaterials>().Target(false);
            foreach (Enemy enemy in FindObjectsOfType<Enemy>())
            {
                if (enemy.targets.Contains(target) && enemy != this)
                {
                    target.GetComponent<HexMaterials>().Target(true);
                    break;
                }
            }
        }
        targets.Clear();
    }
}
