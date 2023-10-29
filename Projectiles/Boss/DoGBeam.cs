using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class DoGBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.hostile = true;
            Projectile.scale = 2f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 960;

            if (CalamityWorld.LegendaryMode && CalamityWorld.revenge)
                Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 1)
                Projectile.frame = 0;

            Lighting.AddLight(Projectile.Center, 0f, 0.2f, 0.3f);

            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + MathHelper.PiOver2;

            int playerTracker = Player.FindClosest(Projectile.Center, 1, 1);
            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] < 120f && Projectile.ai[1] > 30f)
            {
                float projSpeed = Projectile.velocity.Length();
                Vector2 playerDistance = Main.player[playerTracker].Center - Projectile.Center;
                playerDistance.Normalize();
                playerDistance *= projSpeed;
                Projectile.velocity = (Projectile.velocity * 20f + playerDistance) / 21f;
                Projectile.velocity.Normalize();
                Projectile.velocity *= projSpeed;
            }

            if (Projectile.timeLeft == 950)
                Projectile.damage = (int)Projectile.ai[0];
            if (Projectile.timeLeft < 30)
                Projectile.damage = 0;
        }

        public override bool CanHitPlayer(Player target)
        {
            if (Projectile.timeLeft > 950 || Projectile.timeLeft < 30)
            {
                return false;
            }
            return true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft > 950)
            {
                return new Color(0, 0, 0, 0);
            }
            if (Projectile.timeLeft < 30)
            {
                byte b2 = (byte)(Projectile.timeLeft * 8.5);
                byte a2 = (byte)(100f * ((float)b2 / 255f));
                return new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            return new Color(255, 255, 255, 100);
        }
    }
}
