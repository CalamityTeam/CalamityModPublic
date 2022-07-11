using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Dusts
{
    public class GasDust : ModDust
    {
        public override string Texture => "CalamityMod/Dusts/GasDust";
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.alpha = 210;
            dust.scale *= Main.rand.NextFloat(0.8f, 2f);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color color = lightColor.MultiplyRGB(dust.color * 0.995f);
            return color * 0.4f * ((255 - dust.alpha) / 255f);
        }

        public override bool Update(Dust dust)
        {
            dust.velocity *= 0.99f;
            dust.position += dust.velocity;
            dust.alpha += 3;
            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
    }
}
