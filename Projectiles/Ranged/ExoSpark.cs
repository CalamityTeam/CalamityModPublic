using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class ExoSpark : ModProjectile
    {
        public static readonly int[] FrameToDustIDTable = new int[]
        {
            107,
            234,
            269,
        };
        public const float HomingInertia = 10f;
        public const float MaxTargetDistance = 750f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exo Spark");
            Main.projFrames[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 8;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0f)
            {
                projectile.frame = Main.rand.Next(3);
                projectile.localAI[0] = 1f;
                projectile.netUpdate = true;
            }
            NPC potentialTarget = projectile.Center.ClosestNPCAt(MaxTargetDistance);
            if (potentialTarget != null)
                projectile.velocity = (projectile.velocity * (HomingInertia - 1) + projectile.SafeDirectionTo(potentialTarget.Center) * 16f) / HomingInertia;

            projectile.rotation = projectile.velocity.ToRotation();
            if (!Main.dedServ)
            {
                GenerateCircularDust();
            }
        }

        public void GenerateCircularDust()
        {
            for (int i = 0; i < 12; i++)
            {
                float angle = i / 12f * MathHelper.TwoPi;
                Vector2 spawnPosition = projectile.Center + angle.ToRotationVector2().RotatedBy(projectile.rotation) * new Vector2(10f, 6f);
                Dust dust = Dust.NewDustPerfect(spawnPosition, FrameToDustIDTable[projectile.frame]);
                dust.velocity = Vector2.Zero;
                dust.scale = 0.5f;
                dust.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 60);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 60);
            target.AddBuff(ModContent.BuffType<Plague>(), 60);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 60);
            target.AddBuff(BuffID.CursedInferno, 120);
            target.AddBuff(BuffID.Frostburn, 120);
            target.AddBuff(BuffID.OnFire, 120);
            target.AddBuff(BuffID.Ichor, 120);
        }
    }
}
