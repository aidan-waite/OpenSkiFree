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

    public List<GameObject> SetupTrees()
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

      int colCount = 7;
      int rowCount = 9;

      float rowHeight = (maxTreePos.y - minTreePos.y) / rowCount;
      float colWidth = (maxTreePos.x - minTreePos.x) / colCount;

      for (int col = 0; col < colCount; col++)
      {
        for (int row = 0; row < rowCount; row++)
        {
          Vector3 pos = new Vector3(minTreePos.x + colWidth * col, minTreePos.y + rowHeight * row, 0f);
          treeCandidates.Add(new ObstacleCandidate(Random.Range(0f, 1f), pos));
        }
      }

      List<ObstacleCandidate> sortedTreeCandidates = treeCandidates.OrderBy(o => o.value).ToList();
      List<GameObject> spawnedTrees = new List<GameObject>();

      for (int x = 0; x < 20; x++)
      {
        GameObject tree = Instantiate(TreePrefab, Vector3.zero, Quaternion.identity);
        tree.transform.SetParent(TreeContainer.transform);
        float offsetMagnitude = 1.35f;
        float offsetX = Random.Range(-offsetMagnitude, offsetMagnitude);
        float offsetY = Random.Range(-offsetMagnitude, offsetMagnitude);

        float treeX = sortedTreeCandidates[x].pos.x + offsetX;
        treeX = Mathf.Min(MaxTreeTransform.localPosition.x, treeX);
        treeX = Mathf.Max(MinTreeTransform.localPosition.x, treeX);

        float treeY = sortedTreeCandidates[x].pos.y + offsetY;
        treeY = Mathf.Min(MaxTreeTransform.localPosition.y, treeY);
        treeY = Mathf.Max(MinTreeTransform.localPosition.y, treeY);

        tree.transform.localPosition = new Vector3(treeX, treeY, 0f);
        spawnedTrees.Add(tree);
        treeColliders.Add(tree.GetComponent<PolygonCollider2D>());
      }

      return spawnedTrees;
    }

    public void CheckForCollision()
    {
      foreach (PolygonCollider2D collider in treeColliders)
      {
        if (collider.bounds.Intersects(PlayerAgentCollider.bounds))
        {
          //print("didCollideWithObstacle");
          PlayerAgent.didCollideWithObstacle();
          return;
        }
      }
    }
  }
}