using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Enemy
{
    public class LightningCloud : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cloud");
            Main.projFrames[projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            projectile.width = 54;
            projectile.height = 28;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
			projectile.hostile = true;
            projectile.timeLeft = 180;
            projectile.Opacity = 0f;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frameCounter = 0;
                projectile.frame++;

                int maxFrame = projectile.timeLeft < 60 ? 6 : 3; 
                if (projectile.frame >= maxFrame)
                    projectile.frame = 0;

                if (projectile.frame == 5 && Main.myPlayer == projectile.owner)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/LightningStrike"), projectile.Center);
                    float ai = Main.rand.Next(100);
                    Vector2 velocity = Vector2.UnitY * 7f;
                    Projectile.NewProjectile(projectile.Bottom, velocity, ProjectileID.CultistBossLightningOrbArc, projectile.damage, 0f, projectile.owner, MathHelper.PiOver2, ai);
                }
            }

            if (projectile.timeLeft < 30) 
                projectile.Opacity = MathHelper.Lerp(projectile.Opacity, 0f, 0.14f);
            else
                projectile.Opacity = MathHelper.Lerp(projectile.Opacity, 1f, 0.33f);
        }
    }
}
