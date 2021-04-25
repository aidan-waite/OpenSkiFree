namespace OpenSkiFree
{
  using UnityEngine;
  public class FollowPlayer : MonoBehaviour
  {
    public GameObject Player;
    void Update()
    {
      transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, -10f);
    }
  }
}