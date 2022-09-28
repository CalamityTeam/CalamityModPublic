using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class AtlasMunitionsLaser : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exo Pulse Laser");
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.timeLeft = 240;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.MaxUpdates = 3;
            Projectile.Opacity = 0f;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.Cyan.ToVector3() * Projectile.Opacity * 0.67f);

            Projectile.Opacity = Utils.GetLerpValue(240f, 235f, Projectile.timeLeft, true);            
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
                Projectile.frameCounter = 0;
            }

            // Accelerate.
            if (Projectile.velocity.Length() < 16f)
                Projectile.velocity *= 1.02f;
        }
    }
}
