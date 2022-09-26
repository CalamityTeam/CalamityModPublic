using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    // Photoviscerator left click splitting homing projectile
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
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.frame = Main.rand.Next(3);
                Projectile.localAI[0] = 1f;
                Projectile.netUpdate = true;
            }
            NPC potentialTarget = Projectile.Center.ClosestNPCAt(MaxTargetDistance);
            if (potentialTarget != null)
                Projectile.velocity = (Projectile.velocity * (HomingInertia - 1) + Projectile.SafeDirectionTo(potentialTarget.Center) * 16f) / HomingInertia;

            Projectile.rotation = Projectile.velocity.ToRotation();
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
                Vector2 spawnPosition = Projectile.Center + angle.ToRotationVector2().RotatedBy(Projectile.rotation) * new Vector2(10f, 6f);
                Dust dust = Dust.NewDustPerfect(spawnPosition, FrameToDustIDTable[Projectile.frame]);
                dust.velocity = Vector2.Zero;
                dust.scale = 0.5f;
                dust.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.ExoDebuffs();

        public override void OnHitPvp(Player target, int damage, bool crit) => target.ExoDebuffs();
    }
}
