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

        private bool BlueMode => projectile.ai[0] != 0f;
        public override string Texture => "CalamityMod/Projectiles/Ranged/GodSlayerSlugPurple";
        private static Texture2D TextureBlue;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("God Slayer Slug");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
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
            projectile.extraUpdates = 5;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
            projectile.timeLeft = Lifetime;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(projectile.tileCollide);
        public override void ReceiveExtraAI(BinaryReader reader) => projectile.tileCollide = reader.ReadBoolean();

        public override void AI()
        {
            // On frame 1, store the original velocity. This is needed for the warp.
            if (projectile.timeLeft == Lifetime)
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

            // If the bullet has struck at least one target but hasn't hit anything for ten frames, turn blue and warp.
            if (projectile.ai[1] >= TurnBlueFrameDelay)
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

            // Reduce damage, but remove piercing
            projectile.damage = (int)(0.3f * projectile.damage);
            projectile.penetrate = 1;
            projectile.usesLocalNPCImmunity = false;

            // Reset projectile lifetime so it can fly again for its full possible range
            projectile.timeLeft = Lifetime - NoDrawFrames * projectile.MaxUpdates;

            if (!setPosition || Main.myPlayer != projectile.owner)
                return;

            // The warp must be performed client side because it requires knowledge of the player's mouse position.
            projectile.netUpdate = true;
            projectile.tileCollide = false;
            Vector2 mouseVec = CalamityUtils.SafeDirectionTo(Main.LocalPlayer, Main.MouseWorld, -Vector2.UnitY);
            float warpDist = Main.rand.NextFloat(70f, 96f);
            float warpAngle = Main.rand.NextFloat(-MathHelper.Pi / 3f, MathHelper.Pi / 3f);
            Vector2 warpOffset = -warpDist * mouseVec.RotatedBy(warpAngle);
            projectile.position = Main.LocalPlayer.MountedCenter + warpOffset;

            // Set all old positions to the bullet's warp position so that there aren't weird afterimages
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; ++i)
                projectile.oldPos[i] = projectile.position;

            // The projectile flies at the mouse vector at its original muzzle velocity.
            projectile.velocity = mouseVec * projectile.localAI[0];
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

            // The bullet's explosion is a fancy triangle pattern.
            int dustID = 180;
            for (int i = 0; i < 9; ++i)
            {
                float speed = Main.rand.NextFloat(3f, 11.2f);
                Vector2 dustVel = new Vector2(speed, 0f).RotatedByRandom(MathHelper.TwoPi);
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
    }
}
