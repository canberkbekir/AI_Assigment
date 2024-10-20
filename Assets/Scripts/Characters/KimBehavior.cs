using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Characters
{
    public class KimBehavior : MonoBehaviour
    {
        [SerializeField] private Kim kim;
        [SerializeField] private List<Grid.Tile> _burgers = new();
        [SerializeField] private Grid _grid;
        [SerializeField] private int burgerIndex;
        [SerializeField] private List<Grid.Tile> temporarilyOccupiedTiles = new();

        private void Awake()
        {
            kim = GetComponent<Kim>();
        }

        private void Start()
        {
            _grid = Grid.Instance;
            if (_grid == null)
            {
                Debug.LogError("Grid is null");
                return;
            }

            GetBurgers();
            if (_burgers.Any())
                GoToBurger(_burgers[burgerIndex]);
        }

        private void Update()
        {
            MarkZombieTiles();

            if (IsOnBurger(_burgers[burgerIndex]))
                GoToNextBurger();
            else
                GoToBurger(_burgers[burgerIndex]);
        }

        private void OnDrawGizmos()
        {
            foreach (var tile in temporarilyOccupiedTiles)
            {
                Debug.Log($"Drawing Gizmo at tile: {tile.x}, {tile.y}");
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(new Vector3(tile.x, tile.y, 0), new Vector3(5, 5, 5));
            }
        }

        private void GetBurgers()
        {
            var burgerObjects = GameObject.FindGameObjectsWithTag("Burger");
            foreach (var burger in burgerObjects)
            {
                var tile = _grid.GetClosest(burger.transform.position);
                if (tile != null)
                    _burgers.Add(tile);
            }
        }

        private void GoToBurger(Grid.Tile burgerTile)
        {
            var currentTile = _grid.GetClosest(transform.position);
            if (currentTile == burgerTile) return;

            var path = AStarAlgorithm.FindPath(_grid, currentTile, burgerTile);
            if (path != null)
                kim.SetWalkBuffer(path);
        }

        private void GoToNextBurger()
        {
            if (burgerIndex >= _burgers.Count) GoToPortal();
            burgerIndex = (burgerIndex + 1) % _burgers.Count;
            GoToBurger(_burgers[burgerIndex]);
        }

        private void GoToPortal()
        {
            var portalTile = _grid.GetFinishTile();
            GoToBurger(portalTile);
        }

        private bool IsOnBurger(Grid.Tile burgerTile)
        {
            var currentTile = _grid.GetClosest(transform.position);
            return currentTile == burgerTile;
        }

        private void MarkZombieTiles()
        {
            var currentZombieTiles = new HashSet<Grid.Tile>();
            var zombies = kim.GetContextByTag("Zombie");

            foreach (var zombie in zombies)
            {
                var zombieTile = _grid.GetClosest(zombie.transform.position);
                if (zombieTile != null) currentZombieTiles.Add(zombieTile);
            }

            // Remove tiles that are no longer occupied by zombies
            temporarilyOccupiedTiles.RemoveAll(tile => !currentZombieTiles.Contains(tile));

            // Add new tiles that are now occupied by zombies
            foreach (var tile in currentZombieTiles)
                if (!temporarilyOccupiedTiles.Contains(tile))
                    temporarilyOccupiedTiles.Add(tile);
        }
    }
}