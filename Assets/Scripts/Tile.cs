
namespace OpenSkiFree
{
  using UnityEngine;
  public struct Tile
  {
    public int xIndex;
    public int yIndex;

    public GameObject gameObject;

    public Tile(int x, int y, GameObject containerObj)
    {
      xIndex = x;
      yIndex = y;
      gameObject = containerObj;
    }
  }
}