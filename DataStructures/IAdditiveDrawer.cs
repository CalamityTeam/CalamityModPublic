using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.DataStructures
{

    // Restarting the sprite batch for each different projectile is analogously equivalent to creating an entire new factory just to build a single car.
    // It is inefficient and leads to performance problems as many projectiles do it.
    // To mitigate this problem, a special interface exists for the purpose of deferring drawing of certain entities to a time in which they all may be
    // subject to the same additive draw state before all of the spritebatch's contents are flushed.
    // Manual drawing hooks should not be utilized when using this interface, such as ModNPC.PreDraw.

    // The implementation of this is almost identical to that of SLR but they're both so simple that it really shouldn't matter.
    public interface IAdditiveDrawer
    {
        void AdditiveDraw(SpriteBatch spriteBatch);
    }
}
