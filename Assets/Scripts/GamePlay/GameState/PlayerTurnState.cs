using UnityEngine;

namespace FishONU.GamePlay.GameState
{
    public class PlayerTurnState : GameState
    {
        protected override void OnServerEnter(GameStateManager manager)
        {
            base.OnServerEnter(manager);

            // 重设所有玩家的 turn 状态
            var currentPlayer = manager.GetCurrentPlayer();
            foreach (var player in manager.players)
            {
                player.isOwnersTurn = player.guid == currentPlayer.guid;
            }

            Debug.Log($"PlayerTurnState: {currentPlayer.guid}({currentPlayer.displayName})");
        }
    }
}