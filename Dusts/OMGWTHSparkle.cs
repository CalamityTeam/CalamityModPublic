using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Dusts
{
    public class OMGWTHSparkle : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.velocity *= 0.1f;
            dust.alpha = 155;
            dust.noGravity = true;
            dust.noLight = true;
            dust.scale *= 0.25f;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.rotation += dust.velocity.X * 0.05f;
            dust.scale *= 0.99f;
            float light = 0.35f * dust.scale;
            Lighting.AddLight(dust.position, 0.35f, 0.1f, 0.1f);
            if (dust.scale < 0.15f)
            {
                dust.active = false;
            }
            return false;
        }
    }
}
