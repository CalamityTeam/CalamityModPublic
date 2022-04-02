using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod
{
    public static partial class CalamityUtils
    {
        /// <summary>
        /// Gets a unit direction towards an arbitrary destination for an entity based on its center. Has <see cref="float.NaN"/> safety in the form of a fallback vector.
        /// </summary>
        /// <param name="entity">The entity to check from.</param>
        /// <param name="destination">The destination to get the direction to.</param>
        /// <param name="fallback">A fallback value to use in the event of an unsafe normalization.</param>
        public static Vector2 SafeDirectionTo(this Entity entity, Vector2 destination, Vector2? fallback = null)
        {
            // Fall back to zero by default. default(Vector2) could be used in the parameter definition, but
            // this is more clear.
            if (!fallback.HasValue)
                fallback = Vector2.Zero;

            return (destination - entity.Center).SafeNormalize(fallback.Value);
        }
    }
}
