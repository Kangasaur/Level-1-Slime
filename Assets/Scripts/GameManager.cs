using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    GameObject selected;
    Player player;

    [SerializeField] TextMeshProUGUI actionText;
    [SerializeField] TextMeshProUGUI turnText;

    [SerializeField] LayerMask hexMask;
    [SerializeField] GameObject[] enemyTypes;
    [HideInInspector] public enum GameTurn { Player, EnemyAttack, EnemyMove };
    [HideInInspector] public GameTurn turn { get; private set; }

    int turns = 0;

    [HideInInspector] public HexMaterials[] hexes;
    [HideInInspector] public List<GameObject> destroyList = new List<GameObject>();

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(instance);
        player = FindObjectOfType<Player>();
        hexes = FindObjectsOfType<HexMaterials>();
        StartPlayerTurn();
    }

    // Update is called once per frame
    void Update()
    {
        if (turn == GameTurn.Player)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, hexMask))
            {
                if (selected) selected.GetComponent<HexMaterials>().Select(false);
                selected = hit.collider.gameObject;
                selected.GetComponent<HexMaterials>().Select(true);

                if (Input.GetMouseButtonDown(0))
                {
                    if (!GetOccupier(selected) && HexDistance(selected.transform, player.transform) <= player.actions)
                    {
                        player.actions -= HexDistance(selected.transform, player.transform);
                        player.MoveToHex(selected.transform);
                        actionText.text = "Actions: " + player.actions.ToString();
                    }
                    else if (GetOccupier(selected) && HexDistance(selected.transform, player.transform) == 1 && player.actions >= 1)
                    {
                        Debug.Log(GetOccupier(selected).name);
                        player.actions--;
                        player.SetCenterPos(selected.transform.position + new Vector3(0f, 1.02f, 0f));
                        player.Attack(new List<GameObject>() { selected });
                        actionText.text = "Actions: " + player.actions.ToString();
                    }
                }
            }
            else if (selected)
            {
                selected.GetComponent<HexMaterials>().Select(false);
                selected = null;
            }
        }
    }

    public GameObject GetOccupier(GameObject hex)
    {
        foreach (Character character in FindObjectsOfType<Character>())
        {
            Vector3 d = character.GetComponent<NavMeshAgent>().destination;
            //Debug.Log(d);
            if (Vector3.Distance(d, hex.transform.position + Vector3.up * 0.92f) < 0.1f)
            {
                return character.gameObject;
            }
        }
        return null;
    }

    public void EndTurn()
    {
        if (turn == GameTurn.Player) DoEnemyAttack();
    }

    public void DoEnemyAttack()
    {
        turn = GameTurn.EnemyAttack;
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            enemy.Attack(enemy.targets);
        }
        Invoke("DoEnemyMove", 1f);
    }

    public void DoEnemyMove()
    {
        turn = GameTurn.EnemyMove;
        foreach (GameObject kill in destroyList)
        {
            kill.GetComponent<Character>().Die();
        }
        destroyList.Clear();
        foreach (HexMaterials hex in hexes) hex.Target(false);
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            enemy.ClearTargets();
            enemy.ChooseMoveDirection();
        }
        Invoke("StartPlayerTurn", 0.5f);
    }

    public void StartPlayerTurn()
    {
        turn = GameTurn.Player;
        turns++;
        turnText.text = "Turn " + turns.ToString();
        for (int i = 0; i < 2; i++)
        {
            Instantiate(enemyTypes[Random.Range(0, enemyTypes.Length)], RandomEmptyPosition(), Quaternion.identity);
        }
        foreach (Enemy enemy in FindObjectsOfType<Enemy>()) enemy.SetTargets();
        player.actions = 3;
    }
    public int HexDistance(Transform hex, Transform character)
    {
        Vector3 hexPos = hex.position + Vector3.up * 0.5f;
        return Mathf.CeilToInt(Vector3.Distance(hexPos, character.GetComponent<NavMeshAgent>().destination) / 2 - 0.05f);
    }

    Vector3 RandomEmptyPosition()
    {
        GameObject checkHex = hexes[Random.Range(0, hexes.Length)].gameObject;
        if (GetOccupier(checkHex))
        {
            while (GetOccupier(checkHex) != null)
            {
                checkHex = hexes[Random.Range(0, hexes.Length)].gameObject;
            }
        }
        return checkHex.transform.position + Vector3.up * 0.92f;
    }
}
