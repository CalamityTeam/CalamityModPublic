using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class BrimseekerAuraBall : ModProjectile
    {
        public bool Initialized = false;
        public Projectile ParentProjectile => CalamityUtils.FindProjectileByIdentity((int)projectile.ai[0], projectile.owner);
        public float Outwardness
        {
            get => projectile.localAI[0];
            set => projectile.localAI[0] = value;
        }
        public float RotationalSpeed
        {
            get => projectile.localAI[1];
            set => projectile.localAI[1] = value;
        }
        public const float OutwardnessMovementStep = 4f;
        public const float MaxRotationalSpeed = 0.08f;

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
            if (!Initialized)
            {
                projectile.rotation = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                RotationalSpeed = Main.rand.NextFloat(-MaxRotationalSpeed, MaxRotationalSpeed);
                Initialized = true;
            }

            if (ParentProjectile is null)
            {
                projectile.Kill();
                return;
            }

            Vector2 offsetRelativeToTarget = Vector2.UnitY.RotatedBy(projectile.rotation) * Outwardness;

            projectile.Center = ParentProjectile.Center + offsetRelativeToTarget;
            projectile.rotation += RotationalSpeed;

            if (projectile.timeLeft % 200 / OutwardnessMovementStep < 100 / OutwardnessMovementStep)
                Outwardness += OutwardnessMovementStep;
            else
                Outwardness -= OutwardnessMovementStep;

            Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 1.5f);
            dust.noGravity = true;
            dust.velocity.Y = -0.15f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
        }
    }
}
