using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class ShadySalesmanGunshot : ModProjectile, ILocalizedModType
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public new string LocalizationCategory => "Projectiles.Rogue";

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 600;
            Projectile.npcProj = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 10;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;

        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.Khaki.ToVector3() * 0.2f);
        }

        public override void OnSpawn(IEntitySource source)
        {
            // Shift the projectile up to about the center of the salesman and override its speed
            if (source is EntitySource_Parent parentSource && parentSource.Entity is NPC npc && npc.type == ModContent.NPCType<ShadySalesman>())
            {
                Projectile.position.Y -= 4f;

                float rotation = npc.ai[2];
                Vector2 barrelDirection = rotation.ToRotationVector2();

                if (npc.spriteDirection == -1)
                    barrelDirection.X *= -1;

                float speed = Projectile.velocity.Length();
                Projectile.velocity = barrelDirection * speed;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SpawnImpactEffects(target.Center);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SpawnImpactEffects(Projectile.Center);
            return true;
        }

        public void SpawnImpactEffects(Vector2 hitPoint)
        {
            Vector2 velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX);

            Particle bloom = new CustomSpark(hitPoint, Vector2.Zero, "CalamityMod/Particles/BrightFlash", false, 6, 0.35f, Color.Khaki, Vector2.One, true, true, glowOpacity: 0.5f, colorFadeSpeed: 10);
            GeneralParticleHandler.SpawnParticle(bloom, true);

            for (int i = 0; i < 4; i++)
            {
                Vector2 dustVel = velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(2f, 7f);

                Dust dust = Dust.NewDustPerfect(hitPoint, DustID.Smoke, dustVel, 120, default, Main.rand.NextFloat(0.8f, 1.1f));
                dust.noGravity = true;
            }

            for (int i = 0; i < 3; i++)
            {
                Vector2 sparkVel = velocity.RotatedByRandom(0.22f) * Main.rand.NextFloat(3f, 8f);

                Particle spark = new CustomSpark(hitPoint, sparkVel, "CalamityMod/Particles/FadeLine", false, Main.rand.Next(12, 18), 0.22f, Color.LightGray, new Vector2(0.45f, 1f), true, shrinkSpeed: 0.45f, colorFadeSpeed: 10);
                GeneralParticleHandler.SpawnParticle(spark, true);
            }
        }
    }
}
