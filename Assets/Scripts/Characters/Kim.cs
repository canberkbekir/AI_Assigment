using System.Collections.Generic;
using UnityEngine;

public class Kim : CharacterController
{
    [SerializeField] private float ContextRadius;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, ContextRadius);

        Gizmos.color = new Color(0, 1, 0, 0.3f); // Red color with 50% opacity
        var offSet = new Vector3(0, 0.35f, 0);
        Gizmos.DrawSphere(transform.position + offSet, 0.5f);
    }


    public override void StartCharacter()
    {
        base.StartCharacter();
    }

    public override void UpdateCharacter()
    {
        base.UpdateCharacter();

        var closest = GetClosest(GetContextByTag("Zombie"))?.GetComponent<Zombie>();
    }

    private Vector3 GetEndPoint()
    {
        return Grid.Instance.WorldPos(Grid.Instance.GetFinishTile());
    }

    public GameObject[] GetContextByTag(string aTag)
    {
        var context = Physics.OverlapSphere(transform.position, ContextRadius);
        var returnContext = new List<GameObject>();
        foreach (var c in context)
            if (c.transform.CompareTag(aTag))
                returnContext.Add(c.gameObject);
        return returnContext.ToArray();
    }

    private GameObject GetClosest(GameObject[] aContext)
    {
        var dist = float.MaxValue;
        GameObject Closest = null;
        foreach (var z in aContext)
        {
            var curDist = Vector3.Distance(transform.position, z.transform.position);
            if (curDist < dist)
            {
                dist = curDist;
                Closest = z;
            }
        }

        return Closest;
    }
}