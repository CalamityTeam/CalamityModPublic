using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class SuicideBomberDemon : ModProjectile
    {
        public bool HasDamagedSomething
        {
            get => projectile.ai[0] == 1f;
            set => projectile.ai[0] = value.ToInt();
        }
        public ref float Time => ref projectile.ai[1];
        public Player Owner => Main.player[projectile.owner];
        public PrimitiveTrail FlameTrailDrawer = null;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demon");
            Main.projFrames[projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 11;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 48;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
            projectile.Opacity = 0f;
            projectile.timeLeft = 600;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.friendly);
            writer.Write(projectile.hostile);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.friendly = reader.ReadBoolean();
            projectile.hostile = reader.ReadBoolean();
        }

        public override void AI()
        {
            Time++;

            // Decide an owner if necessary.
            if (projectile.owner == 255)
                projectile.owner = Player.FindClosest(projectile.Center, 1, 1);

            // Rapidly fade in.
            projectile.Opacity = MathHelper.Clamp(projectile.Opacity + 0.025f, 0f, 1f);

            // Anti-clumping behavior.
            float pushForce = 0.08f;
            for (int k = 0; k < Main.maxProjectiles; k++)
            {
                Projectile otherProj = Main.projectile[k];

                // Short circuits to make the loop as fast as possible.
                if (!otherProj.active || otherProj.type != projectile.type || k == projectile.whoAmI)
                    continue;

                // If the other projectile is indeed the same owned by the same player and they're too close, nudge them away.
                bool sameProjType = otherProj.type == projectile.type;
                float taxicabDist = MathHelper.Distance(projectile.position.X, otherProj.position.X) + MathHelper.Distance(projectile.position.Y, otherProj.position.Y);
                if (sameProjType && taxicabDist < 60f)
                {
                    if (projectile.position.X < otherProj.position.X)
                        projectile.velocity.X -= pushForce;
                    else
                        projectile.velocity.X += pushForce;

                    if (projectile.position.Y < otherProj.position.Y)
                        projectile.velocity.Y -= pushForce;
                    else
                        projectile.velocity.Y += pushForce;
                }
            }

            Entity target = Owner;
            float attackFlySpeed = 18.5f;
            float flyInertia = 25f;
            if (projectile.friendly)
            {
                target = projectile.Center.ClosestNPCAt(1360f);
                attackFlySpeed = 26.75f;
                flyInertia = 8f;
            }

            // Nullify the target value if they're a dead player.
            else if (!Owner.active || Owner.dead)
                target = null;

            int oldSpriteDirection = projectile.spriteDirection;

            // Rise upward somewhat slowly and flap wings.
            if (Time < 45f)
            {
                projectile.velocity = Vector2.Lerp(projectile.velocity, -Vector2.UnitY * 5f, 0.06f);
                if (projectile.frameCounter >= 6)
                {
                    projectile.frame = (projectile.frame + 1) % 5;
                    projectile.frameCounter = 0;
                }
            }
            else if (Time < 90f)
            {
                // Slow down.
                projectile.velocity = projectile.velocity.MoveTowards(Vector2.Zero, 0.4f) * 0.95f;

                // Handle frames.
                projectile.frame = (int)Math.Round(MathHelper.Lerp(5f, 10f, Utils.InverseLerp(45f, 90f, Time, true)));

                // And look at the target.
                float idealAngle = target is null ? 0f : projectile.AngleTo(target.Center);
                projectile.spriteDirection = target is null ? 1 : (target.Center.X > projectile.Center.X).ToDirectionInt();
                if (projectile.spriteDirection != oldSpriteDirection)
                    projectile.rotation += MathHelper.Pi;

                if (projectile.spriteDirection == -1)
                    idealAngle += MathHelper.Pi;
                projectile.rotation = projectile.rotation.AngleTowards(idealAngle, 0.3f).AngleLerp(idealAngle, 0.08f);

                if (Time == 75f)
                    Main.PlaySound(SoundID.DD2_WyvernScream, projectile.Center);

                // Reset the oldPos array in anticipation of a trail being drawn during the fly phase.
                projectile.oldPos = new Vector2[projectile.oldPos.Length];
            }
            else
            {

                if (Time == 90f)
                    Main.PlaySound(SoundID.DD2_WyvernDiveDown, projectile.Center);

                projectile.frame = Main.projFrames[projectile.type] - 1;

                // Fly away if no valid target was found.
                if (target is null)
                    projectile.velocity = -Vector2.UnitY * 18f;

                // Otherwise fly towards the target.
                else
                    projectile.velocity = (projectile.velocity * (flyInertia - 1f) + projectile.SafeDirectionTo(target.Center) * attackFlySpeed) / flyInertia;

                projectile.spriteDirection = (projectile.velocity.X > 0f).ToDirectionInt();
                projectile.rotation = CalamityUtils.WrapAngle90Degrees(projectile.velocity.ToRotation());
                if (target is null)
                {
                    projectile.spriteDirection = 1;
                    projectile.rotation = projectile.velocity.ToRotation();
                }

                // Die if something has been hit and the target is either gone or really close.
                if (HasDamagedSomething && (target is null || target == Owner || projectile.WithinRange(target.Center, target.Size.Length() * 0.4f)))
                    projectile.Kill();
            }

            if (target != null && !HasDamagedSomething && projectile.Center.ManhattanDistance(target.Center) < target.height)
            {
                HasDamagedSomething = true;
                projectile.netUpdate = true;
            }

            if (Time >= 300f)
                projectile.Kill();

            projectile.frameCounter++;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.DD2_KoboldExplosion, projectile.Center);
            for (int i = 0; i < 40; i++)
            {
                Dust explosion = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 267);
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

        public override bool CanDamage() => projectile.Opacity >= 1f;

        // Ensure damage is not absolutely obscene when hitting players.
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit) => damage = 95;

        public float FlameTrailWidthFunction(float completionRatio) => MathHelper.SmoothStep(21f, 8f, completionRatio);

        public Color FlameTrailColorFunction(float completionRatio)
        {
            float trailOpacity = Utils.InverseLerp(0.8f, 0.27f, completionRatio, true) * Utils.InverseLerp(0f, 0.067f, completionRatio, true);
            Color startingColor = Color.Lerp(Color.Cyan, Color.White, 0.4f);
            Color middleColor = Color.Lerp(Color.Orange, Color.Yellow, 0.3f);
            Color endColor = Color.Lerp(Color.Orange, Color.Red, 0.67f);
            return CalamityUtils.MulticolorLerp(completionRatio, startingColor, middleColor, endColor) * trailOpacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.EnterShaderRegion();

            // Initialize the flame trail drawer.
            if (FlameTrailDrawer is null)
                FlameTrailDrawer = new PrimitiveTrail(FlameTrailWidthFunction, FlameTrailColorFunction, null, GameShaders.Misc["CalamityMod:ImpFlameTrail"]);

            // Prepare the flame trail shader with its map texture.
            GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture(ModContent.GetTexture("CalamityMod/ExtraTextures/ScarletDevilStreak"));

            Texture2D texture = ModContent.GetTexture("CalamityMod/Projectiles/Typeless/SuicideBomberDemon");
            Texture2D glowmask = ModContent.GetTexture("CalamityMod/Projectiles/Typeless/SuicideBomberDemonGlowmask");
            Texture2D orbTexture = ModContent.GetTexture("CalamityMod/Projectiles/Typeless/SuicideBomberDemonOrb");
            if (projectile.friendly)
            {
                texture = ModContent.GetTexture("CalamityMod/Projectiles/Typeless/SuicideBomberDemonFriendly");
                glowmask = ModContent.GetTexture("CalamityMod/Projectiles/Typeless/SuicideBomberDemonGlowmaskFriendly");
                orbTexture = ModContent.GetTexture("CalamityMod/Projectiles/Typeless/SuicideBomberDemonOrbFriendly");
            }
            Rectangle frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            Vector2 drawPosition = projectile.Center - Main.screenPosition;
            SpriteEffects direction = projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            // Draw the base sprite and glowmask.
            spriteBatch.Draw(texture, drawPosition, frame, projectile.GetAlpha(lightColor), projectile.rotation, frame.Size() * 0.5f, projectile.scale, direction, 0f);
            spriteBatch.Draw(glowmask, drawPosition, frame, projectile.GetAlpha(Color.White), projectile.rotation, frame.Size() * 0.5f, projectile.scale, direction, 0f);

            // Draw the flame trail and flame orb once ready.
            if (Time >= 90f)
            {
                float flameOrbGlowIntensity = Utils.InverseLerp(90f, 98f, Time, true);
                for (int i = 0; i < 12; i++)
                {
                    Color flameOrbColor = Color.LightCyan * flameOrbGlowIntensity * 0.125f;
                    flameOrbColor.A = 0;
                    Vector2 flameOrbDrawOffset = (MathHelper.TwoPi * i / 12f + Main.GlobalTime * 2f).ToRotationVector2();
                    flameOrbDrawOffset *= flameOrbGlowIntensity * 3f;
                    spriteBatch.Draw(orbTexture, drawPosition + flameOrbDrawOffset, frame, projectile.GetAlpha(flameOrbColor), projectile.rotation, frame.Size() * 0.5f, projectile.scale, direction, 0f);
                }

                Vector2 trailOffset = projectile.Size * 0.5f;
                trailOffset += (projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * 20f;
                FlameTrailDrawer.Draw(projectile.oldPos, trailOffset - Main.screenPosition, 61);
            }

            spriteBatch.ExitShaderRegion();

            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            HasDamagedSomething = true;
            projectile.netUpdate = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            HasDamagedSomething = true;
            projectile.netUpdate = true;
        }
    }
}
