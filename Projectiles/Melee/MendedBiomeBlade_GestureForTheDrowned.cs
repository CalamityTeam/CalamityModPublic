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
    public class GestureForTheDrowned : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Items/Weapons/Melee/TrueBiomeBlade";
        Vector2 direction = Vector2.Zero;
        public Player Owner => Main.player[Projectile.owner];
        public float Timer => 40 - Projectile.timeLeft;
        public float HalfTimer => (Timer % 20);

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 40;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            if (Math.Abs(HalfTimer / 20f) == 0.5f)
            {
                SoundEngine.PlaySound(SoundID.Splash with { PitchVariance = 2f }, Projectile.Center);

                if (Owner.whoAmI == Main.myPlayer)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, direction * 0.1f + Owner.velocity, ProjectileType<GestureForTheDrownedOrb>(), Projectile.damage, 0, Owner.whoAmI);
                }

                Particle Sparkle = new CritSpark(Projectile.Center, Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(7.5f, 20f), Color.White, Main.rand.NextBool() ? Color.CornflowerBlue : Color.DodgerBlue, 0.1f + Main.rand.NextFloat(0f, 1.5f), 20 + Main.rand.Next(30), 1, 3f);
                GeneralParticleHandler.SpawnParticle(Sparkle);

            }


            direction = Vector2.UnitY.RotatedBy((MathHelper.PiOver4 + Utils.AngleLerp(0f, MathHelper.PiOver4 * 1.5f, HalfTimer / 20f)) * Math.Sign(Timer - 1 - 20f)) * (float)Math.Sin(HalfTimer / 20f * MathHelper.Pi) * 30;
            Owner.ChangeDir(Math.Sign(direction.X));
            Owner.itemRotation = direction.ToRotation();
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= MathHelper.Pi;
            }

            Projectile.Center = Owner.Center + direction;
            Projectile.rotation = direction.ToRotation() + MathHelper.PiOver2;
            Projectile.spriteDirection = Math.Sign(Timer - 20f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D blade = Request<Texture2D>("CalamityMod/Items/Weapons/Melee/TrueBiomeBlade").Value;

            float drawAngle = direction.ToRotation();
            float drawRotation = drawAngle + MathHelper.PiOver4;
            Vector2 drawOrigin = new Vector2(0f, blade.Height);
            Vector2 drawOffset = Projectile.Center - Main.screenPosition;

            Main.EntitySpriteDraw(blade, drawOffset, null, lightColor, drawRotation, drawOrigin, Projectile.scale, 0f, 0);
            return false;
        }

    }
}
