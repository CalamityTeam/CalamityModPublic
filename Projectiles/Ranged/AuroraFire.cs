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
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 3;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if (projectile.ai[0] % 2 == 0)
                projectile.ai[0]++;

            if (!initialized)
            {
                int pointCount = (int)projectile.ai[0];
                radianAmt = 180 + (int)(360 / (pointCount * 2));
                projectile.timeLeft = framesBeforeTurning * pointCount - 1;
                initialized = true;
            }

            //Velocity movement
            projectile.localAI[1]++;
            if (projectile.localAI[1] % framesBeforeTurning == 0)
            {
                projectile.velocity = projectile.velocity.RotatedBy(MathHelper.ToRadians(radianAmt));
            }

            //Dust behavior
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                    ModContent.DustType<AstralBlue>(),
                    ModContent.DustType<AstralOrange>()
                });
                for (int i = 0; i < 5; i++)
                {
                    int astral = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 0.8f);
                    Dust dust = Main.dust[astral];
                    dust.noGravity = true;
                    dust.velocity *= 0f;
                    dust.scale *= Main.rand.NextFloat();
                }
            }

            //rotation code for when sproot
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
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
