using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Dusts
{
    public class CeaselessDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
        }

        public override bool Update(Dust dust)
        {
            float scale = dust.scale;
            Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f), scale * 0.4f, scale * 0.1f, scale);

            dust.position += dust.velocity;
            dust.velocity *= 0.95f;
            dust.scale -= 0.05f;

            if (dust.scale <= 0.05f)
                dust.active = false;

            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            int newColor = (int)(250f * dust.scale);
            return new Color(newColor, newColor, newColor, 0);
        }
    }
}
