using CalamityMod.Buffs.DamageOverTime;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class PlagueExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0f, 0.15f, 0f);

            float projTimer = 25f;
            if (Projectile.ai[0] > 180f)
            {
                projTimer -= (Projectile.ai[0] - 180f) / 2f;
            }
            if (projTimer <= 0f)
            {
                projTimer = 0f;
                Projectile.Kill();
            }
            projTimer *= 0.7f;

            Projectile.ai[0] += 4f;
            int timerCounter = 0;
            while (timerCounter < projTimer)
            {
                float rando1 = Main.rand.Next(-7, 8) * Projectile.scale;
                float rando2 = Main.rand.Next(-7, 8) * Projectile.scale;
                float rando3 = Main.rand.Next(2, 6) * Projectile.scale;
                float randoAdjuster = (float)Math.Sqrt(rando1 * rando1 + rando2 * rando2);
                randoAdjuster = rando3 / randoAdjuster;
                rando1 *= randoAdjuster;
                rando2 *= randoAdjuster;
                int greenPlague = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 89, 0f, 0f, 100, default);
                Main.dust[greenPlague].noGravity = true;
                Main.dust[greenPlague].position.X = Projectile.Center.X;
                Main.dust[greenPlague].position.Y = Projectile.Center.Y;
                Main.dust[greenPlague].position.X += Main.rand.Next(-10, 11);
                Main.dust[greenPlague].position.Y += Main.rand.Next(-10, 11);
                Main.dust[greenPlague].velocity.X = rando1;
                Main.dust[greenPlague].velocity.Y = rando2;
                Main.dust[greenPlague].scale = Projectile.scale * 0.35f;
                timerCounter++;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<Plague>(), 90);
        }
    }
}
