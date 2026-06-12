using System.Composition.Hosting.Core;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class BlahajBlast : ModProjectile, ILocalizedModType
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
            Projectile.penetrate = 4;
            Projectile.alpha = 255;
            Projectile.timeLeft = Lifetime;
            Projectile.MaxUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.tileCollide = false;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(3))
            {
                Gore bubble = Gore.NewGorePerfect(Projectile.GetSource_FromAI(), Projectile.position, Projectile.velocity * 0.2f + Main.rand.NextVector2Circular(1f, 1f), 411);
                bubble.timeLeft = 9 + Main.rand.Next(7);
                bubble.scale = Main.rand.NextFloat(0.6f, 1f);
                bubble.type = Main.rand.NextBool(3) ? 412 : 411;
            }
            if (Projectile.timeLeft <= Lifetime - 4)
            {
                if (Projectile.ai[0] == 0)
                {
                    Particle transspark = new SparkParticle(Projectile.Center - Projectile.velocity * 2, -Projectile.velocity * 0.05f, false, 10, 1.1f, Main.rand.NextBool() ? (Main.rand.NextBool() ? Color.DeepSkyBlue : Color.White) : Color.DeepPink);
                    GeneralParticleHandler.SpawnParticle(transspark);
                }
                else
                {
                    Particle spark = new SparkParticle(Projectile.Center - Projectile.velocity * 2, -Projectile.velocity * 0.05f, false, 10, 1.1f, Color.Aquamarine);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                if (Main.rand.NextBool(8))
                {
                    MediumMistParticle smoke = new MediumMistParticle(Projectile.Center + Main.rand.NextVector2Circular(25, 25), -Projectile.velocity * 0.05f, Main.rand.NextBool(3) ? Color.SeaGreen : Color.SkyBlue, Color.DarkBlue, 0.5f, 180, 3f);
                    GeneralParticleHandler.SpawnParticle(smoke);
                }
            }
            if (Projectile.timeLeft <= Lifetime - 8)
            {
                if (Main.rand.NextBool(2))
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15, 15), ModContent.DustType<LightDust>(), -Projectile.velocity);
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(0.9f, 1.1f);
                    dust.color = Main.rand.NextBool() ? Color.DarkBlue : Color.DarkBlue * 0.5f;

                    Particle trail = new SparkParticle(Projectile.Center + Main.rand.NextVector2Circular(10, 10), -Projectile.velocity * Main.rand.NextFloat(0.2f, 0.8f), false, 30, Main.rand.NextFloat(0.4f, 0.6f), Color.RoyalBlue);
                    GeneralParticleHandler.SpawnParticle(trail);
                }
            }
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
            //Only spawn rocket on the first hit
            if (Projectile.numHits == 0 && Main.myPlayer == Projectile.owner)
            {
                Player Owner = Main.player[Projectile.owner];
                Projectile fishy = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center + Main.rand.NextVector2Circular(150, 150), Vector2.Zero, ModContent.ProjectileType<SeaDragonRocket>(), (int)(Projectile.damage * 1.5f), Projectile.knockBack, Projectile.owner);
                fishy.ai[2] = ((Owner.Calamity().sharkGunDamageScaling + 1) * 0.02f) + 0.1f;
            }
            OnHitEffects();
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            {
                target.AddBuff(ModContent.BuffType<RiptideDebuff>(), 60);
                //Only spawn rocket on the first hit
                if (Projectile.numHits == 0 && Main.myPlayer == Projectile.owner)
                {
                    Player Owner = Main.player[Projectile.owner];
                    Projectile fishy = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center + Main.rand.NextVector2Circular(150, 150), Vector2.Zero, ModContent.ProjectileType<SeaDragonRocket>(), (int)(Projectile.damage * 1.5f), Projectile.knockBack, Projectile.owner);
                    fishy.ai[2] = ((Owner.Calamity().sharkGunDamageScaling + 1) * 0.02f) + 0.1f;
                }
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
                LineParticle spark = new LineParticle(Projectile.Center, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(0.18f, 0.44f)) * Main.rand.NextFloat(0.4f, 1.5f), false, 8, 0.9f, Main.rand.NextBool() ? Color.CornflowerBlue : Color.RoyalBlue);
                GeneralParticleHandler.SpawnParticle(spark);
                LineParticle spark2 = new LineParticle(Projectile.Center, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.18f, -0.44f)) * Main.rand.NextFloat(0.4f, 1.5f), false, 8, 0.9f, Main.rand.NextBool() ? Color.CornflowerBlue : Color.RoyalBlue);
                GeneralParticleHandler.SpawnParticle(spark2);
            }
        }
    }
}
