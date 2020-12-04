using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class RainbowComet : ModProjectile
    {
        public ref float Time => ref projectile.ai[0];
        public const float FadeinTime = 40f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Comet");
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 12;
            Main.projFrames[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = 72;
            projectile.height = 72;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.magic = true;
            projectile.timeLeft = 300;
            projectile.usesLocalNPCImmunity = true;
            projectile.alpha = 255;
            projectile.localNPCHitCooldown = 16;
        }

        public override void AI()
        {
            Time++;
            projectile.Opacity = Utils.InverseLerp(0f, 20f, Time, true);
            projectile.velocity = projectile.velocity.RotatedBy(Math.Sin(Time / 30f) * 0.0125f);
            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;

            projectile.frameCounter++;
            if (projectile.frameCounter % 4 == 3)
                projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D cometTexture = ModContent.GetTexture(Texture);
            spriteBatch.Draw(cometTexture,
                             projectile.Center + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition,
                             cometTexture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame),
                             Color.White * projectile.Opacity,
                             projectile.rotation,
                             cometTexture.Size() * 0.5f,
                             projectile.scale,
                             SpriteEffects.None,
                             0f);
            return false;
        }
        public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
            {
                // Generate pretty sparkles.
                for (int i = 0; i < 18; i++)
                {
                    Vector2 velocity = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(9f, 20f);
                    Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<PartySparkle>(), projectile.damage, 1f, projectile.owner);
                }

                // And release a bunch of rockets.
                for (int i = 0; i < (int)RainbowRocket.PartyCannonExplosionType.Count; i++)
                {
                    Vector2 velocity = -projectile.velocity.SafeNormalize(-Vector2.UnitY);
                    velocity = velocity.RotatedBy(MathHelper.Lerp(-1.1f, 1.1f, i / (float)(int)RainbowRocket.PartyCannonExplosionType.Count));
                    velocity *= Main.rand.NextFloat(7f, 15f);

                    Projectile rocket = Projectile.NewProjectileDirect(projectile.Center, velocity, ModContent.ProjectileType<RainbowRocket>(), projectile.damage * 3, 1f, projectile.owner);
                    rocket.ai[1] = i;
                }
            }

            if (!Main.dedServ)
            {
                for (int i = 0; i < 80; i++)
                {
                    Dust dust = Dust.NewDustPerfect(projectile.Center, 263);
                    dust.color = CalamityUtils.MulticolorLerp((Main.rand.NextFloat(0.4f) + Main.GlobalTime * 0.4f) % 0.999f, RainbowPartyCannon.ColorSet);
                    dust.scale = Main.rand.NextFloat(0.6f, 0.9f);
                    dust.velocity = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(12f, 16f);
                    dust.noGravity = true;
                }

                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LargeWeaponFire").WithVolume(0.45f), projectile.Center);
            }
        }
    }
}
