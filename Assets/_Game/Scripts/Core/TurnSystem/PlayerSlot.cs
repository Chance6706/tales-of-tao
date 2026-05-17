using UnityEngine;

namespace TalesOfTao.Core.TurnSystem
{
    /// <summary>
    /// Represents a player slot in a multiplayer game.
    /// One per player (human or AI).
    /// </summary>
    public class PlayerSlot
    {
        public int PlayerId;
        public string PlayerName;
        public bool IsAI;
        public bool IsReady;
        public bool IsConnected;
        public int SectConfigIndex;
        public Color PlayerColor;

        public PlayerSlot(int playerId, string playerName, bool isAI, Color color)
        {
            PlayerId = playerId;
            PlayerName = playerName;
            IsAI = isAI;
            IsReady = false;
            IsConnected = true;
            SectConfigIndex = 0;
            PlayerColor = color;
        }

        public override string ToString()
        {
            return $"[P{PlayerId}] {PlayerName} ({(IsAI ? "AI" : "Human")}, {(IsReady ? "Ready" : "Not Ready")})";
        }
    }
}
