using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class MegalodonShot : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public static int Lifetime = 600;

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 2;
            Projectile.alpha = 255;
            Projectile.timeLeft = Lifetime;
            Projectile.MaxUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (Main.rand.NextBool())
            {
                Gore bubble = Gore.NewGorePerfect(Projectile.GetSource_FromAI(), Projectile.position, Projectile.velocity * 0.2f + Main.rand.NextVector2Circular(1f, 1f), 411);
                bubble.timeLeft = 9 + Main.rand.Next(7);
                bubble.scale = Main.rand.NextFloat(0.6f, 1f);
                bubble.type = Main.rand.NextBool(3) ? 412 : 411;
            }
            if (Projectile.timeLeft <= Lifetime - 4)
            {
                Particle spark = new SparkParticle(Projectile.Center - Projectile.velocity * 2, -Projectile.velocity * 0.05f, false, 10, 1f, Color.RoyalBlue * 0.5f);
                GeneralParticleHandler.SpawnParticle(spark);
                if (Main.rand.NextBool(3))
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? 45 : 56, -Projectile.velocity.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.01f, 0.3f));
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(1.5f, 0.9f);
                }
            }
        }
        //We use TileCollide and Onhit instead of OnKill due to it having pierce
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.Center, Projectile.velocity, Projectile.width, Projectile.height);
            OnHitEffects();
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            // Bubbles
            for (int i = 0; i < 10; i++)
            {
                Gore bubble = Gore.NewGorePerfect(Projectile.GetSource_FromAI(), Projectile.position, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(60f)) * 0.3f, 411);
                bubble.timeLeft = 9 + Main.rand.Next(7);
                bubble.scale = Main.rand.NextFloat(0.6f, 1f);
                bubble.type = Main.rand.NextBool(3) ? 412 : 411;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<RiptideDebuff>(), 60);
            //Only increase damage of the rocket on the first hit
            if (Projectile.numHits == 0)
            {
                Player Owner = Main.player[Projectile.owner];
                Owner.Calamity().sharkGunDamageScaling++;
            }
            OnHitEffects();
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            {
                if (Projectile.numHits == 0)
                {
                    Player Owner = Main.player[Projectile.owner];
                    Owner.Calamity().sharkGunDamageScaling++;
                }
                target.AddBuff(ModContent.BuffType<RiptideDebuff>(), 60);
                OnHitEffects();
            }
        }

        private void OnHitEffects()
        {
            for (int i = 0; i < 4; ++i)
            {
                int bloodLifetime = Main.rand.Next(22, 25);
                float bloodScale = Main.rand.NextFloat(0.6f, 0.8f);
                Color bloodColor = Color.Lerp(Color.RoyalBlue * 0.7f, Color.DarkBlue, Main.rand.NextFloat());
                bloodColor = Color.Lerp(bloodColor, new Color(51, 22, 94), Main.rand.NextFloat(0.65f));

                if (Main.rand.NextBool(20))
                    bloodScale *= 2f;

                float randomSpeedMultiplier = Main.rand.NextFloat(1.25f, 2.25f);
                Vector2 bloodVelocity = Main.rand.NextVector2Unit() * 2 * randomSpeedMultiplier;
                bloodVelocity.Y -= 5f;
                BloodParticle blood = new BloodParticle(Projectile.Center, bloodVelocity, bloodLifetime, bloodScale, bloodColor);
                GeneralParticleHandler.SpawnParticle(blood);
            }
            for (int i = 0; i <= 2; i++)
            {
                LineParticle spark = new LineParticle(Projectile.Center, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(0.18f, 0.44f)) * Main.rand.NextFloat(0.4f, 1.5f), false, 8, 0.9f, Main.rand.NextBool() ? Color.RoyalBlue * 0.7f : Color.MediumBlue);
                GeneralParticleHandler.SpawnParticle(spark);
                LineParticle spark2 = new LineParticle(Projectile.Center, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.18f, -0.44f)) * Main.rand.NextFloat(0.4f, 1.5f), false, 8, 0.9f, Main.rand.NextBool() ? Color.RoyalBlue * 0.7f : Color.MediumBlue);
                GeneralParticleHandler.SpawnParticle(spark2);
            }
        }
    }
}
