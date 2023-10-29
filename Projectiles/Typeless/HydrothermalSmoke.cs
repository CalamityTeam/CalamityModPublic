using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class HydrothermalSmoke : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 6;
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 6)
                Projectile.Center = Main.player[Projectile.owner].Center;

            int randomDust = Main.rand.Next(4);
            if (randomDust == 3)
            {
                randomDust = 16;
            }
            else
            {
                randomDust = 127;
            }
            if (Main.rand.NextBool(4))
            {
                int fieryDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDust, 0f, 0f, 100, default, 1f);
                if (Main.rand.NextBool(4))
                {
                    Main.dust[fieryDust].scale *= 0.35f;
                }
                Main.dust[fieryDust].velocity *= 0f;
            }

            Vector2 goreVec = new Vector2(Projectile.position.X, Projectile.position.Y);
            if (Main.rand.NextBool(8) && Main.netMode != NetmodeID.Server)
            {
                int smoke = Gore.NewGore(Projectile.GetSource_FromAI(), goreVec, default, Main.rand.Next(375, 378), 0.75f);
                Main.gore[smoke].behindTiles = true;
            }
        }

        public override bool? CanDamage() => false;
    }
}
