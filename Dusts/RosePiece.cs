using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Dusts
{
	public class RosePiece : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.color = default;
            dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override bool Update(Dust dust)
        {
            dust.rotation += MathHelper.ToRadians(1.56f);

            if (!dust.noGravity)
                dust.velocity.Y = MathHelper.Clamp(dust.velocity.Y + 0.15f, -1.8f, 2.5f);

            Vector2 velocity = Vector2.UnitY.RotatedBy(dust.rotation) * new Vector2(2f, 0.5f);

            // Adjust the move angle if a tile has been hit.
            if (velocity != Collision.TileCollision(dust.position, velocity, (int)(dust.scale * 4f), (int)(dust.scale * 4f)))
                dust.rotation = -1f;

            dust.position += velocity + dust.velocity;

            // Fade away.
            dust.scale = MathHelper.Clamp(dust.scale - 0.01f, 0f, 4f) * 0.99f;
            if (dust.scale < 0.4f)
                dust.active = false;

            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) => lightColor;
    }
}