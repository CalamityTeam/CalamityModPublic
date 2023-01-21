using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Sounds;
using CalamityMod.Projectiles.Typeless;

namespace CalamityMod.Projectiles.Turret
{
    public class WaterShot : ModProjectile
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
            DisplayName.SetDefault("Water Shot");
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 4;
            Projectile.timeLeft = 240;
        }


        public override void AI()
        {
            CheckCollision();
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

        public void CheckCollision()
        {
            if (Main.netMode == NetmodeID.SinglePlayer || Main.netMode == NetmodeID.Server)
            {
                var source = Main.player[Main.myPlayer].GetSource_FromThis();
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC target = Main.npc[i];
                    if (Projectile.position.X < target.position.X + target.width && Projectile.position.X + Projectile.width > target.position.X && Projectile.position.Y < target.position.Y + target.height && Projectile.position.Y + Projectile.height > target.position.Y && Projectile.penetrate > 0)
                    {
                        Projectile.NewProjectile(source, target.Center, new Vector2(0f), ModContent.ProjectileType<DirectStrike>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, i);
                        Projectile.penetrate--;
                    }
                }
            }
        }
        public override Color? GetAlpha(Color lightColor) => new Color(255, 190, 255, 0);

        public override bool PreDraw(ref Color lightColor) => Projectile.DrawBeam(MaxTrailPoints, 1.5f, lightColor);
    }
}
