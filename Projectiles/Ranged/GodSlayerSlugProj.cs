using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class GodSlayerSlugProj : ModProjectile
    {
        private const int Lifetime = 600;
        private const int NoDrawFrames = 2;
        private const int BlueNoCollideFrames = 9;
        private const int TurnBlueFrameDelay = 7;
        // Radius of the "circle of inaccuracy" surrounding the mouse. Blue bullets will aim at this circle.
        private const float MouseAimDeviation = 13f;
        private const int TextureHeight = 136;

        private bool BlueMode => projectile.ai[0] != 0f;
        public override string Texture => "CalamityMod/Projectiles/Ranged/GodSlayerSlugPurple";
        private static Texture2D TextureBlue;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("God Slayer Slug");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;

            if (Main.netMode != NetmodeID.Server)
                TextureBlue = mod.GetTexture("Projectiles/Ranged/GodSlayerSlugBlue");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.ignoreWater = true;
            projectile.aiStyle = 1;
            aiType = ProjectileID.Bullet;
            projectile.alpha = 255;
            projectile.MaxUpdates = 6;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
            projectile.timeLeft = Lifetime;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(projectile.tileCollide);
        public override void ReceiveExtraAI(BinaryReader reader) => projectile.tileCollide = reader.ReadBoolean();

        public override void AI()
        {
            // Store the original velocity if it has yet to be initialized. This is needed for the warp.
            if (projectile.localAI[0] == 0f)
                projectile.localAI[0] = projectile.velocity.Length();

            // Rapidly fade into visibility.
            if (projectile.alpha > 0)
                projectile.alpha -= 17;

            // Add light appropriate to the bullet's current state.
            if (BlueMode)
                Lighting.AddLight(projectile.Center, 0.06f, 0.24f, 0.29f);
            else
                Lighting.AddLight(projectile.Center, 0.3f, 0.2f, 0.32f);

            // If the bullet has struck at least one target, increment the counter for turning blue.
            if (projectile.numHits > 0 && CalamityUtils.FinalExtraUpdate(projectile) && !BlueMode)
                ++projectile.ai[1];

            // If the bullet has struck at least one target but hasn't hit anything for several frames, turn blue and warp.
            // Obviously if the bullet is already blue, it can't turn blue again.
            if (!BlueMode && projectile.ai[1] >= TurnBlueFrameDelay)
                TurnBlue(true);

            // When blue, ignore walls for the first several updates.
            if (BlueMode && projectile.ai[1] > 0f)
            {
                --projectile.ai[1];
                if (projectile.ai[1] == 0f)
                    projectile.tileCollide = true;
            }
        }

        private void TurnBlue(bool setPosition = false)
        {
            // Switch to blue mode officially
            projectile.ai[0] = 1f;

            // Provide several frames of passing through walls to prevent frustration
            projectile.ai[1] = BlueNoCollideFrames;
            projectile.tileCollide = false;

            // Reduce damage, but remove piercing. Reset local iframes so the bullet, turned blue, may always strike again.
            projectile.damage = (int)(0.28f * projectile.damage);
            projectile.penetrate = 1;
            for (int i = 0; i < Main.maxNPCs; i++)
                projectile.localNPCImmunity[i] = 0;

            // Reset projectile lifetime so it can fly again for its full possible range
            projectile.timeLeft = Lifetime - NoDrawFrames * projectile.MaxUpdates;

            if (!setPosition || Main.myPlayer != projectile.owner)
                return;

            // The bullet disappears in a puff of dust.
            ProduceWarpCrossDust(projectile.Center, (int)CalamityDusts.PurpleCosmilite);

            // The warp must be performed client side because it requires knowledge of the player's mouse position.
            projectile.netUpdate = true;
            projectile.tileCollide = false;

            // Step 1 of the warp: Place the bullet behind the player, opposite the mouse cursor.
            Vector2 playerToMouseVec = CalamityUtils.SafeDirectionTo(Main.LocalPlayer, Main.MouseWorld, -Vector2.UnitY);
            float warpDist = Main.rand.NextFloat(70f, 96f);
            float warpAngle = Main.rand.NextFloat(-MathHelper.Pi / 3f, MathHelper.Pi / 3f);
            Vector2 warpOffset = -warpDist * playerToMouseVec.RotatedBy(warpAngle);
            projectile.position = Main.LocalPlayer.MountedCenter + warpOffset;

            // Step 2 of the warp: Angle the bullet so that it is pointing at the mouse cursor.
            // This intentionally has a slight inaccuracy.
            Vector2 mouseTargetVec = Main.MouseWorld + Main.rand.NextVector2Circular(MouseAimDeviation, MouseAimDeviation);
            Vector2 bulletToMouseVec = CalamityUtils.SafeDirectionTo(projectile, mouseTargetVec, -Vector2.UnitY);
            projectile.velocity = bulletToMouseVec * projectile.localAI[0];
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // Set all old positions to the bullet's warp position so that there aren't weird afterimages.
            // If an old position is uninitialized (0,0 aka never used), then don't change it.
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; ++i)
            {
                Vector2 oldPosElem = projectile.oldPos[i];
                if (!(oldPosElem == Vector2.Zero))
                    projectile.oldPos[i] = projectile.position;
            }

            // Now that the bullet has warped, produce a tiny puff of dust at its back for effect.
            Vector2 warpInDustPos = projectile.Center - bulletToMouseVec * TextureHeight;
            ProduceWarpCrossDust(warpInDustPos, (int)CalamityDusts.BlueCosmilite);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 140);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft >= Lifetime - NoDrawFrames * projectile.MaxUpdates)
                return false;
            // Use the blue bullet texture if the bullet has turned blue.
            CalamityUtils.DrawAfterimagesFromEdge(projectile, 0, lightColor, BlueMode ? TextureBlue : null);
            return false;
        }

        // God Slayer Slugs explode on death, even if they never visually turned blue.
        public override void Kill(int timeLeft)
        {
            // Turn blue to set stats correctly, if not already done.
            if (!BlueMode)
                TurnBlue(false);
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 48);
            projectile.Damage();

            // Create a fancy triangle of dust.
            int dustID = (int)CalamityDusts.BlueCosmilite;
            int numDust = 9;
            float triangleAngle = Main.rand.NextFloat(MathHelper.TwoPi);
            for (int i = 0; i < numDust; ++i)
            {
                float speed = MathHelper.Lerp(0.2f, 3.6f, i / (float)(numDust - 1));
                Vector2 dustVel = Vector2.UnitX.RotatedBy(triangleAngle) * speed;
                Dust d = Dust.NewDustDirect(projectile.Center, 0, 0, dustID);
                d.position = projectile.Center;
                d.velocity = dustVel;
                d.noGravity = true;
                d.scale *= Main.rand.NextFloat(1.1f, 1.4f);
                Dust.CloneDust(d).velocity = dustVel.RotatedBy(MathHelper.Pi * 2f / 3f);
                Dust.CloneDust(d).velocity = dustVel.RotatedBy(MathHelper.Pi * 4f / 3f);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // If the projectile has hit something but hasn't turned blue, turn it blue and warp it behind the player.
            if (projectile.numHits > 0 && !BlueMode)
            {
                TurnBlue(true);
                return false;
            }

            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Dig, projectile.Center, 1);
            return true;
        }

        private void ProduceWarpCrossDust(Vector2 dustPos, int dustID)
        {
            for (int i = 0; i < 4; ++i)
            {
                float speed = Main.rand.NextFloat(2.0f, 4.1f);
                Vector2 dustVel = Vector2.UnitX * speed;
                Dust d = Dust.NewDustDirect(projectile.Center, 0, 0, dustID);
                d.position = dustPos;
                d.velocity = dustVel;
                d.noGravity = true;
                d.scale *= Main.rand.NextFloat(1.1f, 1.4f);
                Dust.CloneDust(d).velocity = dustVel.RotatedBy(MathHelper.PiOver2);
                Dust.CloneDust(d).velocity = dustVel.RotatedBy(MathHelper.Pi);
                Dust.CloneDust(d).velocity = dustVel.RotatedBy(-MathHelper.PiOver2);
            }
        }
    }
}
