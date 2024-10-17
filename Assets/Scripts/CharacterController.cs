using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// _     __________    _    ______   __   ____ ___  ____  _____           
//| |   | ____/ ___|  / \  / ___\ \ / /  / ___/ _ \|  _ \| ____|          
//| |   |  _|| |  _  / _ \| |    \ V /  | |  | | | | | | |  _|            
//| |___| |__| |_| |/ ___ | |___  | |   | |__| |_| | |_| | |___           
//|_____|_____\____/_/   \_\____| |_|    \____\___/|____/|_____|          


// ____   ___    _   _  ___ _____    ____ _   _    _    _   _  ____ _____ 
//|  _ \ / _ \  | \ | |/ _ |_   _|  / ___| | | |  / \  | \ | |/ ___| ____|
//| | | | | | | |  \| | | | || |   | |   | |_| | / _ \ |  \| | |  _|  _|  
//| |_| | |_| | | |\  | |_| || |   | |___|  _  |/ ___ \| |\  | |_| | |___ 
//|____/ \___/  |_| \_|\___/ |_|    \____|_| |_/_/   \_|_| \_|\____|_____|

public class CharacterController : MonoBehaviour
{
    private const float ReachDistThreshold = 0.1f;
    private const float CharacterMoveSpeed = 1.0f;

    private Transform model;
    protected Animator myAnimator;

    protected Grid.Tile myCurrentTile;
    protected bool myReachedDestination;
    protected bool myReachedTile;
    protected List<Grid.Tile> myWalkBuffer = new();

    private void Start()
    {
        myCurrentTile = Grid.Instance.GetClosest(transform.position);
        myAnimator = GetComponentInChildren<Animator>();

        model = transform.GetChild(0).transform;
    }

    public virtual void UpdateCharacter()
    {
        var tilePosition = new Vector3();
        if (myCurrentTile != null) tilePosition = Grid.Instance.WorldPos(myCurrentTile);
        myReachedTile = Vector3.Distance(transform.position, tilePosition) < ReachDistThreshold;
        transform.position = Vector3.MoveTowards(transform.position, tilePosition, CharacterMoveSpeed * Time.deltaTime);

        myReachedDestination = myWalkBuffer.Count == 0;

        if (myWalkBuffer.Count > 0)
        {
            var t = myWalkBuffer.ElementAt(0);
            if (!t.occupied) MoveTile(myWalkBuffer.ElementAt(0));

            myAnimator.SetBool("Walk", true);
            SetForward((Grid.Instance.WorldPos(myWalkBuffer.ElementAt(0)) - myAnimator.transform.position).normalized);

            if (myReachedTile && myCurrentTile == t) myWalkBuffer.RemoveAt(0);
        }
        else if (myReachedTile)
        {
            myAnimator.SetBool("Walk", false);
        }
    }

    public virtual void StartCharacter()
    {
    }

    private void SetForward(Vector3 forward)
    {
        var newForward = forward;
        newForward.y = 0;

        model.forward = newForward.normalized;
    }

    public void MoveTile(Grid.Tile aTile)
    {
        if (myReachedTile && !aTile.occupied &&
            Grid.Instance.isReachable(myCurrentTile, aTile))
            myCurrentTile = aTile;
    }

    public void SetWalkBuffer(List<Grid.Tile> someTiles)
    {
        myWalkBuffer.Clear();
        myWalkBuffer.AddRange(someTiles);
    }
}