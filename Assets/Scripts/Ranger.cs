using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ranger : Enemy
{
    Vector3 direction;
    public override void ChooseMoveDirection()
    {
        base.ChooseMoveDirection();
    }

    public override void SetTargets()
    {
        if (nav == null) nav = GetComponent<NavMeshAgent>();
        direction = GetDirectionToPlayer();
        //Target all hexes in the general direction of the player
        for (int i = 2; i < 12; i += 2)
        {
            if (Physics.CheckBox(nav.destination + direction * i + Vector3.down, Vector3.one * 0.25f))
            {
                GameObject newTarget = Physics.OverlapBox(nav.destination + direction * i + Vector3.down, Vector3.one * 0.25f)[0].gameObject;
                targets.Add(newTarget);
                newTarget.GetComponent<HexMaterials>().Target(true);
            }
        }
    }

    public override void Attack(List<GameObject> targets)
    {
        foreach (GameObject hex in targets)
        {
            GameObject character = GameManager.instance.GetOccupier(hex);
            if (character) character.GetComponent<Character>().TakeDamage();
        }
        centerPos = nav.destination + Vector3.up * 0.2f;
        GameObject arrow = Instantiate(weaponPrefab, centerPos, Quaternion.identity);
        arrow.transform.rotation = Quaternion.Euler(70f, Vector3.SignedAngle(Vector3.forward, direction, Vector3.up) - 90f, 0f);
        arrow.GetComponent<Arrow>().direction = direction;
    }
}
