using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Tiles.Abyss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static tModPorter.ProgressUpdate;
namespace CalamityMod.Projectiles.Ranged
{
    public class SeaDragonRocket : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetStaticDefaults() => ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        public ref float time => ref Projectile.ai[0];
        private bool beginStretchAnim = false;
        private float progress = -1;
        public bool attacking => Projectile.ai[1] == 5; //  If the missile is launched at the enemy
        public ref float moveSpeed => ref Projectile.ai[2]; // Some speed variation applied to the missiles based on spawn order
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.scale = 0.25f;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.Cyan.ToVector3() * 0.5f);
            Player Owner = Main.player[Projectile.owner];
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            
            if (attacking) // Fly at the enemy
            {
                NPC chosenTarget = Projectile.Center.ClosestNPCAt(600);
                CalamityUtils.HomeInOnSelectedNPC(Projectile, chosenTarget, true, 1.4f + moveSpeed, 25, 0.985f);
                if (chosenTarget != null)
                    Projectile.timeLeft++; // Don't die if you have a target to home in on
            }
            else // Swarm around the player until ready to be fired
            {
                Vector2 circle = Owner.Center + new Vector2(0, -195 + moveSpeed * 25).RotatedBy(time * 0.05f);
                Vector2 moveToEnemy = (circle - Projectile.Center).SafeNormalize(Vector2.UnitX);
                if (Projectile.velocity.Length() < 12)
                    Projectile.velocity = Projectile.velocity * 0.97f + moveToEnemy * moveSpeed;
                else
                    Projectile.velocity *= 0.9f;
                if (time % 2 == 0)
                    Projectile.netUpdate = true;
            }
            if (Main.rand.NextBool(10))
            {
                Dust trailDust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5, 5) - Projectile.velocity, 66);
                trailDust.scale = Main.rand.NextFloat(0.7f, 0.85f);
                trailDust.velocity = -Projectile.velocity * Main.rand.NextFloat(0.3f, 0.5f);
                trailDust.color = Main.rand.NextBool() ? Color.AliceBlue : Color.SkyBlue;
                trailDust.noGravity = true;
                if (attacking)
                {
                    Particle trail = new SparkParticle(Projectile.Center + Main.rand.NextVector2Circular(10, 10), -Projectile.velocity * Main.rand.NextFloat(0.2f, 0.8f), false, 15, Main.rand.NextFloat(0.6f, 0.8f), Main.rand.NextBool(3) ? Color.SeaGreen : Color.SkyBlue);
                    GeneralParticleHandler.SpawnParticle(trail);
                }
            }
            if (Main.rand.NextBool(8) && !attacking)
            {
                Particle Star = new CritSpark(Projectile.Center, -Projectile.velocity * Main.rand.NextFloat(0.3f, 0.5f), Color.SkyBlue, Main.rand.NextBool(3) ? Color.SeaGreen : Color.SkyBlue, Main.rand.NextFloat(0.4f, 0.7f), 30, 0.1f, 3f);
                GeneralParticleHandler.SpawnParticle(Star);
            }

            if (progress >= 0f && progress < 1f) // For spawn animation
                progress += 0.04f;

            if (Projectile.scale < 1f)
                Projectile.scale += 0.05f;

            time++;

            if (Owner.ownedProjectileCounts[ModContent.ProjectileType<SeadragonHoldout>()] <= 0 && !attacking) // If no holdout exists and not attacking then die
                Projectile.Kill();
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 velocity = (MathHelper.TwoPi * i / 15f).ToRotationVector2() * 3f;
                Dust ring = Dust.NewDustPerfect(Projectile.Center, 66, velocity);
                ring.noGravity = true;
                ring.scale = Main.rand.NextFloat(0.5f, 0.7f);
                ring.color = Color.CornflowerBlue;
            }
            beginStretchAnim = true;
        }

        public override void OnKill(int timeLeft)
        {
            // Explode on kill if attacking, else just poof out
            if (attacking)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath19, Projectile.position);
                SoundStyle hitSound2 = new("CalamityMod/Sounds/NPCHit/AnahitaHit", 3);
                SoundEngine.PlaySound(hitSound2 with { Volume = 1.1f }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode.WithPitchOffset(0.8f) with { Volume = 1.1f , MaxInstances = 2}, Projectile.position);
                for (int i = 0; i < 3; i++)
                {
                    Particle Star = new CritSpark(Projectile.Center, Vector2.One.RotatedByRandom(100) * Main.rand.NextFloat(4, 5), Color.SkyBlue, Main.rand.NextBool() ? Color.HotPink : Color.SeaGreen, Main.rand.NextFloat(0.6f, 0.9f), 30, 0.4f, 3f);
                    GeneralParticleHandler.SpawnParticle(Star);
                }
                int points = 4;
                float radians = MathHelper.TwoPi / points;
                Vector2 spinningPoint = Vector2.Normalize(new Vector2(-1f, -1f)).RotatedByRandom(100);
                Color useColor = Main.rand.NextBool() ? Color.SkyBlue : Color.HotPink;
                for (int k = 0; k < points; k++)
                {
                    Vector2 velocity = spinningPoint.RotatedBy(radians * k).RotatedBy(-0.45f);
                    Particle spark = new GlowSparkParticle((Projectile.Center + velocity * 7.5f), velocity * 0.5f, false, 11, 0.03f, useColor, new Vector2(1f, 0.4f), true, false);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), Vector2.One.RotatedByRandom(100) * Main.rand.NextFloat(4, 6));
                    dust.noGravity = false;
                    dust.scale = 0.8f;
                    dust.color = Color.AliceBlue;
                    dust.noLightEmittence = true;
                }
            }
        }
        public override bool? CanDamage() => (attacking ? null : false); // Can't hit if not attacking


        public override bool PreDraw(ref Color lightColor)
        {
            if (beginStretchAnim)
            {
                progress = 0f;
                beginStretchAnim = false;
            }

            if (progress >= 0f) // Squash n' stretch on spawn
            {
                float stretchFactorX, stretchFactorY;

                if (progress < 0.5f) // Stretch
                {
                    float completion = progress / 0.5f;
                    stretchFactorX = MathHelper.Lerp(1.6f, 0.7f, completion);
                    stretchFactorY = MathHelper.Lerp(0.7f, 1.6f, completion);
                }
                else // Squash
                {
                    float completion = (progress - 0.5f) / 0.5f;
                    stretchFactorX = MathHelper.Lerp(0.7f, 1f, completion);
                    stretchFactorY = MathHelper.Lerp(1.6f, 1f, completion);
                }

                if (progress >= 1f)
                {
                    stretchFactorX = 1f;
                    stretchFactorY = 1f;
                    progress = -1f;
                }

                Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
                Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
                Vector2 finalScale = new Vector2(stretchFactorX, stretchFactorY) * Projectile.scale;
                Vector2 finalPos = Projectile.Center - Main.screenPosition;

                Main.spriteBatch.Draw(texture, finalPos, null, lightColor, Projectile.rotation, origin, finalScale, SpriteEffects.None, 0f);

                return false;
            }

            Texture2D mainTexture = ModContent.Request<Texture2D>(Texture).Value;
            SpriteEffects spriteEffects = SpriteEffects.None;
            Vector2 drawOrigin = mainTexture.Size() / 2f;
            float drawScale = Projectile.scale;
            float drawRotation = Projectile.rotation;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.Draw(mainTexture, drawPosition, null, lightColor * (1f - Projectile.alpha / 255f), drawRotation, drawOrigin, drawScale, spriteEffects, 0f);

            return false;
        }


    }
}
