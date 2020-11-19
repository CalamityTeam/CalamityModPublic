using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class PhantomLanceProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/PhantomLance";

        private int projCount = 22;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom Lance");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 300;
            projectile.extraUpdates = 1;
            projectile.Calamity().rogue = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void AI()
        {
			if (projectile.Calamity().stealthStrike != true)
			{
				if (projectile.timeLeft <= 255)
					projectile.alpha += 1;
				if (projectile.timeLeft >= 75)
				{
					projectile.velocity.X *= 0.995f;
					projectile.velocity.Y *= 0.995f;
				}
			}
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 0.785f;
            Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 175, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 0, default, 0.85f);
			projCount--;
			if (projCount <= 0)
			{
				if (projectile.owner == Main.myPlayer)
				{
					if (projectile.Calamity().stealthStrike)
					{
                        int stealthSoulDamage = (int)(projectile.damage * 0.7f);
                        float stealthSoulKB = projectile.knockBack;
                        int stealthSoul = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<Phantom>(), stealthSoulDamage, stealthSoulKB, projectile.owner, 0f, 0f);
						Main.projectile[stealthSoul].Calamity().forceRogue = true;
						Main.projectile[stealthSoul].usesLocalNPCImmunity = true;
						Main.projectile[stealthSoul].localNPCHitCooldown = -2;
					}
					else
					{
                        float damageMult = projectile.timeLeft * 0.7f / 300f;
                        int soulDamage = (int)(projectile.damage * damageMult);
                        float soulKB = projectile.knockBack;
						int soul = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<Phantom>(), soulDamage, soulKB, projectile.owner, 0f, 0f);
						Main.projectile[soul].Calamity().forceRogue = true;
						Main.projectile[soul].usesLocalNPCImmunity = true;
						Main.projectile[soul].localNPCHitCooldown = -2;
					}
				}
				projCount = 18;
			}
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i <= 10; i++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 175, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
