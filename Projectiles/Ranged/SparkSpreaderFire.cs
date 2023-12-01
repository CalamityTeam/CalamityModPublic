using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class SparkSpreaderFire : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public ref float Time => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 3;
            Projectile.MaxUpdates = 2;
            Projectile.timeLeft = 42 * Projectile.MaxUpdates;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            // Krill the fire when the water
            if (Projectile.wet && !Projectile.lavaWet)
            {
                Projectile.Kill();
                return;
            }

            Time++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Time >= 8f)
            {
                float cinderSize = Utils.GetLerpValue(6f, 12f, Time, true);
                Dust cinder = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<FinalFlame>(), Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 10, default, 0.75f);
                if (Main.rand.NextBool(3))
                {
                    cinder.scale *= 3f;
                    cinder.velocity *= 1.5f;
                }
                cinder.noGravity = true;
                cinder.scale *= cinderSize * 0.8f;
                cinder.velocity += Projectile.velocity;
            }
            else if (Time == 7f && !Main.rand.NextBool(4)) // Create short sparks around the tip at a pretty high chance
                SpawnSparks(3, true);
        }

        // On-impact sparks
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SpawnSparks(8);
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 60 * Main.rand.Next(1, 4));
            SpawnSparks(5);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 60 * Main.rand.Next(1, 4));
            SpawnSparks(5);
        }

        public void SpawnSparks(int count, bool shorts = false)
        {
            for (int i = 0; i < count; i++)
            {
                int sparkLifetime = Main.rand.Next(9, 16);
                float sparkScale = Main.rand.NextFloat(0.2f, 0.4f);
                Color sparkColor = Color.Lerp(Color.Gold, Color.OrangeRed, Main.rand.NextFloat(0.1f, 1f));

                if (Main.rand.NextBool(6))
                    sparkScale *= 1.5f;

                Vector2 sparkVelocity = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30f)) * Main.rand.NextFloat(1.5f, 3f);
                sparkVelocity.Y -= Main.rand.NextFloat(6f, 8f);
                SparkParticle spark = new SparkParticle(Projectile.Center, (sparkVelocity * (shorts ? 0.75f : 1f)), true, sparkLifetime, sparkScale, sparkColor);
                GeneralParticleHandler.SpawnParticle(spark);
            }
        }
    }
}
