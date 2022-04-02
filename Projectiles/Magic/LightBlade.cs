using CalamityMod.CalPlayer;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class LightBlade : ModProjectile
    {
        private const int Lifetime = 300;
        private const int NumAfterimages = 8;
        private const float LightBrightness = 0.7f;
        private const int DustID = 175;
        const float SwordHomingStrength = 30f;
        const float EnemyHomingStrength = 70f;

        private Color lightColor = Color.White;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Light Blade");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = NumAfterimages;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.scale = 1.5f;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.penetrate = 2;
            projectile.extraUpdates = 2;
            projectile.timeLeft = Lifetime;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            // The sprite is diagonal, so rotate by pi/4
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;

            // These effects only occur on the first frame. Alpha is set to 0 immediately on frame 1.
            if (projectile.alpha > 0)
            {
                projectile.alpha = 0;
                FirstFrameEffects();

                // Store original velocity for homing calculations
                projectile.ai[0] = projectile.velocity.Length();
            }

            // Every other frame, add less light.
            else
                Lighting.AddLight(projectile.Center, lightColor.ToVector3() * LightBrightness);

            // Home in on the paired sword, if one is defined and it exists
            if (projectile.ai[1] > 0f)
            {
                Projectile paired = Main.projectile[(int)(projectile.ai[1] - 1)];
                if (paired is null || !paired.active || paired.type != ModContent.ProjectileType<LightBlade>())
                    projectile.ai[1] = 0f;
                else
                {
                    Vector2 homingVec = projectile.SafeDirectionTo(paired.Center) * projectile.ai[0];
                    projectile.velocity = (projectile.velocity * (SwordHomingStrength - 1f) + homingVec) / SwordHomingStrength;
                }
            }

            // Also weakly home in on nearby enemies
            CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 200f, projectile.ai[0], EnemyHomingStrength);

            // The projectile never slows down, even as it tries to curve towards enemies and its paired sword.
            float currentSpeed = projectile.velocity.Length();
            projectile.velocity *= projectile.ai[0] / currentSpeed;
        }

        private void FirstFrameEffects()
        {
            // Pick a random sword sprite and a random color to use.
            projectile.frame = Main.rand.Next(14);
            lightColor = Judgement.GetRandomLightColor();

            // Spawn starting dust.
            Vector2 baseOffsetVec = new Vector2(1f, 4f);
            int numDust = 16;
            for(int i = 0; i < numDust; ++i)
            {
                Vector2 dustOffset = Vector2.UnitY.RotatedBy(i * MathHelper.TwoPi / numDust) * baseOffsetVec;
                dustOffset = dustOffset.RotatedBy(projectile.velocity.ToRotation());
                Dust d = Dust.NewDustDirect(projectile.Center, 0, 0, DustID, 0f, 0f);
                d.position = projectile.Center + dustOffset;
                d.velocity = dustOffset.SafeNormalize(Vector2.Zero);
                d.velocity *= 1.4f;
                d.scale = 1.5f;
                d.noGravity = true;
            }

            // Each blade appears in a huge flash of light.
            float startingBrightness = LightBrightness * 3f;
            Lighting.AddLight(projectile.Center, lightColor.ToVector3() * startingBrightness);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer calPlayer = player.Calamity();
            calPlayer.danceOfLightCharge++;
            if (calPlayer.danceOfLightCharge >= Judgement.HitsPerFlash)
            {
                calPlayer.danceOfLightCharge = 0;
                if (projectile.owner == Main.myPlayer)
                {
                    int flashDamage = (int)(Judgement.FlashBaseDamage * player.MagicDamage());
                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<BlindingLight>(), flashDamage, 0f, projectile.owner);
                }
            }

        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.NPCHit3, projectile.position);
            int numDust = Main.rand.Next(4, 10);
            for (int i = 0; i < numDust; i++)
            {
                Dust d = Dust.NewDustDirect(projectile.Center, 0, 0, DustID, 0f, 0f, 100);
                d.velocity *= 1.6f;
                d.velocity += projectile.velocity * Main.rand.NextFloat(-0.5f, 0.5f);
                d.velocity.Y += -1f;
                d.noGravity = true;
                d.scale = 2f;
                d.fadeIn = 0.5f;
            }
        }

        public override Color? GetAlpha(Color lightColor) => lightColor;

        // Uses bizarre drawcode because it has a horizontal sprite sheet
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Point projTile = projectile.Center.ToTileCoordinates();
            Color localLight = Lighting.GetColor(projTile.X, projTile.Y);

            Texture2D tex = Main.projectileTexture[projectile.type];
            Rectangle rect = new Rectangle(38 * projectile.frame, 0, 38, 38);
            Vector2 halfSpriteSize = rect.Size() / 2f;

            // Draw the projectile itself
            Color mainSwordColor = projectile.GetAlpha(localLight);
            mainSwordColor.A = 160;
            SpriteEffects sfx = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 drawPos = projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
            spriteBatch.Draw(tex, drawPos, new Rectangle?(rect), mainSwordColor, projectile.rotation, halfSpriteSize, projectile.scale, sfx, 0f);

            // Draw the projectile's afterimages
            for (int i = 0; i < NumAfterimages; ++i)
            {
                Color afterimageLight = projectile.GetAlpha(localLight);
                afterimageLight.A = 40;

                float posLerp = 1f;
                Vector2 afterimagePos = (1f - posLerp) * projectile.position + posLerp * projectile.oldPos[i];
                float rotation = projectile.oldRot[i];
                SpriteEffects afterimageSfx = projectile.oldSpriteDirection[i] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                float afterimageScale = MathHelper.Lerp(1.2f * projectile.scale, 0.4f * projectile.scale, i / (NumAfterimages - 1f));

                Vector2 imageDrawPos = afterimagePos + projectile.Size / 2f - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
                spriteBatch.Draw(tex, imageDrawPos, new Rectangle?(rect), afterimageLight, rotation, halfSpriteSize, afterimageScale, afterimageSfx, 0f);
            }

            return false;
        }
    }
}
