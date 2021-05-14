namespace OpenSkiFree
{
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;
  using System.Linq;

  public class AgentObstacles : MonoBehaviour
  {
    public GameObject TreePrefab;
    public GameObject TreeContainer;
    public Collider2D PlayerAgentCollider;
    public PlayerAgent PlayerAgent;
    public Transform MinTreeTransform;
    public Transform MaxTreeTransform;

    List<Tile> configuredTiles = new List<Tile>();
    Tile lastTile;

    Dictionary<Tile, List<PolygonCollider2D>> collidersForTile = new Dictionary<Tile, List<PolygonCollider2D>>();

    List<PolygonCollider2D> treeColliders = new List<PolygonCollider2D>();

    public void RandomizeTrees()
    {
      if (treeColliders.Count > 0)
      {
        List<GameObject> objsToDestroy = new List<GameObject>();
        foreach (PolygonCollider2D collider in treeColliders)
        {
          objsToDestroy.Add(collider.gameObject);
        }

        for (int o = 0; o < objsToDestroy.Count; o++)
        {
          Destroy(objsToDestroy[o]);
        }

        treeColliders.Clear();
      }

      Vector3 maxTreePos = MaxTreeTransform.localPosition;
      Vector3 minTreePos = MinTreeTransform.localPosition;

      List<ObstacleCandidate> treeCandidates = new List<ObstacleCandidate>();

      int colCount = 5;
      int rowCount = 5;

      float rowHeight = (maxTreePos.y - minTreePos.y) / rowCount;
      float colWidth = (maxTreePos.x - minTreePos.x) / colCount;

      // Divide tree spawn range into rows and columns
      for (int col = 0; col < colCount; col++)
      {
        for (int row = 0; row < rowCount; row++)
        {
          Vector3 pos = new Vector3(minTreePos.x + colWidth * col, minTreePos.y + rowHeight * row, 0f);
          treeCandidates.Add(new ObstacleCandidate(Random.Range(0f, 1f), pos));
        }
      }

      List<ObstacleCandidate> sortedTreeCandidates = treeCandidates.OrderBy(o => o.value).ToList();

      for (int x = 0; x < 4; x++)
      {
        Vector3 posWithOffset = new Vector3(
          Random.Range(-colWidth / 2.5f, colWidth / 2.5f) + sortedTreeCandidates[x].pos.x,
          Random.Range(-rowHeight / 2.5f, rowHeight / 2.5f) + sortedTreeCandidates[x].pos.y,
          0f
        );
        GameObject tree = Instantiate(TreePrefab, Vector3.zero, Quaternion.identity);
        tree.transform.SetParent(TreeContainer.transform);
        tree.transform.localPosition = posWithOffset;
        treeColliders.Add(tree.GetComponent<PolygonCollider2D>());
      }
    }

    public void CheckForCollision()
    {
      foreach (PolygonCollider2D collider in treeColliders)
      {
        if (collider.bounds.Intersects(PlayerAgentCollider.bounds))
        {
          PlayerAgent.didCollideWithObstacle();
        }
      }
    }
  }
}