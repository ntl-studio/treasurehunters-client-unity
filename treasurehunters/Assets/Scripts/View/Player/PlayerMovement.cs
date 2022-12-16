using NtlStudio.TreasureHunters.Model;
using TreasureHunters;
using Unity.Mathematics;
using UnityEngine;
using Player = TreasureHunters.Player;

public class PlayerMovement : MonoBehaviour
{
    public bool AcceptInput = false;
    public Player Player;

    private bool _isMoving;
    private Vector3 _destination;
    private Vector3 _direction;

    private static GameClient Game => GameClient.Instance();
    
    void Start()
    {
        Debug.Assert(Player != null);

        UpdateView();
    }

    void Update()
    {
        if (AcceptInput)
            MovePlayer();
    }

    void FixedUpdate()
    {
        if (_isMoving)
        {
            const float movementSpeed = 5.0f;

            if (math.distancesq(transform.position, _destination) > 0.01f)
            {
                transform.position += _direction * Time.deltaTime * movementSpeed;
            }
            else
            {
                _isMoving = false;
                transform.position = _destination;

                // tell them game we finished the move when animation finished playing
                Game.EndMove(); 
            }
        }
    }

    public void UpdateView()
    {
        transform.position = new Vector3(Player.Position.X, Player.Position.Y);

        var states = Game.CurrentPlayerMoveStates;
        GameUtils.UpdateRotation(
            states.Count > 0 ? states[0].Direction : MoveDirection.None,
            transform);
    }

    private void MovePlayer()
    {
        if (_isMoving)
            return;

        PlayerAction playerAction = PlayerAction.None;

        // left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            var playerPos = transform.position;
            var clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (math.abs(clickPos.x - playerPos.x) < 1.5f &&
                math.abs(clickPos.y - playerPos.y) < 1.5f)
            {
                var shiftX = (int) math.round(clickPos.x - playerPos.x);
                var shiftY = (int) math.round(clickPos.y - playerPos.y);

                if (math.abs(shiftX) > math.abs(shiftY))
                    playerAction = shiftX > 0 ? PlayerAction.MoveRight : PlayerAction.MoveLeft;
                else
                    playerAction = shiftY > 0 ? PlayerAction.MoveUp: PlayerAction.MoveDown;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            playerAction = PlayerAction.MoveLeft;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            playerAction = PlayerAction.MoveUp;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            playerAction = PlayerAction.MoveRight;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            playerAction = PlayerAction.MoveDown;

        if (playerAction != PlayerAction.None)
        {
            if (Game.MakeTurn(playerAction))
            {
                Vector2Int shift = GameUtils.ActionToVector2(playerAction);

                _destination = new Vector3(transform.position.x + shift.x, transform.position.y + shift.y, 0);

                _direction = (_destination - transform.position).normalized;
                _isMoving = true;
                GameUtils.UpdateRotation(Game.CurrentPlayerMoveStates[0].Direction, transform);
            }
            else
            {
                Debug.Log("Could not move player");
            }
        }
    }
}