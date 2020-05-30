using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class BrimseekerAuraBall : ModProjectile
    {
        public static float MovementSpeed = 4f;
        public static float RotationalSpeed = 0.08f;
        public bool initialized = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seeker");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (!initialized)
            {
                projectile.rotation = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                projectile.localAI[1] = Main.rand.NextFloat(-RotationalSpeed, RotationalSpeed);
                initialized = true;
            }
            projectile.tileCollide = false;

            Projectile parent = Main.projectile[(int)projectile.ai[0]];

            if (parent.active)
            {
                Vector2 positionDelta = new Vector2(0, projectile.localAI[0]);
                positionDelta = positionDelta.RotatedBy(projectile.rotation);

                projectile.Center = parent.Center + positionDelta;
                projectile.rotation += projectile.localAI[1];

                if (projectile.timeLeft % 200 / MovementSpeed < 100 / MovementSpeed)
                {
                    projectile.localAI[0] += MovementSpeed;
                }
                else
                {
                    projectile.localAI[0] -= MovementSpeed;
                }
            }
            else
            {
                projectile.Kill();
            }

            int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity.Y = -0.15f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 240);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 240);
        }
    }
}
