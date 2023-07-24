using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class ThanatosLaser : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public float TelegraphDelay
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public NPC ThingToAttachTo => Main.npc.IndexInRange((int)Projectile.ai[1]) ? Main.npc[(int)Projectile.ai[1]] : null;

        public Vector2 Destination;
        public Vector2 Velocity;
        public const float TelegraphTotalTime = 60f;
        public const float TelegraphFadeTime = 30f;
        public const float TelegraphWidth = 4200f;
        public const float LaserVelocity = 7.5f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 1200;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.extraUpdates);
            writer.WriteVector2(Destination);
            writer.WriteVector2(Velocity);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.extraUpdates = reader.ReadInt32();
            Destination = reader.ReadVector2();
            Velocity = reader.ReadVector2();
        }

        public override void AI()
        {
            // Determine the relative opacities for each player based on their distance.
            // This has a lower bound of 0.35 to prevent the laser from going completely invisible and players getting hit by cheap shots.
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                Projectile.netUpdate = true;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 12)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
                Projectile.frame = 0;

            // If there is no NPC to attach to, run this instead.
            if (Projectile.ai[1] == -1f)
            {
                // Fade in after telegraphs have faded.
                if (TelegraphDelay > TelegraphTotalTime)
                {
                    if (Projectile.alpha > 0)
                        Projectile.alpha -= 25;
                    if (Projectile.alpha < 0)
                        Projectile.alpha = 0;

                    // If a velocity is in reserve, set the true velocity to it and make it as "taken" by setting it to <0,0>
                    if (Velocity != Vector2.Zero)
                    {
                        Projectile.extraUpdates = Main.getGoodWorld ? 4 : 3;
                        Projectile.velocity = Velocity * (BossRushEvent.BossRushActive ? 1.25f : 1f);
                        Velocity = Vector2.Zero;
                        Projectile.netUpdate = true;
                    }

                    // Direction and rotation.
                    if (Projectile.velocity.X < 0f)
                    {
                        Projectile.spriteDirection = -1;
                        Projectile.rotation = (float)Math.Atan2((double)-(double)Projectile.velocity.Y, (double)-(double)Projectile.velocity.X);
                    }
                    else
                    {
                        Projectile.spriteDirection = 1;
                        Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X);
                    }
                }
                else if (Velocity == Vector2.Zero)
                {
                    Velocity = Projectile.velocity;
                    Projectile.velocity = Vector2.Zero;
                    Projectile.netUpdate = true;

                    // Direction and rotation.
                    if (Projectile.velocity.X < 0f)
                    {
                        Projectile.spriteDirection = -1;
                        Projectile.rotation = (float)Math.Atan2((double)-(double)Velocity.Y, (double)-(double)Velocity.X);
                    }
                    else
                    {
                        Projectile.spriteDirection = 1;
                        Projectile.rotation = (float)Math.Atan2((double)Velocity.Y, (double)Velocity.X);
                    }
                }

                TelegraphDelay++;

                return;
            }

            // Die if the thing to attach to disappears.
            if (ThingToAttachTo is null || !ThingToAttachTo.active)
            {
                Projectile.Kill();
                return;
            }

            // If the Ares Laser Cannon is the owner.
            bool aresLaserIsOwner = ThingToAttachTo.type == ModContent.NPCType<AresLaserCannon>();

            // Fade in after telegraphs have faded.
            if (TelegraphDelay > TelegraphTotalTime)
            {
                if (Projectile.alpha > 0)
                    Projectile.alpha -= 25;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;

                // If a velocity is in reserve, set the true velocity to it and make it as "taken" by setting it to <0,0>
                if (Velocity != Vector2.Zero)
                {
                    Projectile.extraUpdates = Main.getGoodWorld ? 4 : 3;
                    Projectile.velocity = Velocity * (BossRushEvent.BossRushActive ? 1.25f : 1f);
                    Velocity = Vector2.Zero;
                    Projectile.netUpdate = true;
                }

                // Direction and rotation.
                if (Projectile.velocity.X < 0f)
                {
                    Projectile.spriteDirection = -1;
                    Projectile.rotation = (float)Math.Atan2((double)-(double)Projectile.velocity.Y, (double)-(double)Projectile.velocity.X);
                }
                else
                {
                    Projectile.spriteDirection = 1;
                    Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X);
                }
            }
            else if (Destination == Vector2.Zero)
            {
                // Set start of telegraph to the npc center.
                Projectile.Center = ThingToAttachTo.Center;

                // Set destination of the laser, the target's center.
                Destination = Projectile.velocity;

                if (aresLaserIsOwner)
                    Projectile.Center += Vector2.Normalize(Destination - ThingToAttachTo.Center) * 70f + Vector2.UnitY * 16f;

                // Calculate and store the velocity that will be used for laser telegraph rotation and beam firing.
                Vector2 projectileDestination = Destination - ThingToAttachTo.Center;
                Velocity = Vector2.Normalize(projectileDestination) * LaserVelocity;

                // Set velocity to zero.
                Projectile.velocity = Vector2.Zero;
                Projectile.netUpdate = true;

                // Direction and rotation.
                if (Projectile.velocity.X < 0f)
                {
                    Projectile.spriteDirection = -1;
                    Projectile.rotation = (float)Math.Atan2((double)-(double)Velocity.Y, (double)-(double)Velocity.X);
                }
                else
                {
                    Projectile.spriteDirection = 1;
                    Projectile.rotation = (float)Math.Atan2((double)Velocity.Y, (double)Velocity.X);
                }
            }
            else
            {
                // Set start of telegraph to the npc center.
                Projectile.Center = ThingToAttachTo.Center;

                if (aresLaserIsOwner)
                    Projectile.Center += Vector2.Normalize(Destination - ThingToAttachTo.Center) * 70f + Vector2.UnitY * 16f;

                // Calculate and store the velocity that will be used for laser telegraph rotation and beam firing.
                Vector2 projectileDestination = Destination - ThingToAttachTo.Center;
                Velocity = Vector2.Normalize(projectileDestination) * LaserVelocity;

                // Direction and rotation.
                if (Projectile.velocity.X < 0f)
                {
                    Projectile.spriteDirection = -1;
                    Projectile.rotation = (float)Math.Atan2((double)-(double)Velocity.Y, (double)-(double)Velocity.X);
                }
                else
                {
                    Projectile.spriteDirection = 1;
                    Projectile.rotation = (float)Math.Atan2((double)Velocity.Y, (double)Velocity.X);
                }
            }

            TelegraphDelay++;
        }

        public override bool CanHitPlayer(Player target) => TelegraphDelay > TelegraphTotalTime;

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0 || TelegraphDelay <= TelegraphTotalTime)
                return;

            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CalamityUtils.CircularHitboxCollision(projHitbox.Center(), Projectile.Size.Length() * 0.5f, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (TelegraphDelay >= TelegraphTotalTime)
            {
                lightColor.R = (byte)(255 * Projectile.Opacity);
                lightColor.G = (byte)(255 * Projectile.Opacity);
                lightColor.B = (byte)(255 * Projectile.Opacity);
                Vector2 drawOffset = Projectile.velocity.SafeNormalize(Vector2.Zero) * -30f;
                Projectile.Center += drawOffset;
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
                Projectile.Center -= drawOffset;
                return false;
            }

            Texture2D laserTelegraph = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/LaserWallTelegraphBeam").Value;

            float yScale = 2f;
            if (TelegraphDelay < TelegraphFadeTime)
                yScale = MathHelper.Lerp(0f, 2f, TelegraphDelay / 15f);
            if (TelegraphDelay > TelegraphTotalTime - TelegraphFadeTime)
                yScale = MathHelper.Lerp(2f, 0f, (TelegraphDelay - (TelegraphTotalTime - TelegraphFadeTime)) / 15f);

            Vector2 scaleInner = new Vector2(TelegraphWidth / laserTelegraph.Width, yScale);
            Vector2 origin = laserTelegraph.Size() * new Vector2(0f, 0.5f);
            Vector2 scaleOuter = scaleInner * new Vector2(1f, 2.2f);

            Color colorOuter = Color.Lerp(Color.Red, Color.Crimson, TelegraphDelay / TelegraphTotalTime * 2f % 1f); // Iterate through crimson and red once and then flash.
            Color colorInner = Color.Lerp(colorOuter, Color.White, 0.75f);

            colorOuter *= 0.6f;
            colorInner *= 0.6f;

            Main.EntitySpriteDraw(laserTelegraph, Projectile.Center - Main.screenPosition, null, colorInner, Velocity.ToRotation(), origin, scaleInner, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(laserTelegraph, Projectile.Center - Main.screenPosition, null, colorOuter, Velocity.ToRotation(), origin, scaleOuter, SpriteEffects.None, 0);
            return false;
        }
    }
}
