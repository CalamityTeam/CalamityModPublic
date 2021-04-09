using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class InfernalSpearProjectile : ModProjectile
    {
        private float speedX = -3f;
        private float speedX2 = -5f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spear");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 62;
            projectile.height = 62;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 300;
            projectile.Calamity().rogue = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(speedX);
            writer.Write(speedX2);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            speedX = reader.ReadSingle();
            speedX2 = reader.ReadSingle();
        }

        public override void AI()
        {
			if (projectile.timeLeft % 12 == 0)
			{
				if (projectile.owner == Main.myPlayer)
				{
					if (projectile.Calamity().stealthStrike)
					{
						Projectile.NewProjectile(projectile.Center, projectile.velocity, ModContent.ProjectileType<InfernalFireball>(), (int)(projectile.damage * 0.75), projectile.knockBack, projectile.owner);
					}
				}
			}

            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;
			CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 200f, 12f, 20f);
        }

        public override void Kill(int timeLeft)
        {
			if (projectile.Calamity().stealthStrike)
			{
				if (projectile.owner == Main.myPlayer)
				{
					for (int x = 0; x < 3; x++)
					{
						Projectile.NewProjectile((int)projectile.Center.X, (int)projectile.Center.Y, speedX, -50f, ModContent.ProjectileType<InfernalFireballEruption>(), projectile.damage, 0f, projectile.owner, 0f, 0f);
						speedX += 3f;
					}
					for (int x = 0; x < 2; x++)
					{
						Projectile.NewProjectile((int)projectile.Center.X, (int)projectile.Center.Y, speedX2, -75f, ModContent.ProjectileType<InfernalFireballEruption>(), projectile.damage, 0f, projectile.owner, 0f, 0f);
						speedX2 += 10f;
					}
				}
			}
			CalamityGlobalProjectile.ExpandHitboxBy(projectile, 160);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            Main.PlaySound(SoundID.Item14, projectile.position);
            for (int d = 0; d < 20; d++)
            {
                int fire = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default, 1.2f);
                Main.dust[fire].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[fire].scale = 0.5f;
                    Main.dust[fire].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int d = 0; d < 30; d++)
            {
                int fire = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default, 1.7f);
                Main.dust[fire].noGravity = true;
                Main.dust[fire].velocity *= 5f;
                fire = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default, 1f);
                Main.dust[fire].velocity *= 2f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Daybreak, 360);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }
    }
}
