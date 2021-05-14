namespace OpenSkiFree
{
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;
  using Unity.MLAgents;
  using Unity.MLAgents.Sensors;

  public class PlayerAgent : Agent
  {
    public AgentObstacles AgentObstacles;
    public Sprite PlayerFullSideways;
    public Sprite PlayerMostlySideways;
    public Sprite PlayerMostlyDown;
    public Sprite PlayerFullDown;
    public Sprite PlayerWipeOut;

    public SpriteRenderer PlayerSpriteRenderer;

    PlayerState currentState;

    public Transform Target;
    float lastDistance = 0f;

    float playerSpriteScale = 2f;

    List<float> claimedRewards = new List<float>();
    List<float> possibleRewards = new List<float>();

    public void didCollideWithObstacle()
    {
      currentState = PlayerState.WipeOut;
      PlayerSpriteRenderer.sprite = PlayerWipeOut;
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
    public override void OnEpisodeBegin()
    {
      transform.localPosition = new Vector3(0f, 4.18f, 0f);
      lastDistance = Vector3.Distance(transform.position, Target.position);
      AgentObstacles.RandomizeTrees();
      cumulativeSpeed = 0f;

      for (float x = 3.3f; x > Target.localPosition.y; x -= 1f)
      {
        possibleRewards.Add(x);
      }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
      sensor.AddObservation(Target.position);
      sensor.AddObservation(transform.position);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
      var action = Mathf.RoundToInt(vectorAction[0]);
      // Actions
      switch (action)
      {
        case 0:
          // Do nothing
          break;
        case 1:
          currentState = PlayerState.FullDown;
          break;
        case 2:
          currentState = PlayerState.MostlyDownRight;
          break;
        case 3:
          currentState = PlayerState.MostlyDownLeft;
          break;
        case 4:
          currentState = PlayerState.MostlySidewaysRight;
          break;
        case 5:
          currentState = PlayerState.MostlySidewaysLeft;
          break;
        case 6:
          currentState = PlayerState.FullSidewaysRight;
          break;
        case 7:
          currentState = PlayerState.FullSidewaysLeft;
          break;
      }

      AgentObstacles.CheckForCollision();
      updatePlayerSprite();
      movePlayer();

      if (transform.localPosition.x < -5.6f || transform.localPosition.x > 5.6f || transform.localPosition.y < -20f)
      {
        EndEpisode();
      }

      if (currentState == PlayerState.FullSidewaysLeft || currentState == PlayerState.FullSidewaysRight)
      {
        SetReward(-0.00001f);
      }

      foreach (float possibleReward in possibleRewards)
      {
        if (transform.localPosition.y < possibleReward && !claimedRewards.Contains(possibleReward))
        {
          claimedRewards.Add(possibleReward);
          SetReward(0.25f);
        }
      }

      // Crashed
      if (currentState == PlayerState.WipeOut)
      {
        EndEpisode();
      }

      float dist = Vector3.Distance(transform.position, Target.position);
      // Reached goal
      if (dist < 1f)
      {
        SetReward(1.0f);
        EndEpisode();
      }
    }

    public override void Heuristic(float[] actionsOut)
    {
      Vector3 mousePos = new Vector3(Input.mousePosition.x - Screen.width / 2, Input.mousePosition.y - Screen.height / 2, 0);

      // Choose sprite based on the angle of the mouse
      float angle = Vector3.Angle(mousePos, Vector3.down);
      switch (angle)
      {
        case float x when (x < 15):
          actionsOut[0] = 1;
          break;

        case float x when (x >= 15 && x < 40):
          actionsOut[0] = mousePos.x > 0 ? 2 : 3;
          break;

        case float x when (x >= 40 && x < 60):
          actionsOut[0] = mousePos.x > 0 ? 4 : 5;
          break;

        case float x when (x > 60):
          actionsOut[0] = mousePos.x > 0 ? 6 : 7;
          break;
      }

      print("cumulative speed: " + cumulativeSpeed);
    }
  }
}