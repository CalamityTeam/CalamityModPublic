using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class AndromedaRegislash : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Regislash");
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 582;
            projectile.height = 304;
			projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.light = 3f;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
        }

		public override void AI()
		{
            Player player = Main.player[projectile.owner];
            if (projectile.localAI[0] == 0f)
            {
                if (CalamityUtils.CountProjectiles(projectile.type) > 1)
                {
                    projectile.Kill();
                    return;
                }
                Main.PlaySound(SoundID.DD2_DrakinShot, projectile.Center);
                projectile.rotation = projectile.AngleTo(Main.MouseWorld);
                projectile.localAI[0] = 1f;
            }
            projectile.position = player.Center - projectile.Size / 2f;
            if (Math.Abs(Math.Cos(projectile.rotation)) > 0.675f)
            {
                projectile.position.X += Math.Sign(Math.Cos(projectile.rotation)) * 295f;
            }
            projectile.position.Y += (float)Math.Sin(projectile.rotation) * 325f;
            projectile.frameCounter++;
            if (projectile.frameCounter % 7 == 6)
            {
                projectile.frame++;
            }
			if (projectile.frame >= Main.projFrames[projectile.type])
			{
                projectile.Kill();
			}
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type]);
            return false;
        }
    }
}
