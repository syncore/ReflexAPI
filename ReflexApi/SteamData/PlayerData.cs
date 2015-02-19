// ReSharper disable InconsistentNaming
namespace ReflexApi.SteamData
{
    /// <summary>
    /// Model for Steam data that represents a player on a Reflex server.
    /// </summary>
    /// <remarks>
    /// Ignore ReSharper warnings about capitalization since JSON responses
    /// are not typically capitalized in APIs.
    /// </remarks>
    public class PlayerData
    {
        /// <summary>
        /// Gets or sets the duration of the player's connection to the server.
        /// </summary>
        /// <value>
        /// The duration of the player's connection to the server.
        /// </value>
        public double connectedFor { get; set; }

        /// <summary>
        /// Gets or sets the player's index on the server.
        /// </summary>
        /// <value>
        /// The player's index on the server.
        /// </value>
        //public int index { get; set; }

        /// <summary>
        /// Gets or sets the name of the player on the server.
        /// </summary>
        /// <value>
        /// The name of the player on the server.
        /// </value>
        public string name { get; set; }

        /// <summary>
        /// Gets or sets the score of the player on the server.
        /// </summary>
        /// <value>
        /// The score of the player on the server.
        /// </value>
        public int score { get; set; }
    }
}