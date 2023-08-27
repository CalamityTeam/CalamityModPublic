using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class EclipsesFallMain : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/EclipsesFall";

        // these also affect KB
        public const float RainDamageMultMin = 0.4f;
        public const float RainDamageMultMax = 0.5f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.MaxUpdates = 2;
            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6 * Projectile.MaxUpdates;
            Projectile.timeLeft = 150 * Projectile.MaxUpdates;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 1f, 0.8f, 0.3f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            if (Main.rand.NextBool(5))
            {
                Vector2 trailPos = Projectile.Center + Vector2.UnitY.RotatedBy(Projectile.rotation) * Main.rand.NextFloat(-16f, 16f);
                float trailScale = Main.rand.NextFloat(0.8f, 1.2f);
                Color trailColor = Main.rand.NextBool() ? Color.Indigo : Color.DarkOrange;
                Particle eclipseTrail = new SparkParticle(trailPos, Projectile.velocity * 0.2f, false, 60, trailScale, trailColor);
                GeneralParticleHandler.SpawnParticle(eclipseTrail);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => SpawnSpears(target.Center);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => SpawnSpears(target.Center);

        private void SpawnSpears(Vector2 targetPos)
        {
            int spearAmt = Main.rand.Next(3, 5 + 1); // 3 to 5 spears
            var source = Projectile.GetSource_FromThis();
            for (int n = 0; n < spearAmt; n++)
            {
                float dmgMult = Main.rand.NextFloat(RainDamageMultMin, RainDamageMultMax);
                CalamityUtils.ProjectileRain(source, targetPos, 400f, 100f, 500f, 800f, 29f, ModContent.ProjectileType<EclipsesSmol>(), (int)(Projectile.damage * dmgMult), Projectile.knockBack * dmgMult, Projectile.owner);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
