using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;


namespace CalamityMod.Dusts { 
    public class PumplerDust : ModDust
    {
        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "CalamityMod/Dusts/SmallSmoke";
            return true;
        }
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 24, 24);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color gray = new Color(40, 40, 40);
            Color ret;
            ret = Color.Lerp(Color.Orange, gray, MathHelper.Clamp((float)(dust.alpha - 100) / 80, 0f, 1f));
            return ret * ((255 - dust.alpha) / 255f);
        }

        public override bool Update(Dust dust)
        {
            dust.velocity *= 0.85f;

            if (dust.alpha < 165)
            {
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.1f);
                dust.scale += 0.01f;
                dust.alpha += 3;
            }
            else
            {
                dust.scale *= 0.975f;
                dust.alpha += 2;
            }

            dust.position += dust.velocity;
            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
    }
}