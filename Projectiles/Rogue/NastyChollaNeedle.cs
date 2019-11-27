using Terraria;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Projectiles.Rogue
{
    public class NastyChollaNeedle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Needle");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.Calamity().rogue = true;
            projectile.penetrate = 3;
            projectile.timeLeft = 180;
            projectile.aiStyle = 1;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
			projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
			projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
			//Rotating 45 degrees if shooting right
			if (projectile.spriteDirection == 1)
			{
				projectile.rotation += MathHelper.ToRadians(45f);
			}
			//Rotating 45 degrees if shooting right
			if (projectile.spriteDirection == -1)
			{
				projectile.rotation -= MathHelper.ToRadians(45f);
			}
            projectile.velocity.X *= 0.9995f;
            projectile.velocity.Y = projectile.velocity.Y + 0.01f;
        }
    }
}
