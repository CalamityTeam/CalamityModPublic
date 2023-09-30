using CalamityMod.Events;
using CalamityMod.Skies;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.NPCs.ExoMechs.Ares;

namespace CalamityMod.Projectiles.Boss
{
    public class AresGaussNukeProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        private const int timeLeft = 180;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 12;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            CooldownSlot = ImmunityCooldownID.Bosses;
            Projectile.timeLeft = timeLeft;

            if (Main.getGoodWorld)
                Projectile.extraUpdates = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (Projectile.position.Y > Projectile.ai[1])
                Projectile.tileCollide = true;

            // Animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= 12)
                Projectile.frame = 0;

            // Rotation
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) - MathHelper.PiOver2;

            // Difficulty modes
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool expertMode = Main.expertMode || bossRush;

            // Spawn effects
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                for (int i = 0; i < 25; i++)
                {
                    // Choose a random speed and angle
                    float dustSpeed = Main.rand.NextFloat(3f, 13f);
                    float angleRandom = 0.06f;
                    Vector2 dustVel = new Vector2(dustSpeed, 0f).RotatedBy(Projectile.velocity.ToRotation());
                    dustVel = dustVel.RotatedBy(-angleRandom);
                    dustVel = dustVel.RotatedByRandom(2f * angleRandom);

                    // Random size
                    float scale = Main.rand.NextFloat(0.5f, 1.6f);

                    // Spawn dust
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, 107, -dustVel, 0, default, scale);
                    dust.noGravity = true;
                }

                // Gauss sparks
                if (Main.myPlayer == Projectile.owner)
                {
                    int totalProjectiles = bossRush ? 18 : 12;
                    float radians = MathHelper.TwoPi / totalProjectiles;
                    int type = ModContent.ProjectileType<AresGaussNukeProjectileSpark>();
                    float velocity = Projectile.velocity.Length();
                    double angleA = radians * 0.5;
                    double angleB = MathHelper.ToRadians(90f) - angleA;
                    float velocityX2 = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
                    Vector2 spinningPoint = Main.rand.NextBool() ? new Vector2(0f, -velocity) : new Vector2(-velocityX2, -velocity);
                    for (int k = 0; k < totalProjectiles; k++)
                    {
                        Vector2 velocity2 = spinningPoint.RotatedBy(radians * k);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity2 + Vector2.Normalize(Projectile.velocity) * -6f, type, (int)Math.Round(Projectile.damage * 0.5), 0f, Main.myPlayer);
                    }
                }
            }

            // Light
            Lighting.AddLight(Projectile.Center, 0.2f, 0.25f, 0.05f);

            // Get a target and calculate distance from it
            int target = Player.FindClosest(Projectile.Center, 1, 1);
            Vector2 distanceFromTarget = Main.player[target].Center - Projectile.Center;

            // Set AI to stop homing, start accelerating
            float stopHomingDistance = bossRush ? 260f : death ? 280f : revenge ? 290f : expertMode ? 300f : 320f;
            if ((distanceFromTarget.Length() < stopHomingDistance && Projectile.ai[0] != -1f) || Projectile.ai[0] == 1f)
            {
                Projectile.ai[0] = 1f;

                if (Projectile.velocity.Length() < 24f)
                    Projectile.velocity *= 1.025f;

                return;
            }

            // Home in on target
            float scaleFactor = Projectile.velocity.Length();
            float inertia = bossRush ? 6f : death ? 8f : revenge ? 9f : expertMode ? 10f : 12f;
            distanceFromTarget.Normalize();
            distanceFromTarget *= scaleFactor;
            Projectile.velocity = (Projectile.velocity * inertia + distanceFromTarget) / (inertia + 1f);
            Projectile.velocity.Normalize();
            Projectile.velocity *= scaleFactor;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.EnterShaderRegion();
            Texture2D telegraphBase = ModContent.Request<Texture2D>("CalamityMod/Projectiles/InvisibleProj").Value;

            GameShaders.Misc["CalamityMod:CircularAoETelegraph"].UseOpacity(0.2f * MathHelper.Clamp((1 - Projectile.timeLeft / (float)timeLeft) * 8f, 0, 1));
            GameShaders.Misc["CalamityMod:CircularAoETelegraph"].UseColor(Color.Lerp(Color.Goldenrod, Color.Gold, 0.7f * (float)Math.Pow(0.5 + 0.5 * Math.Sin(Main.GlobalTimeWrappedHourly), 3)));
            GameShaders.Misc["CalamityMod:CircularAoETelegraph"].UseSecondaryColor(Color.Lerp(Color.Yellow, Color.White, 0.5f));
            GameShaders.Misc["CalamityMod:CircularAoETelegraph"].UseSaturation(1 - Projectile.timeLeft / (float)timeLeft);

            GameShaders.Misc["CalamityMod:CircularAoETelegraph"].Apply();

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(telegraphBase, drawPosition, null, lightColor, 0, telegraphBase.Size() / 2f, 1480f, 0, 0);
            Main.spriteBatch.ExitShaderRegion();

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int height = texture.Height / Main.projFrames[Projectile.type];
            int drawStart = height * Projectile.frame;
            Vector2 origin = Projectile.Size / 2;
            Main.EntitySpriteDraw(ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/AresGaussNukeProjectileGlow").Value, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, drawStart, texture.Width, height)), Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 45f, targetHitbox);

        public override void OnKill(int timeLeft)
        {
            // Nuke explosion sound.
            SoundEngine.PlaySound(AresGaussNuke.NukeExplosionSound, Projectile.Center);

            if (Main.netMode != NetmodeID.Server)
            {
                // Nuke gores
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, Projectile.velocity, Mod.Find<ModGore>("AresGaussNuke1").Type, 1f);
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, Projectile.velocity, Mod.Find<ModGore>("AresGaussNuke3").Type, 1f);
            }

            // Create a bunch of lightning bolts in the sky
            ExoMechsSky.CreateLightningBolt(12);

            if (Main.myPlayer == Projectile.owner && Projectile.ai[0] != -1f)
            {
                // Explosion waves
                for (int i = 0; i < 3; i++)
                {
                    Projectile explosion = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<AresGaussNukeProjectileBoom>(), Projectile.damage, 0f, Main.myPlayer);
                    if (explosion.whoAmI.WithinBounds(Main.maxProjectiles))
                    {
                        // Make the max explosion radius decrease over time, creating a ring effect.
                        explosion.ai[1] = 560f + i * 90f;
                        explosion.localAI[1] = 0.25f;
                        explosion.Opacity = MathHelper.Lerp(0.18f, 0.6f, i / 7f) + Main.rand.NextFloat(-0.08f, 0.08f);
                        explosion.netUpdate = true;
                    }
                }
            }
        }
    }
}
