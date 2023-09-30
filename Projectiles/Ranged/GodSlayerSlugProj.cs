using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class GodSlayerSlugProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        private const int Lifetime = 600;
        private const int NoDrawFrames = 2;
        // 25 instead of 24 because it is decremented once immediately after turning blue
        private const int BlueNoCollideFrames = 25;
        private const int TurnBlueFrameDelay = 7;
        // Radius of the "circle of inaccuracy" surrounding the mouse. Blue bullets will aim at this circle.
        private const float MouseAimDeviation = 13f;
        private const int TextureHeight = 136;

        private bool BlueMode => Projectile.ai[0] != 0f;
        public override string Texture => "CalamityMod/Projectiles/Ranged/GodSlayerSlugPurple";
        private static Texture2D TextureBlue;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;

            if (Main.netMode != NetmodeID.Server)
                TextureBlue = Mod.Assets.Request<Texture2D>("Projectiles/Ranged/GodSlayerSlugBlue", AssetRequestMode.ImmediateLoad).Value;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.Bullet;
            Projectile.alpha = 255;
            Projectile.MaxUpdates = 6;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = Lifetime;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(Projectile.tileCollide);
        public override void ReceiveExtraAI(BinaryReader reader) => Projectile.tileCollide = reader.ReadBoolean();

        public override void AI()
        {
            // Store the original velocity if it has yet to be initialized. This is needed for the warp.
            if (Projectile.localAI[0] == 0f)
                Projectile.localAI[0] = Projectile.velocity.Length();

            // Rapidly fade into visibility.
            if (Projectile.alpha > 0)
                Projectile.alpha -= 17;

            // Add light appropriate to the bullet's current state.
            if (BlueMode)
                Lighting.AddLight(Projectile.Center, 0.06f, 0.24f, 0.29f);
            else
                Lighting.AddLight(Projectile.Center, 0.3f, 0.2f, 0.32f);

            // If the bullet has struck at least one target, increment the counter for turning blue.
            if (Projectile.numHits > 0 && CalamityUtils.FinalExtraUpdate(Projectile) && !BlueMode)
                ++Projectile.ai[1];

            // If the bullet has struck at least one target but hasn't hit anything for several frames, turn blue and warp.
            // Obviously if the bullet is already blue, it can't turn blue again.
            if (!BlueMode && Projectile.ai[1] >= TurnBlueFrameDelay)
                TurnBlue(true);

            // When blue, ignore walls for the first several updates.
            if (BlueMode && Projectile.ai[1] > 0f)
            {
                --Projectile.ai[1];
                if (Projectile.ai[1] == 0f)
                    Projectile.tileCollide = true;
            }
        }

        private void TurnBlue(bool setPosition = false)
        {
            // Switch to blue mode officially
            Projectile.ai[0] = 1f;

            // Provide several frames of passing through walls to prevent frustration
            Projectile.ai[1] = BlueNoCollideFrames;
            Projectile.tileCollide = false;

            // Reduce damage, but remove piercing. Reset local iframes so the bullet, turned blue, may always strike again. Reset the point blank timer.
            Projectile.damage = (int)(0.28f * Projectile.damage);
            Projectile.penetrate = 1;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
            for (int i = 0; i < Main.maxNPCs; i++)
                Projectile.localNPCImmunity[i] = 0;

            // Reset projectile lifetime so it can fly again for its full possible range
            Projectile.timeLeft = Lifetime - NoDrawFrames * Projectile.MaxUpdates;

            if (!setPosition || Main.myPlayer != Projectile.owner)
                return;

            // The bullet disappears in a puff of dust.
            ProduceWarpCrossDust(Projectile.Center, (int)CalamityDusts.PurpleCosmilite);

            // The warp must be performed client side because it requires knowledge of the player's mouse position.
            Projectile.netUpdate = true;
            Projectile.tileCollide = false;

            // Step 1 of the warp: Place the bullet behind the player, opposite the mouse cursor.
            Vector2 playerToMouseVec = CalamityUtils.SafeDirectionTo(Main.LocalPlayer, Main.MouseWorld, -Vector2.UnitY);
            float warpDist = Main.rand.NextFloat(70f, 96f);
            float warpAngle = Main.rand.NextFloat(-MathHelper.Pi / 3f, MathHelper.Pi / 3f);
            Vector2 warpOffset = -warpDist * playerToMouseVec.RotatedBy(warpAngle);
            Projectile.position = Main.LocalPlayer.MountedCenter + warpOffset;

            // Step 2 of the warp: Angle the bullet so that it is pointing at the mouse cursor.
            // This intentionally has a slight inaccuracy.
            Vector2 mouseTargetVec = Main.MouseWorld + Main.rand.NextVector2Circular(MouseAimDeviation, MouseAimDeviation);
            Vector2 bulletToMouseVec = CalamityUtils.SafeDirectionTo(Projectile, mouseTargetVec, -Vector2.UnitY);
            Projectile.velocity = bulletToMouseVec * Projectile.localAI[0];
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // Set all old positions to the bullet's warp position so that there aren't weird afterimages.
            // If an old position is uninitialized (0,0 aka never used), then don't change it.
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; ++i)
            {
                Vector2 oldPosElem = Projectile.oldPos[i];
                if (!(oldPosElem == Vector2.Zero))
                    Projectile.oldPos[i] = Projectile.position;
            }

            // Now that the bullet has warped, produce a tiny puff of dust at its back for effect.
            Vector2 warpInDustPos = Projectile.Center - bulletToMouseVec * TextureHeight;
            ProduceWarpCrossDust(warpInDustPos, (int)CalamityDusts.BlueCosmilite);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 140);

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft >= Lifetime - NoDrawFrames * Projectile.MaxUpdates)
                return false;
            // Use the blue bullet texture if the bullet has turned blue.
            CalamityUtils.DrawAfterimagesFromEdge(Projectile, 0, lightColor, BlueMode ? TextureBlue : null);
            return false;
        }

        // God Slayer Slugs explode on death, even if they never visually turned blue.
        public override void OnKill(int timeLeft)
        {
            // Turn blue to set stats correctly, if not already done.
            if (!BlueMode)
                TurnBlue(false);
            Projectile.ExpandHitboxBy(48);
            Projectile.Damage();

            // Create a fancy triangle of dust.
            int dustID = (int)CalamityDusts.BlueCosmilite;
            int numDust = 9;
            float triangleAngle = Main.rand.NextFloat(MathHelper.TwoPi);
            for (int i = 0; i < numDust; ++i)
            {
                float speed = MathHelper.Lerp(0.2f, 3.6f, i / (float)(numDust - 1));
                Vector2 dustVel = Vector2.UnitX.RotatedBy(triangleAngle) * speed;
                Dust d = Dust.NewDustDirect(Projectile.Center, 0, 0, dustID);
                d.position = Projectile.Center;
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
            if (Projectile.numHits > 0 && !BlueMode)
            {
                TurnBlue(true);
                return false;
            }

            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            return true;
        }

        private void ProduceWarpCrossDust(Vector2 dustPos, int dustID)
        {
            for (int i = 0; i < 4; ++i)
            {
                float speed = Main.rand.NextFloat(2.0f, 4.1f);
                Vector2 dustVel = Vector2.UnitX * speed;
                Dust d = Dust.NewDustDirect(Projectile.Center, 0, 0, dustID);
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
