using CalamityMod.Particles;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Reflection.Metadata;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class RealityRuptureStealth : ModProjectile, ILocalizedModType
    {
        public static readonly SoundStyle Hitsound = new("CalamityMod/Sounds/Item/RealityRuptureStealthHit") { Volume = 1.2f, PitchVariance = 0.3f};
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/RealityRupture";
        public bool posthit = false;
        public int Time = 0;
        public override void SetDefaults()
        {
            Projectile.width = 43;
            Projectile.height = 43;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 900;
            Projectile.aiStyle = 0;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 50;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 7;
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            float playerDist = Vector2.Distance(Owner.Center, Projectile.Center);

            Time++;
            Projectile.velocity *= 1.003f;
            Projectile.scale = 1.3f;

            Vector3 DustLight = new Vector3(0.209f, 0.140f, 0.202f);
            Lighting.AddLight(Projectile.Center, DustLight * 8);

            if (Time > 10 && playerDist < 1400f)
            {
                SparkParticle spark3 = new SparkParticle(Projectile.Center - Projectile.velocity, Projectile.velocity * 0.01f, false, 18, 3.4f, Color.Plum * 0.6f);
                GeneralParticleHandler.SpawnParticle(spark3);
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(Hitsound, Projectile.position);
            posthit = true;
            for (int i = 0; i < 6; i++)
            {
                Vector2 vel = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(20f));
                float Scale = Main.rand.NextFloat(1.8f, 2.3f);
                CrackParticle crack = new CrackParticle(Projectile.Center, vel, Color.Orchid, new Vector2(1f, 1f), 0, Scale, Scale - 0.5f, Main.rand.Next(18, 23));
                GeneralParticleHandler.SpawnParticle(crack);
            }
            Vector2 vel2 = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(3f));
            float Scale2 = Main.rand.NextFloat(2.9f, 3.2f);
            CrackParticle crack2 = new CrackParticle(Projectile.Center, vel2, Color.Plum, new Vector2(1f, 1f), 0, Scale2, Scale2 - 0.5f, Main.rand.Next(27, 32));
            GeneralParticleHandler.SpawnParticle(crack2);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0f, ModContent.ProjectileType<SpearofDestinyStealthExplosion>(), Projectile.damage / 2, Projectile.knockBack * 2, Projectile.owner, 0f);
            Main.player[Projectile.owner].Calamity().GeneralScreenShakePower = 8;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;
            float scale = Projectile.scale;
            float rotation = Projectile.rotation;

            Rectangle rectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle, Color.White, rotation, origin, scale, SpriteEffects.None, 0);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i <= 15; i++)
            {
                Vector2 sparkVelocity = Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.6f, 1.5f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, 272, sparkVelocity.RotatedByRandom(0.1f), 0, default, Main.rand.NextFloat(1.2f, 1.5f));
                dust.noGravity = true;
                
                int sparkLifetime = Main.rand.Next(43, 48);
                float sparkScale = Main.rand.NextFloat(2.2f, 3f);
                Color sparkColor = Color.Plum * 0.8f;
                SparkParticle spark = new SparkParticle(Projectile.Center, sparkVelocity, false, sparkLifetime, sparkScale, sparkColor);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, Color.Plum, new Vector2(2f, 2f), Main.rand.NextFloat(12f, 25f), 0.1f, 1f, 13);
            GeneralParticleHandler.SpawnParticle(pulse);
        }
    }
}
