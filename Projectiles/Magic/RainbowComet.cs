using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class RainbowComet : ModProjectile
    {
        public ref float Time => ref Projectile.ai[0];
        public const float FadeinTime = 40f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Comet");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 72;
            Projectile.height = 72;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.alpha = 255;
            Projectile.localNPCHitCooldown = 16;
        }

        public override void AI()
        {
            Time++;
            Projectile.Opacity = Utils.InverseLerp(0f, 20f, Time, true);
            Projectile.velocity = Projectile.velocity.RotatedBy(Math.Sin(Time / 30f) * 0.0125f);
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            Projectile.frameCounter++;
            if (Projectile.frameCounter % 4 == 3)
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D cometTexture = ModContent.Request<Texture2D>(Texture);
            spriteBatch.Draw(cometTexture,
                             Projectile.Center + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition,
                             cometTexture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame),
                             Color.White * Projectile.Opacity,
                             Projectile.rotation,
                             cometTexture.Size() * 0.5f,
                             Projectile.scale,
                             SpriteEffects.None,
                             0f);
            return false;
        }
        public override void Kill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                // Generate pretty sparkles.
                for (int i = 0; i < 18; i++)
                {
                    Vector2 velocity = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(9f, 20f);
                    Projectile.NewProjectile(Projectile.Center, velocity, ModContent.ProjectileType<PartySparkle>(), Projectile.damage, 1f, Projectile.owner);
                }

                // And release a bunch of rockets.
                for (int i = 0; i < (int)RainbowRocket.PartyCannonExplosionType.Count; i++)
                {
                    Vector2 velocity = -Projectile.velocity.SafeNormalize(-Vector2.UnitY);
                    velocity = velocity.RotatedBy(MathHelper.Lerp(-1.1f, 1.1f, i / (float)(int)RainbowRocket.PartyCannonExplosionType.Count));
                    velocity *= Main.rand.NextFloat(7f, 15f);

                    Projectile rocket = Projectile.NewProjectileDirect(Projectile.Center, velocity, ModContent.ProjectileType<RainbowRocket>(), Projectile.damage * 3, 1f, Projectile.owner);
                    rocket.ai[1] = i;
                }
            }

            if (!Main.dedServ)
            {
                for (int i = 0; i < 80; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, 263);
                    dust.color = CalamityUtils.MulticolorLerp((Main.rand.NextFloat(0.4f) + Main.GlobalTime * 0.4f) % 0.999f, RainbowPartyCannon.ColorSet);
                    dust.scale = Main.rand.NextFloat(0.6f, 0.9f);
                    dust.velocity = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(12f, 16f);
                    dust.noGravity = true;
                }

                SoundEngine.PlaySound(Mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LargeWeaponFire").WithVolume(0.45f), Projectile.Center);
            }
        }
    }
}
