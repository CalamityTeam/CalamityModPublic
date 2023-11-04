using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class Sandstream : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public int Time = 0;
        public bool PostHit = false;
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 2;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 240;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Time++;
            if (Time == 1)
            {
                for (int i = 0; i <= 10; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center,313, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(35f)) * Main.rand.NextFloat(0.2f, 1.2f), 0, default, Main.rand.NextFloat(1.3f, 1.7f));
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(1.7f, 2.3f);
                }
            }
            if (PostHit)
            {
                for (int i = 0; i < 4; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? 216 : 32);
                    dust.noGravity = true;
                    dust.velocity = new Vector2(0.5f, 0.5f).RotatedByRandom(100);
                    dust.scale = Main.rand.NextFloat(0.3f, 1.3f);
                }
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center, 313);
                dust2.noGravity = true;
                dust2.velocity = -Projectile.velocity * Main.rand.NextFloat(0.2f, 0.5f);
                dust2.scale = Main.rand.NextFloat(0.3f, 1.3f);
                Projectile.penetrate = 1;
                Projectile.velocity.Y += 0.1f;
                Projectile.velocity.X *= 0.97f;
                Projectile.extraUpdates = 3;
            }
            else
            {
                if (Main.rand.NextBool(5))
                {
                    MediumMistParticle SandCloud = new MediumMistParticle(Projectile.Center, (-Projectile.velocity * 0.2f).RotatedByRandom(0.2f), Color.Peru, Color.PeachPuff, MathHelper.Clamp(Main.rand.NextFloat(1.1f, 1.5f) - Time * 0.02f, 0.5f, 2), 120f, Main.rand.NextFloat(0.03f, -0.03f));
                    GeneralParticleHandler.SpawnParticle(SandCloud);
                }
                for (int i = 0; i < 4; i++)
                {
                    float DustArea = MathHelper.Clamp(3 - Time * 0.03f, 1, 3);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(DustArea, DustArea), Main.rand.NextBool(3) ? 216 : 32);
                    dust.noGravity = true;
                    dust.velocity = new Vector2(0.5f, 0.5f).RotatedByRandom(100) + Projectile.velocity * 0.3f;
                    dust.scale = MathHelper.Clamp(Main.rand.NextFloat(1.4f, 1.9f) - Time * 0.01f, 0.9f, 1.5f);
                }

                if (Main.rand.NextBool(4))
                {
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? 216 : 32);
                    dust2.noGravity = false;
                    dust2.velocity = new Vector2(0, Main.rand.NextFloat(1, 4));
                    dust2.scale = Main.rand.NextFloat(0.3f, 0.5f);
                    dust2.fadeIn = 0.7f;
                }
                Projectile.velocity.Y += 0.035f;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            PostHit = true;
            Projectile.timeLeft = 300;
            float numberOfDusts = 6f;
            float rotFactor = 360f / numberOfDusts;
            for (int i = 0; i < numberOfDusts; i++)
            {
                float rot = MathHelper.ToRadians(i * rotFactor);
                Vector2 offset = new Vector2(Main.rand.NextFloat(0.5f, 1.5f), 0).RotatedBy(rot * Main.rand.NextFloat(1.1f, 9.1f));
                Vector2 velOffset = new Vector2(Main.rand.NextFloat(0.5f, 1.5f), 0).RotatedBy(rot * Main.rand.NextFloat(1.1f, 9.1f));
                Dust dust = Dust.NewDustPerfect(target.Center + offset, Main.rand.NextBool(3) ? 288 : 207, new Vector2(velOffset.X, velOffset.Y));
                dust.noGravity = false;
                dust.velocity = velOffset;
                dust.scale = Main.rand.NextFloat(0.5f, 0.8f);
            }
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SandstreamScepterExplosion>(), Projectile.damage / 3, Projectile.knockBack * 4, Projectile.owner);
            float numberOfDusts = 20f;
            float rotFactor = 360f / numberOfDusts;
            for (int i = 0; i < numberOfDusts; i++)
            {
                float rot = MathHelper.ToRadians(i * rotFactor);
                Vector2 offset = new Vector2(Main.rand.NextFloat(1.5f, 5.5f), 0).RotatedBy(rot * Main.rand.NextFloat(3.1f, 9.1f));
                Vector2 velOffset = new Vector2(Main.rand.NextFloat(1.5f, 5.5f), 0).RotatedBy(rot * Main.rand.NextFloat(3.1f, 9.1f));
                MediumMistParticle SandCloud = new MediumMistParticle(Projectile.Center + offset, velOffset * Main.rand.NextFloat(1.5f, 3f), Color.Peru, Color.PeachPuff, Main.rand.NextFloat(0.9f, 1.2f), 160f, Main.rand.NextFloat(0.03f, -0.03f));
                GeneralParticleHandler.SpawnParticle(SandCloud);
                Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, Main.rand.NextBool() ? 288 : 207, new Vector2(velOffset.X, velOffset.Y));
                dust.noGravity = false;
                dust.velocity = velOffset;
                dust.scale = Main.rand.NextFloat(1.2f, 1.6f);
            }
        }
    }
}
