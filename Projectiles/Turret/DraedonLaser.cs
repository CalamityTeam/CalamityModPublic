using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Sounds;

namespace CalamityMod.Projectiles.Turret
{
    public class DraedonLaser : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/LaserProj";

        // The DrawBeam method relies on localAI[0] for its calculations. A different parameter won't work.
        public float TrailLength
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        public const int MaxTrailPoints = 50;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lab Turret Laser");
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 4;
            Projectile.extraUpdates = 4;
            Projectile.timeLeft = 240;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }
        public override bool PreAI()
        {
            // If projectile knockback is set to 0 in the tile entity file, projectile hits players instead
            // This is used to check if the projectile came from the hostile version of the tile entity
            if (Projectile.knockBack == 0f)
                Projectile.hostile = true;
            else Projectile.friendly = true;
            return true;
        }
        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                // play a sound frame 1. changed this from space gun sound because that sound was way too annoying
                var sound = SoundEngine.PlaySound(CommonCalamitySounds.LaserCannonSound with { Volume = CommonCalamitySounds.LaserCannonSound.Volume * 0.23f}, Projectile.Center);

                Projectile.localAI[0] = 1f;
            }
            Projectile.alpha = (int)(Math.Sin(Projectile.timeLeft / 240f * MathHelper.Pi) * 1.6f * 255f);
            if (Projectile.alpha > 255)
                Projectile.alpha = 255;
            TrailLength += 1.5f;
            if (TrailLength > MaxTrailPoints)
            {
                TrailLength = MaxTrailPoints;
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 190, 255, 0);

        public override bool PreDraw(ref Color lightColor) => Projectile.DrawBeam(MaxTrailPoints, 1.5f, lightColor);
    }
}
