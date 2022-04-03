using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Enemy
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
            DisplayName.SetDefault("Lab Turret Laser");
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 4;
            Projectile.timeLeft = 240;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                // play a sound frame 1. changed this from space gun sound because that sound was way too annoying
                var sound = SoundEngine.PlaySound(Mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon"), Projectile.Center);
                if (sound != null)
                    sound.Volume *= 0.35f;

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

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => Projectile.DrawBeam(MaxTrailPoints, 1.5f, lightColor);
    }
}
