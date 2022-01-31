using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class TheMaelstromShark : ModProjectile
    {
        public PrimitiveTrail LightningTrailDrawer = null;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaper Shark");
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 44;
            projectile.height = 44;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.arrow = true;
            projectile.penetrate = 1;
            projectile.Opacity = 0f;
            projectile.ranged = true;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            projectile.frame = projectile.frameCounter / 5 % Main.projFrames[projectile.type];

            projectile.rotation = projectile.velocity.ToRotation();
            projectile.spriteDirection = (Math.Cos(projectile.rotation) > 0f).ToDirectionInt();
            if (projectile.spriteDirection == -1)
                projectile.rotation += MathHelper.Pi;

            projectile.Opacity = MathHelper.Clamp(projectile.Opacity + 0.1f, 0f, 1f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y, 1, 1f, 0f);
            return true;
        }

        internal Color ColorFunction(float completionRatio)
        {
            float fadeOpacity = Utils.InverseLerp(0.94f, 0.54f, completionRatio, true) * projectile.Opacity;
            return Color.Lerp(Color.Cyan, Color.White, 0.4f) * fadeOpacity;
        }

        internal float WidthFunction(float completionRatio)
        {
            float expansionCompletion = 1f - (float)Math.Pow(1f - Utils.InverseLerp(0f, 0.3f, completionRatio, true), 2D);
            return MathHelper.Lerp(0f, 12f * projectile.Opacity, expansionCompletion);
        }

        public override void Kill(int timeLeft)
        {
            // Create death effects for the shark, including a death sound, gore, and some blood.
            Main.PlaySound(SoundID.NPCDeath1, projectile.Center);
            Gore.NewGore(projectile.position, projectile.velocity, mod.GetGoreSlot("Gores/MaelstromReaperShark1"), projectile.scale);
            Gore.NewGore(projectile.position, projectile.velocity, mod.GetGoreSlot("Gores/MaelstromReaperShark2"), projectile.scale);
            Gore.NewGore(projectile.position, projectile.velocity, mod.GetGoreSlot("Gores/MaelstromReaperShark3"), projectile.scale);
            for (int i = 0; i < 12; i++)
            {
                Dust blood = Dust.NewDustPerfect(projectile.Center, 5);
                blood.velocity = Main.rand.NextVector2Circular(6f, 6f);
                blood.scale *= Main.rand.NextFloat(0.7f, 1.3f);
                blood.noGravity = true;
            }

            if (Main.myPlayer != projectile.owner)
                return;

            Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<TheMaelstromExplosion>(), projectile.damage, 0f, projectile.owner);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Rectangle frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 drawPosition = projectile.Center - Main.screenPosition;
            SpriteEffects direction = projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (LightningTrailDrawer is null)
                LightningTrailDrawer = new PrimitiveTrail(WidthFunction, ColorFunction, specialShader: GameShaders.Misc["CalamityMod:TrailStreak"]);

            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.GetTexture("CalamityMod/ExtraTextures/ScarletDevilStreak"));
            LightningTrailDrawer.Draw(projectile.oldPos, projectile.Size * 0.5f - Main.screenPosition, 60);
            spriteBatch.Draw(texture, drawPosition, frame, projectile.GetAlpha(lightColor), projectile.rotation, origin, projectile.scale, direction, 0f);
            return false;
        }
    }
}
