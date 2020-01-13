using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class CardClubSplit : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Club");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.ranged = true;
            projectile.extraUpdates = 1;
            projectile.aiStyle = 1;
            aiType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.5f / 255f, (255 - projectile.alpha) * 0f / 255f, (255 - projectile.alpha) * 0f / 255f);
			projectile.rotation -= (MathHelper.ToRadians(90) * projectile.direction);
			projectile.spriteDirection = projectile.direction;
            if (Main.rand.NextBool(2))
            {
                int num137 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), 1, 1, 30, 0f, 0f, 0, default, 0.5f);
                Main.dust[num137].velocity *= 0f;
                Main.dust[num137].noGravity = true;
            }

			if (projectile.alpha < 128)
			{
                float pcx = projectile.Center.X;
                float pcy = projectile.Center.Y;
                float var1 = 800f;
                bool flag = false;
                for (int npcvar = 0; npcvar < Main.npc.Length; npcvar++)
                {
                    if (Main.npc[npcvar].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[npcvar].Center, 1, 1))
                    {
                        float var2 = Main.npc[npcvar].position.X + (Main.npc[npcvar].width / 2);
                        float var3 = Main.npc[npcvar].position.Y + (Main.npc[npcvar].height / 2);
                        float var4 = Math.Abs(projectile.position.X + (projectile.width / 2) - var2) + Math.Abs(projectile.position.Y + (projectile.height / 2) - var3);
                        if (var4 < var1)
                        {
                            var1 = var4;
                            pcx = var2;
                            pcy = var3;
                            flag = true;
                        }
                    }
                }
                if (flag)
                {
                    float homingstrenght = 12f;
                    Vector2 vector1 = new Vector2(projectile.position.X + projectile.width * 0.5f, projectile.position.Y + projectile.height * 0.5f);
                    float var6 = pcx - vector1.X;
                    float var7 = pcy - vector1.Y;
                    float var8 = (float)Math.Sqrt(var6 * var6 + var7 * var7);
                    var8 = homingstrenght / var8;
                    var6 *= var8;
                    var7 *= var8;
                    projectile.velocity.X = (projectile.velocity.X * 20f + var6) / 21f;
                    projectile.velocity.Y = (projectile.velocity.Y * 20f + var7) / 21f;
                }
			}
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(0, (int)projectile.position.X, (int)projectile.position.Y, 1, 1f, 0f);
            return true;
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 2; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 30, projectile.oldVelocity.X * 0.15f, projectile.oldVelocity.Y * 0.15f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 360);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

		public override bool CanDamage()
		{
			return projectile.alpha < 128;
		}
    }
}
