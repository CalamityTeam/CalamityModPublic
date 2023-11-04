using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Particles;

namespace CalamityMod.Projectiles.Melee
{
    public class SaharaSlicersBolt : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public ref int Bolts => ref Main.player[Projectile.owner].Calamity().saharaSlicersBolts;
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            float playerDist = Vector2.Distance(Owner.Center, Projectile.Center);

            // Shot Mode
            if (Projectile.ai[0] == 1)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                Projectile.extraUpdates = 6;
                if (Main.rand.NextBool(2))
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5, 5), Main.rand.NextBool() ? 288 : 207);
                    dust.scale = Main.rand.NextFloat(0.2f, 0.45f);
                    dust.noGravity = true;
                    dust.velocity = -Projectile.velocity * 0.5f;
                }
                if (Projectile.timeLeft % 2 == 0 && playerDist < 1400f)
                {
                    SparkParticle spark = new SparkParticle(Projectile.Center - Projectile.velocity * 3f, -Projectile.velocity * 0.05f, false, 10, 1f, Color.White * 0.135f);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
            else // "Quiver" mode
            {
                // Refresh lifetime
                Projectile.timeLeft = 4;

                // If the player uses a bolt, destroy the most recent bolt
                if (Bolts < Projectile.ai[1])
                    Projectile.Kill();

                // Setting the bolt's position on the player's back
                Projectile.rotation = (21.8f - (Projectile.ai[1] * 0.1f)) * -Owner.direction;
                Vector2 BoltPos = Owner.MountedCenter + new Vector2((10 + Projectile.ai[1] * 2.5f) * -Owner.direction, 3f - Projectile.ai[1]);
                
                Projectile.Center = BoltPos;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i <= 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool() ? 288 : 207, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(0.3f, 1.9f));
                dust.noGravity = false;
                dust.scale = Main.rand.NextFloat(0.6f, 0.9f);
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool() ? 288 : 207, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(35f)) * Main.rand.NextFloat(0.05f, 0.9f));
                dust2.noGravity = false;
                dust2.scale = Main.rand.NextFloat(0.6f, 0.9f);
            }
        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[0] == 1)
            {
                SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
                for (int i = 0; i <= 5; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool() ? 216 : 207, -Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(0.2f, 1f));
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(1.1f, 1.8f);
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool() ? 216 : 207, -Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(35f)) * Main.rand.NextFloat(0.05f, 0.4f));
                    dust2.noGravity = true;
                    dust2.scale = Main.rand.NextFloat(1.1f, 1.8f);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] == 1)
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return true;
        }
        public override bool? CanDamage() => Projectile.ai[0] == 1 ? true : false;
    }
}
