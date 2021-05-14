namespace OpenSkiFree
{
  using UnityEngine;
  using UnityEngine.UI;

  enum PlayerState
  {
    FullDown, // 1
    MostlyDownRight, // 2
    MostlyDownLeft, // 3
    MostlySidewaysRight, // 4
    MostlySidewaysLeft, // 5
    FullSidewaysRight, // 6
    FullSidewaysLeft, // 7
    WipeOut
  }

  public class Player : MonoBehaviour
  {
    public Text SpeedText;
    public Sprite PlayerFullSideways;
    public Sprite PlayerMostlySideways;
    public Sprite PlayerMostlyDown;
    public Sprite PlayerFullDown;
    public Sprite PlayerWipeOut;

    public SpriteRenderer PlayerSpriteRenderer;
    PlayerState currentState;

    float playerSpriteScale = 2f;
    float startWipeOut;

    private void Awake()
    {
      Application.targetFrameRate = 90;
    }

    private void Update()
    {
      if (currentState == PlayerState.WipeOut)
      {
        if (Time.timeSinceLevelLoad - startWipeOut > 1.5)
        {
          cumulativeSpeed = 0f;
          currentState = PlayerState.FullSidewaysRight;
        }
        return;
      }

      updatePlayerState();
      updatePlayerSprite();
      movePlayer();
    }

    public void didCollideWithObstacle()
    {
      if (Time.timeSinceLevelLoad < 1) { return; }

      startWipeOut = Time.timeSinceLevelLoad;
      currentState = PlayerState.WipeOut;
      PlayerSpriteRenderer.sprite = PlayerWipeOut;
    }

    private void updatePlayerState()
    {
      Vector3 mousePos = new Vector3(Input.mousePosition.x - Screen.width / 2, Input.mousePosition.y - Screen.height / 2, 0);

      // Choose sprite based on the angle of the mouse
      float angle = Vector3.Angle(mousePos, Vector3.down);
      switch (angle)
      {
        case float x when (x < 15):
          currentState = PlayerState.FullDown;
          break;

        case float x when (x >= 15 && x < 40):
          currentState = mousePos.x > 0 ? PlayerState.MostlyDownRight : PlayerState.MostlyDownLeft;
          break;

        case float x when (x >= 40 && x < 60):
          currentState = mousePos.x > 0 ? PlayerState.MostlySidewaysRight : PlayerState.MostlySidewaysLeft;
          break;

        case float x when (x > 60):
          currentState = mousePos.x > 0 ? PlayerState.FullSidewaysRight : PlayerState.FullSidewaysLeft;
          break;
      }
    }

    private void updatePlayerSprite()
    {
      switch (currentState)
      {
        case PlayerState.FullSidewaysLeft:
          PlayerSpriteRenderer.sprite = PlayerFullSideways;
          PlayerSpriteRenderer.transform.localScale = new Vector3(-playerSpriteScale, playerSpriteScale, playerSpriteScale);
          break;
        case PlayerState.MostlySidewaysLeft:
          PlayerSpriteRenderer.sprite = PlayerMostlySideways;
          PlayerSpriteRenderer.transform.localScale = new Vector3(-playerSpriteScale, playerSpriteScale, playerSpriteScale);
          break;
        case PlayerState.MostlyDownLeft:
          PlayerSpriteRenderer.sprite = PlayerMostlyDown;
          PlayerSpriteRenderer.transform.localScale = new Vector3(-playerSpriteScale, playerSpriteScale, playerSpriteScale);
          break;
        case PlayerState.FullDown:
          PlayerSpriteRenderer.sprite = PlayerFullDown;
          PlayerSpriteRenderer.transform.localScale = new Vector3(playerSpriteScale, playerSpriteScale, playerSpriteScale);
          break;
        case PlayerState.FullSidewaysRight:
          PlayerSpriteRenderer.sprite = PlayerFullSideways;
          PlayerSpriteRenderer.transform.localScale = new Vector3(playerSpriteScale, playerSpriteScale, playerSpriteScale);
          break;
        case PlayerState.MostlySidewaysRight:
          PlayerSpriteRenderer.sprite = PlayerMostlySideways;
          PlayerSpriteRenderer.transform.localScale = new Vector3(playerSpriteScale, playerSpriteScale, playerSpriteScale);
          break;
        case PlayerState.MostlyDownRight:
          PlayerSpriteRenderer.sprite = PlayerMostlyDown;
          PlayerSpriteRenderer.transform.localScale = new Vector3(playerSpriteScale, playerSpriteScale, playerSpriteScale);
          break;
      }
    }

    float cumulativeSpeed = 0f;
    float minSpeed = 25f;
    float maxSpeed = 1000f;

    private void movePlayer()
    {
      float speed = 0f;
      speed = Mathf.Min(maxSpeed, cumulativeSpeed);
      speed = Mathf.Max(minSpeed, cumulativeSpeed);

      SpeedText.text = "Speed: " + Mathf.Max(Mathf.FloorToInt(cumulativeSpeed), 0f);

      switch (currentState)
      {
        case PlayerState.FullSidewaysLeft:
          transform.position = new Vector3(
            transform.position.x - (speed * 0.2f * Time.deltaTime),
            transform.position.y,
            transform.position.z
          );
          if (cumulativeSpeed > 0f)
          {
            cumulativeSpeed -= Time.deltaTime * 10f;
          }
          break;
        case PlayerState.FullSidewaysRight:
          transform.position = new Vector3(
            transform.position.x + (speed * 0.2f * Time.deltaTime),
            transform.position.y,
            transform.position.z
          );
          if (cumulativeSpeed > 0f)
          {
            cumulativeSpeed -= Time.deltaTime * 10f;
          }
          break;

        case PlayerState.MostlySidewaysLeft:
          transform.position = new Vector3(
            transform.position.x - (speed * 0.2f * Time.deltaTime),
            transform.position.y - (speed * 0.1f * Time.deltaTime),
            transform.position.z
          );
          if (cumulativeSpeed > 0f)
          {
            cumulativeSpeed -= Time.deltaTime * 1.5f;
          }
          break;
        case PlayerState.MostlySidewaysRight:
          transform.position = new Vector3(
            transform.position.x + (speed * 0.2f * Time.deltaTime),
            transform.position.y - (speed * 0.1f * Time.deltaTime),
            transform.position.z
          );
          if (cumulativeSpeed > 0f)
          {
            cumulativeSpeed -= Time.deltaTime * 1.5f;
          }
          break;

        case PlayerState.MostlyDownLeft:
          transform.position = new Vector3(
            transform.position.x - (speed * 0.15f * Time.deltaTime),
            transform.position.y - (speed * 0.28f * Time.deltaTime),
            transform.position.z
          );
          cumulativeSpeed += Time.deltaTime * 2f;
          break;
        case PlayerState.MostlyDownRight:
          transform.position = new Vector3(
            transform.position.x + (speed * 0.15f * Time.deltaTime),
            transform.position.y - (speed * 0.28f * Time.deltaTime),
            transform.position.z
          );
          cumulativeSpeed += Time.deltaTime * 2f;
          break;

        case PlayerState.FullDown:
          transform.position = new Vector3(
            transform.position.x,
            transform.position.y - (speed * 0.45f * Time.deltaTime),
            transform.position.z
          );
          cumulativeSpeed += Time.deltaTime * 5f;
          break;
      }
    }
  }
}