using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.StatDebuffs;

namespace CalamityMod.Projectiles.Magic
{
    public class SlitheringEelProjectile : ModProjectile
    {
        public struct EelSegment
        {
            public Vector2 CurrentPosition;
            public float Rotation;
            public EelSegment(Vector2 position, float rotation)
            {
                CurrentPosition = position;
                Rotation = rotation;
            }
        }

        public EelSegment[] Segments = new EelSegment[10];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eel");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 22;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 360;
            projectile.Opacity = 0f;
        }

        public override void AI()
        {
            projectile.spriteDirection = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation();
            if (projectile.numHits >= 3f)
            {
                projectile.alpha += 5;
                if (projectile.alpha >= 255)
                    projectile.Kill();
            }
            else
                projectile.Opacity = MathHelper.Clamp(projectile.Opacity + 0.1f, 0f, 1f);

            if (projectile.timeLeft % 80f < 35f && projectile.Distance(Main.MouseWorld) > 70f)
            {
                float angleToTarget = projectile.AngleTo(Main.MouseWorld);
                float angleDifference = MathHelper.WrapAngle(angleToTarget - projectile.velocity.ToRotation());
                projectile.velocity = projectile.velocity.RotatedBy(angleDifference / 9f);
            }

            if (projectile.timeLeft % 65f == 64f)
                Projectile.NewProjectile(projectile.Center, Vector2.UnitY * 7f, ModContent.ProjectileType<EelDrop>(), projectile.damage / 2, 2f, projectile.owner);

            UpdateSegments();
        }

        public void UpdateSegments()
        {
            Vector2 aheadPosition = projectile.Center;
            float aheadRotation = projectile.rotation;

            // Connect all segments to each-other and calculate their rotations.
            for (int i = 0; i < Segments.Length; i++)
            {
                Vector2 offsetToDestination = aheadPosition - Segments[i].CurrentPosition;

                // This variant of segment attachment incorporates rotation.
                // Given the fact that all segments will execute this code is succession, the
                // result across the entire worm will exponentially decay over each segment,
                // allowing for smooth rotations. This code is what the stardust dragon uses for its segmenting.
                if (aheadRotation != projectile.rotation)
                {
                    float offsetAngle = MathHelper.WrapAngle(aheadRotation - projectile.rotation) * 0.03f;
                    offsetToDestination = offsetToDestination.RotatedBy(offsetAngle);
                }
                Segments[i].Rotation = (aheadPosition - Segments[i].CurrentPosition).ToRotation();
                Segments[i].CurrentPosition = aheadPosition - offsetToDestination.SafeNormalize(Vector2.UnitY) * 13f;

                // Adjust the ahead positions before interating further.
                aheadPosition = Segments[i].CurrentPosition;
                aheadRotation = Segments[i].Rotation;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D headTexture = ModContent.GetTexture("CalamityMod/Projectiles/Magic/SlitheringEelProjectile");
            Texture2D bodyTexture = ModContent.GetTexture("CalamityMod/Projectiles/Magic/SlitheringEelBody");
            Texture2D bodyTexture2 = ModContent.GetTexture("CalamityMod/Projectiles/Magic/SlitheringEelBody2");
            Texture2D tailTexture = ModContent.GetTexture("CalamityMod/Projectiles/Magic/SlitheringEelTail");

            Vector2 drawPosition;
            lightColor = new Color(255, 255, 255, 127);
            for (int i = 0; i < Segments.Length; i++)
            {
                Texture2D textureToUse = i % 2 == 1 ? bodyTexture2 : bodyTexture;

                // Use the tail texture for the last segment.
                if (i == Segments.Length - 1)
                    textureToUse = tailTexture;

                drawPosition = Segments[i].CurrentPosition - Main.screenPosition;
                spriteBatch.Draw(textureToUse, drawPosition, null, projectile.GetAlpha(lightColor), Segments[i].Rotation, textureToUse.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0f);
            }

            drawPosition = projectile.Center - Main.screenPosition;
            spriteBatch.Draw(headTexture, drawPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, headTexture.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int i = 0; i < Segments.Length; i++)
            {
                if (targetHitbox.Intersects(Utils.CenteredRectangle(Segments[i].CurrentPosition, Vector2.One * 12f)))
                    return true;
            }
            return null;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 23; i++)
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f);
        }
    }
}
