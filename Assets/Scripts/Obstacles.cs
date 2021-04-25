namespace OpenSkiFree
{
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;
  using System.Linq;

  public class Obstacles : MonoBehaviour
  {
    public GameObject Player;
    public Camera Camera;

    public Sprite WhiteSquareSprite;
    public GameObject TreePrefab;

    Collider2D playerCollider;
    Player playerScript;

    List<Tile> configuredTiles = new List<Tile>();
    Tile lastTile;

    Dictionary<Tile, List<PolygonCollider2D>> collidersForTile = new Dictionary<Tile, List<PolygonCollider2D>>();

    void Awake()
    {
      playerCollider = Player.GetComponent<CapsuleCollider2D>();
      playerScript = Player.GetComponent<Player>();
    }

    void Update()
    {
      // Get the current tile and all surrounding tiles
      float height = Camera.orthographicSize * 2;
      float width = (height / Screen.height) * Screen.width;
      int tileXIndex = Mathf.FloorToInt(Mathf.Abs(Player.transform.position.x) / width);
      int tileYIndex = Mathf.FloorToInt(Mathf.Abs(Player.transform.position.y) / height);

      if (Player.transform.position.x < 0)
      {
        tileXIndex *= -1;
      }

      if (Player.transform.position.y < 0)
      {
        tileYIndex *= -1;
      }

      for (int x = -2; x <= 2; x++)
      {
        for (int y = -2; y < 2; y++)
        {
          handleTile(tileXIndex + x, tileYIndex + y, width, height);
        }
      }

      List<Tile> tilesToDestroy = new List<Tile>();
      foreach (Tile tile in configuredTiles)
      {
        if (
          tile.xIndex < tileXIndex - 2 ||
          tile.xIndex > tileXIndex + 2 ||
          tile.yIndex > tileYIndex + 2 ||
          tile.yIndex < tileYIndex - 2
        )
        {
          tilesToDestroy.Add(tile);
        }
      }
      foreach (Tile t in tilesToDestroy)
      {
        collidersForTile[t].Clear();
        GameObject.Destroy(t.gameObject);
        configuredTiles.Remove(t);
      }

      foreach (KeyValuePair<Tile, List<PolygonCollider2D>> colliderAndTile in collidersForTile)
      {
        foreach (PolygonCollider2D collider in colliderAndTile.Value)
        {
          if (collider.bounds.Intersects(playerCollider.bounds))
          {
            playerScript.didCollideWithObstacle();

            // To avoid re-colliding with this obstacle, remove this collider from our dictionary
            // We use a coroutine to avoid issues with mutating what we're iterating over
            StartCoroutine(cleanupSingleObstacle(colliderAndTile.Key, collider));
          }
        }
      }
    }

    IEnumerator cleanupSingleObstacle(Tile tile, PolygonCollider2D polygonCollider)
    {
      yield return new WaitForEndOfFrame();
      int index = collidersForTile[tile].IndexOf(polygonCollider);
      collidersForTile[tile].RemoveAt(index);
    }

    void handleTile(int xIndex, int yIndex, float cameraWidth, float cameraHeight)
    {
      if (configuredTiles.Exists(t => t.xIndex == xIndex && t.yIndex == yIndex))
      {
        return;
      }

      // Setup a container with a background object for debugging purposes
      GameObject container = new GameObject();

      GameObject tileObj = new GameObject();
      tileObj.transform.SetParent(container.transform);

      container.name = "Tile x:" + xIndex + " y:" + yIndex;
      container.transform.SetParent(transform);

      tileObj.AddComponent<SpriteRenderer>();
      tileObj.name = "Background";

      SpriteRenderer sr = tileObj.GetComponent<SpriteRenderer>();
      sr.sprite = WhiteSquareSprite;
      sr.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 0.3f);

      tileObj.transform.localScale = new Vector3(Screen.width, Screen.height, 1f);
      container.transform.position = new Vector3(xIndex * cameraWidth, yIndex * cameraHeight);

      // Randomly add trees
      // First break the new tile into 16 quadrants each with a random value
      // Add a tree to the highest x values
      List<ObstacleCandidate> treeCandidates = new List<ObstacleCandidate>();

      for (int x = -2; x <= 2; x++)
      {
        for (int y = -2; y <= 2; y++)
        {
          Vector3 pos = new Vector3((cameraWidth / 5) * x, (cameraHeight / 5) * y, 0f);
          treeCandidates.Add(new ObstacleCandidate(Random.Range(0f, 1f), pos));
        }
      }

      Tile tile = new Tile(xIndex, yIndex, container);
      collidersForTile[tile] = new List<PolygonCollider2D>();

      List<ObstacleCandidate> sortedTreeCandidates = treeCandidates.OrderBy(o => o.value).ToList();
      int treesPerTile = 3;
      for (int x = 0; x < treesPerTile; x++)
      {
        GameObject tree = Instantiate(TreePrefab, Vector3.zero, Quaternion.identity);
        tree.transform.SetParent(container.transform);
        tree.transform.localPosition = sortedTreeCandidates[x].pos;
        collidersForTile[tile].Add(tree.GetComponent<PolygonCollider2D>());
      }

      configuredTiles.Add(tile);
    }
  }

  struct ObstacleCandidate
  {
    public float value;
    public Vector3 pos;

    public ObstacleCandidate(float v, Vector3 p)
    {
      value = v;
      pos = p;
    }
  }
}