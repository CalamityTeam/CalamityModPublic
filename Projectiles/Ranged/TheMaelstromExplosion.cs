using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class TheMaelstromExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/ExtraTextures/SmallGreyscaleCircle";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 75;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.scale = 0.1f;

            // Hardcoded spaghetti appearss to make the scale setting above tamper with the hitbox.
            Projectile.width = Projectile.height = (int)(120 / Projectile.scale);
            Projectile.timeLeft = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;
        }

        public override void AI()
        {
            Projectile.scale = MathHelper.Lerp(Projectile.scale, 1f, 0.125f);
            Projectile.Opacity = Projectile.scale * Utils.GetLerpValue(0f, 30f, Projectile.timeLeft, true);

            // Emit sparks and dust.
            if (Main.netMode != NetmodeID.Server)
            {
                int sparkLifetime = Main.rand.Next(22, 36);
                float sparkScale = Main.rand.NextFloat(1f, 1.3f);
                Color sparkColor = Color.Lerp(Color.Cyan, Color.DarkBlue, Main.rand.NextFloat(0.7f));
                Vector2 sparkVelocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(3f, 8f);

                SparkParticle spark = new SparkParticle(Projectile.Center, sparkVelocity, false, sparkLifetime, sparkScale, sparkColor);
                GeneralParticleHandler.SpawnParticle(spark);

                Vector2 dustSpawnOffset = Main.rand.NextVector2Circular(Projectile.width, Projectile.height) * Projectile.scale * 0.4f;
                Dust electricity = Dust.NewDustPerfect(Projectile.Center + dustSpawnOffset, 267);
                electricity.color = Color.Cyan;
                electricity.color.A = 84;
                electricity.scale *= Main.rand.NextFloat(0.7f, 1.2f);
                electricity.velocity = dustSpawnOffset.SafeNormalize(Vector2.UnitY).RotatedBy(MathHelper.PiOver2).RotatedByRandom(0.37f);
                electricity.velocity *= Main.rand.NextFloat(2f, 6f);
                electricity.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<CrushDepth>(), 180);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<CrushDepth>(), 180);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = texture.Size() * 0.5f;

            float scale = Projectile.scale * Projectile.width / texture.Width;
            Color frontAfterimageColor = Projectile.GetAlpha(Color.Lerp(Color.Cyan, Color.DarkBlue, Projectile.identity / 7f % 0.8f)) * 0.2f;
            frontAfterimageColor.A = 0;
            for (int i = 0; i < 12; i++)
            {
                Vector2 drawOffset = (MathHelper.TwoPi * i / 12f + Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * 2f;
                Vector2 afterimageDrawPosition = Projectile.Center + drawOffset - Main.screenPosition;
                Main.EntitySpriteDraw(texture, afterimageDrawPosition, null, frontAfterimageColor, Projectile.rotation, origin, scale, SpriteEffects.None, 0);
            }
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.Size.Length() * Projectile.scale / 1.414f, targetHitbox);
        }

        public override bool? CanDamage() => Projectile.Opacity > 0.4f ? null : false;
    }
}
