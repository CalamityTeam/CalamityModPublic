using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class BrimseekerAuraBall : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public bool Initialized = false;
        public Projectile ParentProjectile => CalamityUtils.FindProjectileByIdentity((int)Projectile.ai[0], Projectile.owner);
        public float Outwardness
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        public float RotationalSpeed
        {
            get => Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }
        public const float OutwardnessMovementStep = 4f;
        public const float MaxRotationalSpeed = 0.08f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            if (!Initialized)
            {
                Projectile.rotation = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                RotationalSpeed = Main.rand.NextFloat(-MaxRotationalSpeed, MaxRotationalSpeed);
                Initialized = true;
            }

            if (ParentProjectile is null)
            {
                Projectile.Kill();
                return;
            }

            Vector2 offsetRelativeToTarget = Vector2.UnitY.RotatedBy(Projectile.rotation) * Outwardness;

            Projectile.Center = ParentProjectile.Center + offsetRelativeToTarget;
            Projectile.rotation += RotationalSpeed;

            if (Projectile.timeLeft % 200 / OutwardnessMovementStep < 100 / OutwardnessMovementStep)
                Outwardness += OutwardnessMovementStep;
            else
                Outwardness -= OutwardnessMovementStep;

            Dust brimstoneFire = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 1.5f);
            brimstoneFire.noGravity = true;
            brimstoneFire.velocity.Y = -0.15f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
    }
}
