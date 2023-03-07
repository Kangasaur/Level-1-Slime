using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]

public class Character : MonoBehaviour
{
    protected NavMeshAgent nav;
    protected int health = 1;
    [SerializeField] protected GameObject weaponPrefab;
    protected Vector3 centerPos;

    protected virtual void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        nav.speed = 5;
    }

    public void TakeDamage()
    {
        health--;
        if (health == 0) AddToDieList();
    }

    void AddToDieList()
    {
        if (GameManager.instance.turn == GameManager.GameTurn.EnemyAttack)
        {
            GameManager.instance.destroyList.Add(gameObject);
        }
        else Die();
    }
    public virtual void Die()
    {
        Destroy(gameObject);
    }

    public virtual void Attack(List<GameObject> targets)
    {
        foreach (GameObject hex in targets)
        {
            GameObject character = GameManager.instance.GetOccupier(hex);
            if (character) character.GetComponent<Character>().TakeDamage();
        }
        Instantiate(weaponPrefab, centerPos, Quaternion.Euler(70, 0, 0));
    }
    public void MoveToHex(Transform hex)
    {
        nav.SetDestination(hex.transform.position + Vector3.up * 0.92f);
    }

    public Vector3 GetDestination()
    {
        return nav.destination;
    }
}
