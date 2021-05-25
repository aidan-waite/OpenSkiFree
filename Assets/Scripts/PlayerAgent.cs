namespace OpenSkiFree
{
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;
  using Unity.MLAgents;
  using Unity.MLAgents.Sensors;

  public class PlayerAgent : Agent
  {
    public GameObject SuccessMarkerPrefab;
    public GameObject FailureMarkerPrefab;
    public AgentObstacles AgentObstacles;
    public Sprite PlayerFullSideways;
    public Sprite PlayerMostlySideways;
    public Sprite PlayerMostlyDown;
    public Sprite PlayerFullDown;
    public Sprite PlayerWipeOut;
    public Transform PlayerStartMin;
    public Transform PlayerStartMax;

    public SpriteRenderer PlayerSpriteRenderer;
    public Transform Target;
    public Transform LeftEdge;
    public Transform RightEdge;
    public Transform BottomEdge;

    public Transform Reward1;
    bool reward1Claimed = false;
    public Transform Reward2;
    bool reward2Claimed = false;
    public Transform Reward3;
    bool reward3Claimed = false;

    PlayerState currentState;
    bool completed = false;

    float playerSpriteScale = 2f;

    List<AgentLineRenderer> agentLines = new List<AgentLineRenderer>();

    public void didCollideWithObstacle()
    {
      handleFail("Crashed into obstacle");
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
      cumulativeSpeed = 0f;
      completed = false;
      AgentObstacles.SetupTrees();
      randomizeStartPosition();
      randomizeTarget();
      agentLines.Clear();
      for (int x = 1; x <= 8; x++)
      {
        agentLines.Add(transform.Find("RayTarget" + x).gameObject.GetComponent<AgentLineRenderer>());
      }
      reward1Claimed = false;
      reward2Claimed = false;
      reward3Claimed = false;
    }

    private void randomizeStartPosition()
    {
      float spawnX = Random.Range(PlayerStartMin.transform.localPosition.x, PlayerStartMax.transform.localPosition.x);
      float spawnY = PlayerStartMin.transform.localPosition.y;
      transform.localPosition = new Vector3(spawnX, spawnY, 0f);
    }

    private void randomizeTarget()
    {
      float x = Random.Range(LeftEdge.localPosition.x + 1f, RightEdge.localPosition.x - 1f);
      Target.localPosition = new Vector3(x, Target.localPosition.y, Target.localPosition.z);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
      sensor.AddObservation(Target.localPosition);
      sensor.AddObservation(transform.localPosition);

      foreach (AgentLineRenderer line in agentLines)
      {
        sensor.AddObservation(line.Distance);
      }
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

      if (transform.localPosition.x < LeftEdge.localPosition.x)
      {
        handleFail("Off left edge");
      }

      if (transform.localPosition.x > RightEdge.localPosition.x)
      {
        handleFail("Off right edge");
      }

      if (transform.localPosition.y < BottomEdge.localPosition.y)
      {
        handleFail("Off bottom edge");
      }

      if (!reward1Claimed && transform.localPosition.y < Reward1.localPosition.y)
      {
        reward1Claimed = true;
        SetReward(0.1f);
        showSuccessAtCurrentPosition();
      }

      if (!reward2Claimed && transform.localPosition.y < Reward2.localPosition.y)
      {
        reward2Claimed = true;
        SetReward(0.1f);
        showSuccessAtCurrentPosition();
      }

      if (!reward3Claimed && transform.localPosition.y < Reward3.localPosition.y)
      {
        reward3Claimed = true;
        SetReward(0.1f);
        showSuccessAtCurrentPosition();
      }

      float dist = Vector3.Distance(transform.position, Target.position);
      // Reached goal
      if (dist < 0.8f)
      {
        handleSuccess();
      }
    }

    void showSuccessAtCurrentPosition()
    {
      GameObject successMarker = Instantiate(SuccessMarkerPrefab);
      successMarker.transform.position = transform.position;
    }

    void handleFail(string reason)
    {
      if (completed == true)
      {
        //print("Skip fail");
        return;
      }

      print("end episode: " + reason);

      if (reason == "Crashed into obstacle")
      {
        SetReward(-0.1f);
      }

      completed = true;
      makeFailureMarkerAtCurrentPosition();
      EndEpisode();
    }

    void handleSuccess()
    {
      if (completed == true)
      {
        //print("Skip successs");
        return;
      }

      //print("Reached goal");
      completed = true;
      GameObject successMarker = Instantiate(SuccessMarkerPrefab);
      successMarker.transform.position = Target.transform.position;
      SetReward(1.0f);
      EndEpisode();
    }

    void makeFailureMarkerAtCurrentPosition()
    {
      GameObject failureMarker = Instantiate(FailureMarkerPrefab);
      failureMarker.transform.position = transform.position;
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
    }
  }
}