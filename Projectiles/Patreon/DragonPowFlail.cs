using CalamityMod.Items.Patreon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Patreon
{
    public class DragonPowFlail : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon Pow");
        }

        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 50;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.extraUpdates = 2;

            // Only hits enemies once, but doesn't give them iframes so sparks and explosions can hit.
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;

            projectile.alpha = 255;
        }

        // ai[0] = flail's current behavior state
        // 0 = flail is heading outwards
        // 1 = flail is coming back (Any other value also makes the flail return)
        // ai[1] = tick counter used to spawn projectiles and control transparency
        public override void AI()
        {
            drawOffsetX = 0;
            drawOriginOffsetY = -11;
            drawOriginOffsetX = 0;

            Player owner = Main.player[projectile.owner];

            // Update facing so it's always directly away from player
            Vector2 posDiff = owner.Center - projectile.Center;
            projectile.rotation = posDiff.ToRotation();

            // Destroy immediately if the owner is dead
            if (owner.dead)
            {
                projectile.Kill();
                return;
            }

            // Make the owner face the correct direction for the flail
            if (posDiff.X < 0f)
            {
                owner.ChangeDir(1);
                projectile.direction = 1;
                projectile.spriteDirection = 1;
                projectile.rotation += MathHelper.Pi; // keep dragon head facing up
            }
            else
            {
                owner.ChangeDir(-1);
                projectile.direction = -1;
                projectile.spriteDirection = -1;
            }

            // Keep the owner's hand out for as long as necessary and face it the right way
            owner.itemRotation = (-1f * posDiff * (float)projectile.direction).ToRotation();
            owner.itemAnimation = 6;
            owner.itemTime = 6;

            // If the flail is far enough out and still "heading out", then swap it to be returning
            float dist = posDiff.Length();
            if (projectile.ai[0] == 0f && dist > 800f)
                projectile.ai[0] = 1f;

            // Flail returning behavior
            if (projectile.ai[0] != 0f)
            {
                // If the flail gets way too far away, just destroy it
                if (dist > 1500f)
                {
                    projectile.Kill();
                    return;
                }

                // Set velocity to be directly towards the player
                projectile.velocity = posDiff.SafeNormalize(Vector2.Zero) * DragonPow.ReturnSpeed;

                // Once the flail is close enough to the player, destroy it
                if (posDiff.Length() < DragonPow.ReturnSpeed)
                {
                    projectile.Kill();
                    return;
                }
            }

            // Creates a flurry of dust
            int numDust = 5;
            for (int i = 0; i < numDust; ++i)
            {
                int dustType = Main.rand.NextBool(3) ? 246 : 244;
                float scale = 0.8f + Main.rand.NextFloat(0.6f);
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].scale = scale;
                Main.dust[idx].velocity *= 2f;
                Main.dust[idx].velocity += projectile.velocity * 0.3f;
            }

            // Increment the frame counter
            ++projectile.ai[1];

            // If the flail has been out for at least 5 frames, make it visible
            projectile.alpha = (projectile.ai[1] > 5f) ? 0 : 255;

            if (projectile.ai[1] % 4 == 0)
            {
                int type = mod.ProjectileType("DraconicSpark");
                int damage = DragonPow.BaseDamage / 8;
                float kb = 3f;
                float speed = DragonPow.SparkSpeed;
                Vector2 vel = new Vector2(speed, 0f).RotatedByRandom(MathHelper.TwoPi);
                vel += projectile.velocity * 0.05f;
                float sparkVariety = Main.rand.Next(3);

                if (projectile.owner == Main.myPlayer)
                    Projectile.NewProjectile(projectile.Center, vel, type, damage, kb, projectile.owner, sparkVariety, 0f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Inflicts Daybroken, Abyssal Flames and Holy Flames for 8 seconds on-hit
            target.AddBuff(BuffID.Daybreak, 480);
            target.AddBuff(mod.BuffType("AbyssalFlames"), 480);
            target.AddBuff(mod.BuffType("HolyLight"), 480);

            projectile.ai[0] = 1f;
            projectile.netUpdate = true;

            // No petals or waterfalls when hitting dummies.
            if (target.type == NPCID.TargetDummy)
                return;

            PetalStorm(target, damage, knockback, crit);
            Waterfalls(target, damage, knockback, crit);
        }

        // Spawns a storm of flower petals on-hit.
        private void PetalStorm(NPC target, int damage, float knockback, bool crit)
        {
            Main.PlaySound(SoundID.Item105, target.Center);

            int type = ProjectileID.FlowerPetal;
            int numPetals = 12;
            int petalDamage = DragonPow.BaseDamage / 8;
            float petalKB = 0f;
            Player owner = Main.player[projectile.owner];
            for (int i = 0; i < numPetals; ++i)
            {
                float startOffsetX = Main.rand.NextFloat(1000f, 1400f) * (Main.rand.NextBool() ? -1f : 1f);
                float startOffsetY = Main.rand.NextFloat(80f, 900f) * (Main.rand.NextBool() ? -1f : 1f);
                Vector2 startPos = new Vector2(target.Center.X + startOffsetX, target.Center.Y + startOffsetY);
                float dx = target.Center.X - startPos.X;
                float dy = target.Center.Y - startPos.Y;

                // Add some randomness / inaccuracy to the petal target location
                dx += Main.rand.NextFloat(-5f, 5f);
                dy += Main.rand.NextFloat(-5f, 5f);
                float speed = Main.rand.NextFloat(DragonPow.MinPetalSpeed, DragonPow.MaxPetalSpeed);
                float dist = (float)Math.Sqrt((double)(dx * dx + dy * dy));
                dist = speed / dist;
                dx *= dist;
                dy *= dist;
                Vector2 petalVel = new Vector2(dx, dy);
                float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                if (projectile.owner == Main.myPlayer)
                {
                    int idx = Projectile.NewProjectile(startPos, petalVel, type, petalDamage, petalKB, projectile.owner, 0f, 0f);
                    Main.projectile[idx].rotation = angle;
                    Main.projectile[idx].melee = true;
                    Main.projectile[idx].usesLocalNPCImmunity = true;
                    Main.projectile[idx].localNPCHitCooldown = -1;
                }
            }
        }

        private void Waterfalls(NPC target, int damage, float knockback, bool crit)
        {
            int type = mod.ProjectileType("OceanBeam");
            int numWaterfalls = 12;
            int waterfallDamage = DragonPow.BaseDamage / 6;
            float waterfallKB = 0f;
            Player owner = Main.player[projectile.owner];
            for (int i = 0; i < numWaterfalls; ++i)
            {
                float startOffsetX = Main.rand.NextFloat(-120f, 120f);
                float startOffsetY = Main.rand.NextFloat(-740f, -700f);
                Vector2 startPos = new Vector2(target.Center.X + startOffsetX, target.Center.Y + startOffsetY);
                float fallSpeed = Main.rand.NextFloat(DragonPow.MinWaterfallSpeed, DragonPow.MaxWaterfallSpeed);
                Vector2 fallVec = new Vector2(0f, fallSpeed);
                fallVec = fallVec.RotatedBy(Main.rand.NextFloat(-0.08f, 0.08f));

                if (projectile.owner == Main.myPlayer)
                {
                    int idx = Projectile.NewProjectile(startPos, fallVec, type, waterfallDamage, waterfallKB, projectile.owner, 0f, 0f);
                    Main.projectile[idx].timeLeft = 100;
                    Main.projectile[idx].tileCollide = false;
                }
            }
        }

        // PreDraw draws the flail's chain (underneath it, so it doesn't look weird)
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Player owner = Main.player[projectile.owner];
            Vector2 mountedCenter = owner.MountedCenter;
            Color transparent = Color.Transparent;
            Texture2D chainTex = mod.GetTexture("ExtraTextures/Chains/DragonPowChain");
            Vector2 chainDrawPos = projectile.Center;

            Rectangle? sourceRectangle = null;
            Vector2 origin = new Vector2(chainTex.Width * 0.5f, chainTex.Height * 0.5f);

            Vector2 posDiff = mountedCenter - chainDrawPos;
            float rot = (float)Math.Atan2(posDiff.Y, posDiff.X) - MathHelper.PiOver2;

            // If going right: Flip chain segments over so they face upwards
            if (posDiff.X < 0f)
                rot += MathHelper.Pi;

            // Just don't draw anything if there's NaNs to prevent crashes
            bool keepDrawing = true;
            if (chainDrawPos.HasNaNs() || posDiff.HasNaNs())
                keepDrawing = false;

            // Draw chain segments until you run out of space
            while (keepDrawing)
            {
                // Stop drawing when closer than 1 chain link away.
                if (posDiff.Length() < chainTex.Height + 1f)
                {
                    break;
                }

                Vector2 chainDirection = posDiff.SafeNormalize(Vector2.Zero);
                chainDrawPos += chainDirection * chainTex.Height;
                posDiff = mountedCenter - chainDrawPos;
                Color colorAtLoc = Lighting.GetColor((int)chainDrawPos.X / 16, (int)chainDrawPos.Y / 16);
                Main.spriteBatch.Draw(chainTex, chainDrawPos - Main.screenPosition, sourceRectangle, colorAtLoc, rot, origin, 1f, SpriteEffects.None, 0f);
            }
            return true;
        }
    }
}
