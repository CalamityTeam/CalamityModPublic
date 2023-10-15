using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace CalamityMod.Projectiles.VanillaProjectileOverrides
{
    public static class RavenMinionAI
    {
        // Shorter range when the minion has no target yet: 60 tiles
        // This is half of the width of a 1920x1080 screen.
        private const float MinEnemyDistanceDetection = 960f;

        // Longer range when a target is already acquired: 150 tiles
        private const float MaxEnemyDistanceDetection = 2400f;

        // The detection distance dynamically updates.
        // If the minion hasn't detected a target, the minion will check within screen more or less.
        // When the minion has detected a target, the minion will follow it as long as we set it to.
        private static float EnemyDistanceDetection { get => Target is null ? MinEnemyDistanceDetection : MaxEnemyDistanceDetection; }

        private const float DashSpeed = 35f;
        private static SoundStyle CrowNoises = new("CalamityMod/Sounds/Custom/Crow", 3);
        private static NPC Target { get; set; }

        public static bool DoRavenMinionAI(Projectile proj)
        {
            Player owner = Main.player[proj.owner];
            Target = owner.Center.MinionHoming(EnemyDistanceDetection, owner);

            CheckMinionExistence(proj, owner);
            DoAnimation(proj);

            proj.localNPCHitCooldown = 10;
            proj.friendly = true;
            proj.tileCollide = false;
            proj.ignoreWater = true;
            proj.usesLocalNPCImmunity = true;
            proj.usesIDStaticNPCImmunity = false;
            proj.rotation = MathHelper.ToRadians(proj.velocity.X);
            proj.MinionAntiClump(0.5f);

            if (Target is not null)
            {
                Vector2 dashDirection = proj.SafeDirectionTo(Target.Center);

                // If the minion is not withing dashing range, go towards it.
                // When the minion's inside the range, it'll just move forward
                // until it hits the outside bounds of the range, changing it's directions back.
                // Hence giving the effect of a dash.
                if (!proj.WithinRange(Target.Center, 240f))
                {
                    float inertia = 5f;
                    proj.velocity = (proj.velocity * inertia + dashDirection * DashSpeed) / (inertia + 1f);
                    SyncVariables(proj);
                }

                // But if there was the case where the minion was already inside the range,
                // if the velocity's not around the dash speed, make it dash.
                else if (proj.velocity.Length() < DashSpeed - 15f)
                {
                    proj.velocity = dashDirection * (DashSpeed - 10f);
                    SyncVariables(proj);
                }
            }
            else
            {
                // The minion will hover around the owner.
                if (!proj.WithinRange(owner.Center, 320f))
                {
                    proj.velocity = (proj.velocity + proj.SafeDirectionTo(owner.Center)) * 0.9f;
                    SyncVariables(proj);
                }

                // The minion will teleport on the owner if they get far enough.
                if (!proj.WithinRange(owner.Center, MaxEnemyDistanceDetection))
                {
                    proj.Center = owner.Center;
                    SyncVariables(proj);
                }

                // So the minion doens't still weirdly still, move it.
                if (proj.velocity == Vector2.Zero)
                    proj.velocity = Main.rand.NextVector2Circular(5f, 5f);
            }

            if (!Main.dedServ)
            {
                if (Main.rand.NextBool(6000))
                    SoundEngine.PlaySound(CrowNoises, proj.Center);
            }

            return false;
        }

        public static bool DoRavenMinionDrawing(Projectile proj, ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[proj.type].Value;
            Vector2 drawPosition = proj.Center - Main.screenPosition;
            Rectangle frame = texture.Frame(1, Main.projFrames[proj.type], 0, proj.frame);
            Vector2 origin = frame.Size() * 0.5f;
            SpriteEffects effects = MathF.Sign(-proj.velocity.X) == 1f ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            ProjectileID.Sets.TrailingMode[proj.type] = 2;
            ProjectileID.Sets.TrailCacheLength[proj.type] = 5;

            if (Target is not null)
                CalamityUtils.DrawAfterimagesCentered(proj, 0, Color.MediumPurple with { A = 50 });

            Main.EntitySpriteDraw(texture, drawPosition, frame, proj.GetAlpha(lightColor), proj.rotation, origin, proj.scale, effects);

            return false;
        }

        #region AI Methods

        private static void CheckMinionExistence(Projectile proj, Player owner)
        {
            owner.AddBuff(BuffID.Ravens, 2);
            if (proj.type != ProjectileID.Raven)
                return;

            if (owner.dead)
                owner.raven = false;
            if (owner.raven)
                proj.timeLeft = 2;
        }

        private static void DoAnimation(Projectile proj)
        {
            proj.frameCounter++;
            if (proj.frameCounter >= 5)
            {
                proj.frame = (proj.frame + 1) % Main.projFrames[proj.type];
                proj.frameCounter = 0;
            }
        }

        private static void SyncVariables(Projectile proj)
        {
            proj.netUpdate = true;
            if (proj.netSpam >= 10)
                proj.netSpam = 9;
        }

        #endregion
    }
}
