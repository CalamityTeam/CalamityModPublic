using Microsoft.Xna.Framework;
using CalamityMod.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Buffs.StatDebuffs;

namespace CalamityMod.Projectiles.Turret
{
    public class IceShot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Shot");
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }


        public override void AI()
        {
            float fallSpeedCap = 15f;
            float downwardsAccel = 0.3f;
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.velocity.Y -= 3f;
                // play a sound frame 1.
                SoundEngine.PlaySound(SoundID.Item89 with { Volume = 0.4f }, Projectile.position);
            }
            Projectile.localAI[0]++;
            if (Projectile.velocity.Y < fallSpeedCap)
                Projectile.velocity.Y += downwardsAccel;
            if (Projectile.velocity.Y > fallSpeedCap)
                Projectile.velocity.Y = fallSpeedCap;
            Projectile.velocity.X *= 0.985f;
            Projectile.rotation += 0.1f * Projectile.velocity.X;
            DrawParticles();
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
            target.AddBuff(BuffID.Frostburn2, 180);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.oldVelocity.Y > 0f && Projectile.velocity.X != 0f)
            {
                Projectile.velocity.Y = -0.6f * Projectile.oldVelocity.Y;
                Projectile.velocity.X *= 0.975f;
            }   
            else if (Projectile.velocity.X == 0f)
            {
                Projectile.velocity.X = -0.6f * Projectile.oldVelocity.X;
            }
            return false;
        }
        public override Color? GetAlpha(Color drawColor)
        {
            if (Projectile.timeLeft < 30 && Projectile.timeLeft % 10 < 5)
                return Color.Orange;
            return Color.White;
        }
        public void DrawParticles()
        {
            Vector2 bloodSpawnPosition = Projectile.Center + (Vector2.UnitY * -13f).RotatedBy(Projectile.rotation);
            Vector2 splatterDirection = -(Projectile.Center - bloodSpawnPosition).SafeNormalize(Vector2.UnitY);
            int bloodLifetime = Main.rand.Next(5, 8);
            float bloodScale = Main.rand.NextFloat(0.4f, 0.6f);
            Color bloodColor = Color.Lerp(Color.Cyan, Color.LightCyan, Main.rand.NextFloat());
            bloodColor = Color.Lerp(bloodColor, new Color(11, 64, 128), Main.rand.NextFloat(0.65f));

            if (Main.rand.NextBool(20))
                bloodScale *= 1.7f;

            Vector2 bloodVelocity = splatterDirection.RotatedByRandom(0.81f) * Main.rand.NextFloat(3f, 6f);
            bloodVelocity.Y -= 1f;
            BloodParticle blood = new BloodParticle(bloodSpawnPosition, bloodVelocity, bloodLifetime, bloodScale, bloodColor);
            GeneralParticleHandler.SpawnParticle(blood);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/NPCHit/CryogenHit", 3) with { Volume = 0.55f }, Projectile.Center);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0f, ModContent.ProjectileType<IceExplosion>(), (int)(Projectile.damage * 0.25f), Projectile.knockBack, Main.myPlayer);
        }
    }
}
