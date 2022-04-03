using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class AuroraFire : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private const int framesBeforeTurning = 70;
        private bool initialized = false;
        private int radianAmt = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aurora");
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 3;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if (Projectile.ai[0] % 2 == 0)
                Projectile.ai[0]++;

            if (!initialized)
            {
                int pointCount = (int)Projectile.ai[0];
                radianAmt = 180 + (int)(360 / (pointCount * 2));
                Projectile.timeLeft = framesBeforeTurning * pointCount - 1;
                initialized = true;
            }

            //Velocity movement
            Projectile.localAI[1]++;
            if (Projectile.localAI[1] % framesBeforeTurning == 0)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(radianAmt));
            }

            //Dust behavior
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 4f)
            {
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                    ModContent.DustType<AstralBlue>(),
                    ModContent.DustType<AstralOrange>()
                });
                for (int i = 0; i < 5; i++)
                {
                    int astral = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 0.8f);
                    Dust dust = Main.dust[astral];
                    dust.noGravity = true;
                    dust.velocity *= 0f;
                    dust.scale *= Main.rand.NextFloat();
                }
            }

            //rotation code for when sproot
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240);
        }
    }
}
