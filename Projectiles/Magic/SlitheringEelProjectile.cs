using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.StatDebuffs;

namespace CalamityMod.Projectiles.Magic
{
    public class SlitheringEelProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
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
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 360;
            Projectile.Opacity = 0f;
        }

        public override void AI()
        {
            Projectile.spriteDirection = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.numHits >= 3f)
            {
                Projectile.alpha += 5;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            else
                Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity + 0.1f, 0f, 1f);

            if (Projectile.timeLeft % 80f < 35f && Projectile.Distance(Main.MouseWorld) > 70f)
            {
                float angleToTarget = Projectile.AngleTo(Main.MouseWorld);
                float angleDifference = MathHelper.WrapAngle(angleToTarget - Projectile.velocity.ToRotation());
                Projectile.velocity = Projectile.velocity.RotatedBy(angleDifference / 9f);
            }

            if (Projectile.timeLeft % 65f == 64f)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitY * 7f, ModContent.ProjectileType<EelDrop>(), Projectile.damage / 2, 2f, Projectile.owner);

            UpdateSegments();
        }

        public void UpdateSegments()
        {
            Vector2 aheadPosition = Projectile.Center;
            float aheadRotation = Projectile.rotation;

            // Connect all segments to each-other and calculate their rotations.
            for (int i = 0; i < Segments.Length; i++)
            {
                Vector2 offsetToDestination = aheadPosition - Segments[i].CurrentPosition;

                // This variant of segment attachment incorporates rotation.
                // Given the fact that all segments will execute this code is succession, the
                // result across the entire worm will exponentially decay over each segment,
                // allowing for smooth rotations. This code is what the stardust dragon uses for its segmenting.
                if (aheadRotation != Projectile.rotation)
                {
                    float offsetAngle = MathHelper.WrapAngle(aheadRotation - Projectile.rotation) * 0.03f;
                    offsetToDestination = offsetToDestination.RotatedBy(offsetAngle);
                }
                Segments[i].Rotation = (aheadPosition - Segments[i].CurrentPosition).ToRotation();
                Segments[i].CurrentPosition = aheadPosition - offsetToDestination.SafeNormalize(Vector2.UnitY) * 13f;

                // Adjust the ahead positions before interating further.
                aheadPosition = Segments[i].CurrentPosition;
                aheadRotation = Segments[i].Rotation;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D headTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/SlitheringEelProjectile").Value;
            Texture2D bodyTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/SlitheringEelBody").Value;
            Texture2D bodyTexture2 = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/SlitheringEelBody2").Value;
            Texture2D tailTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/SlitheringEelTail").Value;

            Vector2 drawPosition;
            lightColor = new Color(255, 255, 255, 127);
            for (int i = 0; i < Segments.Length; i++)
            {
                Texture2D textureToUse = i % 2 == 1 ? bodyTexture2 : bodyTexture;

                // Use the tail texture for the last segment.
                if (i == Segments.Length - 1)
                    textureToUse = tailTexture;

                drawPosition = Segments[i].CurrentPosition - Main.screenPosition;
                Main.EntitySpriteDraw(textureToUse, drawPosition, null, Projectile.GetAlpha(lightColor), Segments[i].Rotation, textureToUse.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            }

            drawPosition = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(headTexture, drawPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, headTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 23; i++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f);
        }
    }
}
