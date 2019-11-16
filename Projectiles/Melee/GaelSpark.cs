using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class GaelSpark : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spark");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 180;
            projectile.alpha = 255;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.velocity.X = 0f;
            return false;
        }
        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.7f, 0f, 0f);
            projectile.velocity.Y += 0.3f;
            int dustIndex = Dust.NewDust(projectile.Center, 0, 0, DustID.Blood, 0f, 0f, 0, default, 1.4f);
            Main.dust[dustIndex].noGravity = true;
        }
    }
}
