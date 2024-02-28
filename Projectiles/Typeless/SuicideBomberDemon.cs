using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Graphics.Primitives;

namespace CalamityMod.Projectiles.Typeless
{
    public class SuicideBomberDemon : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public bool HasDamagedSomething
        {
            get => Projectile.ai[0] == 1f;
            set => Projectile.ai[0] = value.ToInt();
        }
        public ref float Time => ref Projectile.ai[1];
        public Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 11;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 48;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.Opacity = 0f;
            Projectile.timeLeft = 600;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.friendly);
            writer.Write(Projectile.hostile);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.friendly = reader.ReadBoolean();
            Projectile.hostile = reader.ReadBoolean();
        }

        public override void AI()
        {
            Time++;

            // Decide an owner if necessary.
            if (Projectile.owner == Main.maxPlayers)
                Projectile.owner = Player.FindClosest(Projectile.Center, 1, 1);

            // Rapidly fade in.
            Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity + 0.025f, 0f, 1f);

            // Anti-clumping behavior.
            float pushForce = 0.08f;
            for (int k = 0; k < Main.maxProjectiles; k++)
            {
                Projectile otherProj = Main.projectile[k];

                // Short circuits to make the loop as fast as possible.
                if (!otherProj.active || otherProj.type != Projectile.type || k == Projectile.whoAmI)
                    continue;

                // If the other projectile is indeed the same owned by the same player and they're too close, nudge them away.
                bool sameProjType = otherProj.type == Projectile.type;
                float taxicabDist = MathHelper.Distance(Projectile.position.X, otherProj.position.X) + MathHelper.Distance(Projectile.position.Y, otherProj.position.Y);
                if (sameProjType && taxicabDist < 60f)
                {
                    if (Projectile.position.X < otherProj.position.X)
                        Projectile.velocity.X -= pushForce;
                    else
                        Projectile.velocity.X += pushForce;

                    if (Projectile.position.Y < otherProj.position.Y)
                        Projectile.velocity.Y -= pushForce;
                    else
                        Projectile.velocity.Y += pushForce;
                }
            }

            Entity target = Owner;
            float attackFlySpeed = 18.5f;
            float flyInertia = 25f;
            if (Projectile.friendly)
            {
                target = Projectile.Center.ClosestNPCAt(1360f);
                attackFlySpeed = 26.75f;
                flyInertia = 8f;
            }

            // Nullify the target value if they're a dead player.
            else if (!Owner.active || Owner.dead)
                target = null;

            int oldSpriteDirection = Projectile.spriteDirection;

            // Rise upward somewhat slowly and flap wings.
            if (Time < 45f)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, -Vector2.UnitY * 5f, 0.06f);
                if (Projectile.frameCounter >= 6)
                {
                    Projectile.frame = (Projectile.frame + 1) % 5;
                    Projectile.frameCounter = 0;
                }
            }
            else if (Time < 90f)
            {
                // Slow down.
                Projectile.velocity = Projectile.velocity.MoveTowards(Vector2.Zero, 0.4f) * 0.95f;

                // Handle frames.
                Projectile.frame = (int)Math.Round(MathHelper.Lerp(5f, 10f, Utils.GetLerpValue(45f, 90f, Time, true)));

                // And look at the target.
                float idealAngle = target is null ? 0f : Projectile.AngleTo(target.Center);
                Projectile.spriteDirection = target is null ? 1 : (target.Center.X > Projectile.Center.X).ToDirectionInt();
                if (Projectile.spriteDirection != oldSpriteDirection)
                    Projectile.rotation += MathHelper.Pi;

                if (Projectile.spriteDirection == -1)
                    idealAngle += MathHelper.Pi;
                Projectile.rotation = Projectile.rotation.AngleTowards(idealAngle, 0.3f).AngleLerp(idealAngle, 0.08f);

                if (Time == 75f)
                    SoundEngine.PlaySound(SoundID.DD2_WyvernScream, Projectile.Center);

                // Reset the oldPos array in anticipation of a trail being drawn during the fly phase.
                Projectile.oldPos = new Vector2[Projectile.oldPos.Length];
            }
            else
            {

                if (Time == 90f)
                    SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown, Projectile.Center);

                Projectile.frame = Main.projFrames[Projectile.type] - 1;

                // Fly away if no valid target was found.
                if (target is null)
                    Projectile.velocity = -Vector2.UnitY * 18f;

                // Otherwise fly towards the target.
                else
                    Projectile.velocity = (Projectile.velocity * (flyInertia - 1f) + Projectile.SafeDirectionTo(target.Center) * attackFlySpeed) / flyInertia;

                Projectile.spriteDirection = (Projectile.velocity.X > 0f).ToDirectionInt();
                Projectile.rotation = CalamityUtils.WrapAngle90Degrees(Projectile.velocity.ToRotation());
                if (target is null)
                {
                    Projectile.spriteDirection = 1;
                    Projectile.rotation = Projectile.velocity.ToRotation();
                }

                // Die if something has been hit and the target is either gone or really close.
                if (HasDamagedSomething && (target is null || target == Owner || Projectile.WithinRange(target.Center, target.Size.Length() * 0.4f)))
                    Projectile.Kill();
            }

            if (target != null && !HasDamagedSomething && Projectile.Center.ManhattanDistance(target.Center) < target.height)
            {
                HasDamagedSomething = true;
                Projectile.netUpdate = true;
            }

            if (Time >= 300f)
                Projectile.Kill();

            Projectile.frameCounter++;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, Projectile.Center);
            for (int i = 0; i < 40; i++)
            {
                Dust explosion = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 267);
                explosion.velocity = Main.rand.NextVector2Circular(4f, 4f);
                explosion.color = Color.Red;
                explosion.scale = 1.35f;
                explosion.fadeIn = 0.45f;
                explosion.noGravity = true;

                if (Main.rand.NextBool(3))
                    explosion.scale *= 1.45f;

                if (Main.rand.NextBool(6))
                {
                    explosion.scale *= 1.75f;
                    explosion.fadeIn += 0.4f;
                }
            }
        }

        public override bool? CanDamage() => Projectile.Opacity >= 1f ? null : false;

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.SourceDamage *= 0f;
            if (Main.masterMode) modifiers.SourceDamage.Flat += 540f;
            else if (Main.expertMode) modifiers.SourceDamage.Flat += 450f;
            else modifiers.SourceDamage.Flat += 360f;
        }

        public float FlameTrailWidthFunction(float completionRatio) => MathHelper.SmoothStep(21f, 8f, completionRatio);

        public Color FlameTrailColorFunction(float completionRatio)
        {
            float trailOpacity = Utils.GetLerpValue(0.8f, 0.27f, completionRatio, true) * Utils.GetLerpValue(0f, 0.067f, completionRatio, true);
            Color startingColor = Color.Lerp(Color.Cyan, Color.White, 0.4f);
            Color middleColor = Color.Lerp(Color.Orange, Color.Yellow, 0.3f);
            Color endColor = Color.Lerp(Color.Orange, Color.Red, 0.67f);
            return CalamityUtils.MulticolorLerp(completionRatio, startingColor, middleColor, endColor) * trailOpacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.EnterShaderRegion();

            // Prepare the flame trail shader with its map texture.
            GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));

            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/SuicideBomberDemon").Value;
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/SuicideBomberDemonGlowmask").Value;
            Texture2D orbTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/SuicideBomberDemonOrb").Value;
            if (Projectile.friendly)
            {
                texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/SuicideBomberDemonFriendly").Value;
                glowmask = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/SuicideBomberDemonGlowmaskFriendly").Value;
                orbTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/SuicideBomberDemonOrbFriendly").Value;
            }
            Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            SpriteEffects direction = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            // Draw the base sprite and glowmask.
            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, direction, 0);
            Main.EntitySpriteDraw(glowmask, drawPosition, frame, Projectile.GetAlpha(Color.White), Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, direction, 0);

            // Draw the flame trail and flame orb once ready.
            if (Time >= 90f)
            {
                float flameOrbGlowIntensity = Utils.GetLerpValue(90f, 98f, Time, true);
                for (int i = 0; i < 12; i++)
                {
                    Color flameOrbColor = Color.LightCyan * flameOrbGlowIntensity * 0.125f;
                    flameOrbColor.A = 0;
                    Vector2 flameOrbDrawOffset = (MathHelper.TwoPi * i / 12f + Main.GlobalTimeWrappedHourly * 2f).ToRotationVector2();
                    flameOrbDrawOffset *= flameOrbGlowIntensity * 3f;
                    Main.EntitySpriteDraw(orbTexture, drawPosition + flameOrbDrawOffset, frame, Projectile.GetAlpha(flameOrbColor), Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, direction, 0);
                }

                Vector2 trailOffset = Projectile.Size * 0.5f;
                trailOffset += (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * 20f;
                PrimitiveSet.Prepare(Projectile.oldPos, new(FlameTrailWidthFunction, FlameTrailColorFunction, (_) => trailOffset, shader: GameShaders.Misc["CalamityMod:ImpFlameTrail"]), 61);
            }

            Main.spriteBatch.ExitShaderRegion();

            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            HasDamagedSomething = true;
            Projectile.netUpdate = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            HasDamagedSomething = true;
            Projectile.netUpdate = true;
        }
    }
}
