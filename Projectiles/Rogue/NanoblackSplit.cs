using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class NanoblackSplit : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        private static int SpriteWidth = 52;
        private static int Lifetime = 90;
        private static float MaxRotationSpeed = 0.25f;
        private static float MaxSpeed = 22f;

        private static float HomingStartRange = 600f;
        private static float HomingBreakRange = 1000f;
        private static float HomingBonusRangeCap = 200f;
        private static float BaseHomingFactor = 1.6f;
        private static float MaxHomingFactor = 6.6f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 4;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = Lifetime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        // ai[0] = Index of current NPC target. If 0 or negative, the projectile has no target
        // ai[1] = Current spin speed. Negative speeds are also allowed.
        public override void AI()
        {
            DrawOffsetX = -10;
            DrawOriginOffsetY = 0;
            DrawOriginOffsetX = 0;

            // On the very first frame, clear any invalid starting target variable and create some dust.
            // Also grab the damage type based on ai[0].
            if (Projectile.timeLeft == Lifetime)
                SpawnDust();

            // Spin in the specified starting direction and slow down spin over time
            // Loses 1.66% of current speed every frame
            // Also update current orientation to reflect current spin direction
            float currentSpin = Projectile.ai[1];
            Projectile.direction = currentSpin <= 0f ? 1 : -1;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation += currentSpin * MaxRotationSpeed;
            float spinReduction = 0.0166f * currentSpin;
            Projectile.ai[1] -= spinReduction;

            // If about to disappear, shrink by 8% every frame
            if (Projectile.timeLeft < 15)
                Projectile.scale *= 0.92f;

            // Search for and home in on nearby targets
            HomingAI();
        }

        private void HomingAI()
        {
            // If we don't currently have a target, go try and get one!
            int targetID = (int)Projectile.ai[0] - 1;
            if (targetID < 0)
                targetID = AcquireTarget();

            // Save the target, whether we have one or not.
            Projectile.ai[0] = targetID + 1f;

            // If we don't have a target, then just slow down a bit.
            if (targetID < 0)
            {
                Projectile.velocity *= 0.94f;
                return;
            }

            // Homing behavior depends on how far the blade is from its target.
            NPC target = Main.npc[targetID];
            float xDist = Projectile.Center.X - target.Center.X;
            float yDist = Projectile.Center.Y - target.Center.Y;
            float dist = (float)Math.Sqrt(xDist * xDist + yDist * yDist);

            // If the target is too far away, stop homing in on it.
            if (dist > HomingBreakRange)
            {
                Projectile.ai[0] = 0f;
                return;
            }

            // Adds a multiple of the towards-target vector to its velocity every frame.
            float homingFactor = CalcHomingFactor(dist);
            Vector2 posDiff = target.Center - Projectile.Center;
            posDiff = posDiff.SafeNormalize(Vector2.Zero);
            posDiff *= homingFactor;
            Vector2 newVelocity = Projectile.velocity += posDiff;

            // Caps speed to make sure it doesn't go too fast.
            if (newVelocity.Length() >= MaxSpeed)
            {
                newVelocity = newVelocity.SafeNormalize(Vector2.Zero);
                newVelocity *= MaxSpeed;
            }

            Projectile.velocity = newVelocity;
        }

        // Returns the ID of the NPC to be targeted by this energy blade.
        // It chooses the closest target which can be chased, ignoring invulnerable NPCs.
        // Nanoblack Blades prefer to target bosses whenever possible.
        private int AcquireTarget()
        {
            bool bossFound = false;
            int target = -1;
            float minDist = HomingStartRange;
            for (int i = 0; i < Main.maxNPCs; ++i)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.type == NPCID.TargetDummy)
                    continue;

                // If we've found a valid boss target, ignore ALL targets which aren't bosses.
                if (bossFound && !npc.boss)
                    continue;

                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float xDist = Projectile.Center.X - npc.Center.X;
                    float yDist = Projectile.Center.Y - npc.Center.Y;
                    float distToNPC = (float)Math.Sqrt(xDist * xDist + yDist * yDist);
                    if (distToNPC < minDist)
                    {
                        // If this target within range is a boss, set the boss found flag.
                        if (npc.boss)
                            bossFound = true;
                        minDist = distToNPC;
                        target = i;
                    }
                }
            }
            return target;
        }

        // Energy blades home even more aggressively if they are very close to their target.
        private float CalcHomingFactor(float dist)
        {
            float baseFactor = BaseHomingFactor;
            float bonus = (MaxHomingFactor - BaseHomingFactor) * (1f - dist / HomingBonusRangeCap);
            if (bonus < 0f)
                bonus = 0f;
            return baseFactor + bonus;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        // Draws the energy blade's glowmask.
        public override void PostDraw(Color lightColor)
        {
            float fWidthOverTwo = SpriteWidth / 2f;
            float fHeightOverTwo = Projectile.height / 2f;

            // Make sure the glowmask matches the blade's own orientation
            SpriteEffects eff = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                eff = SpriteEffects.FlipHorizontally;
            Vector2 origin = new Vector2(fWidthOverTwo, fHeightOverTwo);
            Main.EntitySpriteDraw(ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/NanoblackSplitGlow").Value,
                Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation,
                origin, Projectile.scale, eff, 0);
        }

        // Spawns a tiny bit of dust when the energy blade vanishes.
        public override void OnKill(int timeLeft)
        {
            SpawnDust();
        }

        // Spawns a small bit of Luminite themed dust.
        private void SpawnDust()
        {
            int dustCount = Main.rand.Next(3, 6);
            Vector2 corner = Projectile.position;
            for (int i = 0; i < dustCount; ++i)
            {
                int dustType = 229;
                float scale = 0.6f + Main.rand.NextFloat(0.4f);
                int idx = Dust.NewDust(corner, Projectile.width, Projectile.height, dustType);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 3f;
                Main.dust[idx].scale = scale;
            }
        }
    }
}
