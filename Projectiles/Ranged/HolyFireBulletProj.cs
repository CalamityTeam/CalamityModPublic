using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Ammo;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class HolyFireBulletProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        private const int Lifetime = 600;
        private static readonly Color Alpha = new Color(1f, 1f, 1f, 0f);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.MaxUpdates = 5;
            Projectile.timeLeft = Lifetime;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            Projectile.spriteDirection = Projectile.direction;

            // Flaking dust
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 4f)
            {
                if (Main.rand.NextBool())
                {
                    float scale = Main.rand.NextFloat(0.6f, 1.6f);
                    int dustID = Dust.NewDust(Projectile.Center, 1, 1, 244);
                    Main.dust[dustID].position = Projectile.Center;
                    Main.dust[dustID].noGravity = true;
                    Main.dust[dustID].scale = scale;
                    float angleDeviation = 0.17f;
                    float angle = Main.rand.NextFloat(-angleDeviation, angleDeviation);
                    Vector2 sprayVelocity = Projectile.velocity.RotatedBy(angle) * 0.6f;
                    Main.dust[dustID].velocity = sprayVelocity;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) => Alpha;

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesFromEdge(Projectile, 0, lightColor);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            // Spawn an on-hit explosion which deals 75% of the projectile's damage.
            if (Projectile.owner == Main.myPlayer)
            {
                int blastDamage = (int)(Projectile.damage * HolyFireBullet.ExplosionMultiplier);
                float scale = 0.85f + Main.rand.NextFloat() * 1.15f;
                int boom = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FuckYou>(), blastDamage, Projectile.knockBack, Projectile.owner, 0f, scale);

                // Explosions match the bullet's damage type (e.g. ranged or summon)
                if (boom.WithinBounds(Main.maxProjectiles))
                    Main.projectile[boom].DamageType = Projectile.DamageType;
            }

            // Spawn four shrapnel dust. This deals no damage.
            for (int k = 0; k < 4; k++)
            {
                float scale = Main.rand.NextFloat(1.4f, 1.8f);
                int dustID = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 244);
                Main.dust[dustID].noGravity = false;
                Main.dust[dustID].scale = scale;
                float angleDeviation = 0.25f;
                float angle = Main.rand.NextFloat(-angleDeviation, angleDeviation);
                float velMult = Main.rand.NextFloat(0.08f, 0.14f);
                Vector2 shrapnelVelocity = Projectile.oldVelocity.RotatedBy(angle) * velMult;
                Main.dust[dustID].velocity = shrapnelVelocity;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 300);
        }
    }
}
