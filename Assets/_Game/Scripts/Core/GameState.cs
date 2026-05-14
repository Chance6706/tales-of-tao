using System;

namespace TalesOfTao.Core
{
    /// <summary>
    /// Static game state flags accessible from any assembly.
    /// Used to coordinate between systems without creating assembly references.
    /// </summary>
    public static class GameState
    {
        /// <summary>
        /// True when the mouse is over a UI element that should block tile selection.
        /// </summary>
        public static bool IsMouseOverUI { get; set; }
    }
}
