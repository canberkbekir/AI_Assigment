using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//#     # #     #  #####  #     #    ######  ######     #    ### #     #  #####  
//#     # #     # #     # #     #    #     # #     #   # #    #  ##    # #     # 
//#     # #     # #       #     #    #     # #     #  #   #   #  # #   # #       
//#     # #     # #  #### #######    ######  ######  #     #  #  #  #  #  #####  
//#     # #     # #     # #     #    #     # #   #   #######  #  #   # #       # 
//#     # #     # #     # #     #    #     # #    #  #     #  #  #    ## #     # 
// #####   #####   #####  #     #    ######  #     # #     # ### #     #  #####  

public class Zombie : CharacterController
{
    private readonly Vector2 braaaaains = new(5, 8);
    private Vector2Int uhghbrains = new(3, 7);
    private float uuugh;

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f); // Red color with 50% opacity
        var offSet = new Vector3(0, 0.35f, 0);
        Gizmos.DrawSphere(transform.position + offSet, 0.5f);
    }

    public override void StartCharacter()
    {
        myCurrentTile = Grid.Instance.GetClosest(transform.position);
        base.StartCharacter();
    }

    public override void UpdateCharacter()
    {
        base.UpdateCharacter();

        if (uuugh > 0)
            uuugh -= Time.deltaTime;
        else
            ughBrainBrainugh();
    }

    public void ughBrainBrainugh()
    {
        uuugh = Random.Range(braaaaains.x, braaaaains.y);
        brainbrainzombiebrain();
    }

    public void brainbrainzombiebrain()
    {
        var braaaaaains = Random.Range(0, 4);
        var ughghhhhhh = Random.Range(uhghbrains.x, uhghbrains.y);

        var brains = new List<Grid.Tile>();

        var bruhains = Vector2Int.zero;

        switch (braaaaaains)
        {
            case 0:
                bruhains = new Vector2Int(1, 0);
                break;
            case 1:
                bruhains = new Vector2Int(-1, 0);
                break;
            case 2:
                bruhains = new Vector2Int(0, 1);
                break;
            case 3:
                bruhains = new Vector2Int(0, -1);
                break;
        }

        for (var brainses = 1; brainses < ughghhhhhh + 1; brainses++)
        {
            var brainbrains = Grid.Instance.TryGetTile(new Vector2Int(myCurrentTile.x + bruhains.x * brainses,
                myCurrentTile.y + bruhains.y * brainses));
            if (brainbrains != null)
                if (!brainbrains.occupied)
                    brains.Add(brainbrains);
        }

        SetWalkBuffer(brains);
    }
}