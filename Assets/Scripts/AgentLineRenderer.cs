using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentLineRenderer : MonoBehaviour
{
  public float Distance;

  LineRenderer line;
  float dist;
  private void Awake()
  {
    line = GetComponent<LineRenderer>();
    dist = Vector2.Distance(transform.parent.transform.position, transform.position);
  }

  void Update()
  {
    line.SetPosition(0, transform.position);
    line.SetPosition(1, transform.parent.transform.position);

    Vector2 dir = new Vector2(
      transform.parent.transform.position.x - transform.position.x,
      transform.parent.transform.position.y - transform.position.y
    );

    LayerMask mask = LayerMask.GetMask("Obstacle");
    RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dist, mask);
    if (hit.collider != null)
    {
      Distance = hit.distance;
      line.startColor = Color.red;
      line.endColor = Color.red;
    }
    else
    {
      Distance = dist;
      line.startColor = Color.green;
      line.endColor = Color.green;
    }
  }
}
