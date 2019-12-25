using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public abstract class LoreItem : ModItem
    {
        public override void Update(ref float gravity, ref float maxFallSpeed) => gravity = maxFallSpeed = 0f;
        public override Color? GetAlpha(Color lightColor) => Color.White;
    }
}
