using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class GestureForTheDrowned : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Melee/TrueBiomeBlade";
        Vector2 direction = Vector2.Zero;
        public Player Owner => Main.player[projectile.owner];
        public float Timer => 40 - projectile.timeLeft;
        public float HalfTimer => (Timer % 20);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Biome Blade"); //This is litterally just the biome blade without even any extra magic effects so
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 32;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 40;
            projectile.melee = true;
            projectile.tileCollide = false;
        }

        public override bool CanDamage() => false;

        public override void AI()
        {
            if (Math.Abs(HalfTimer / 20f) == 0.5f)
            {
                var splash = new LegacySoundStyle(SoundID.Splash, 0).WithPitchVariance(Main.rand.NextFloat());
                Main.PlaySound(splash, projectile.Center);

                if (Owner.whoAmI == Main.myPlayer)
                {
                    Projectile.NewProjectile(Owner.Center, direction * 0.1f + Owner.velocity, ProjectileType<GestureForTheDrownedOrb>(), projectile.damage, 0, Owner.whoAmI);
                }

                Particle Sparkle = new CritSpark(projectile.Center, Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(7.5f, 20f), Color.White, Main.rand.NextBool() ? Color.CornflowerBlue : Color.DodgerBlue, 0.1f + Main.rand.NextFloat(0f, 1.5f), 20 + Main.rand.Next(30), 1, 3f);
                GeneralParticleHandler.SpawnParticle(Sparkle);

            }


            direction = Vector2.UnitY.RotatedBy((MathHelper.PiOver4 + Utils.AngleLerp(0f, MathHelper.PiOver4 * 1.5f, HalfTimer / 20f)) * Math.Sign(Timer - 1 - 20f)) * (float)Math.Sin(HalfTimer / 20f * MathHelper.Pi) * 30;
            Owner.direction = Math.Sign(direction.X);
            Owner.itemRotation = direction.ToRotation();
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= MathHelper.Pi;
            }

            projectile.Center = Owner.Center + direction;
            projectile.rotation = direction.ToRotation() + MathHelper.PiOver2;
            projectile.spriteDirection = Math.Sign(Timer - 20f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D blade = GetTexture("CalamityMod/Items/Weapons/Melee/TrueBiomeBlade");

            float drawAngle = direction.ToRotation();
            float drawRotation = drawAngle + MathHelper.PiOver4;
            Vector2 drawOrigin = new Vector2(0f, blade.Height);
            Vector2 drawOffset = projectile.Center - Main.screenPosition;

            spriteBatch.Draw(blade, drawOffset, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
            return false;
        }

    }
}
