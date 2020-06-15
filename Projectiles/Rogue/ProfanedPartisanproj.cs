using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Rogue
{
	public class ProfanedPartisanproj : ModProjectile
    {
    	public int stealthStrikeTimer = 0;

    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Profaned Partisan");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = 5;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 3;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.Calamity().rogue = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (projectile.ai[0] < 0.4f)
            {
                projectile.ai[0] += 0.1f;
            }
            else
            {
                projectile.tileCollide = true;
            }

            //Backwards projectile fix
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
            //Rotating 45 degrees if shooting right
            if (projectile.spriteDirection == 1)
            {
                projectile.rotation += MathHelper.ToRadians(45f);
                drawOffsetX = -26;
                drawOriginOffsetX = 13;
                drawOriginOffsetY = 2;
            }
            //Rotating 45 degrees if shooting right
            if (projectile.spriteDirection == -1)
            {
                projectile.rotation -= MathHelper.ToRadians(45f);
                drawOffsetX = 2;
                drawOriginOffsetX = -13;
                drawOriginOffsetY = 2;
            }

            if (Main.rand.Next(3) == 0)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, projectile.velocity.X, projectile.velocity.Y, 100, default(Color), 1.1f);
                Main.dust[d].position = projectile.Center;
                Main.dust[d].velocity *= 0.3f;
                Main.dust[d].velocity += projectile.velocity * 0.85f;
            }
            Lighting.AddLight(projectile.Center, 1f, 0.8f, 0.2f);

            if (projectile.Calamity().stealthStrike) //Stealth strike
			{
				Vector2 spearposition = new Vector2(projectile.Center.X + Main.rand.NextFloat(-15f, 15f), projectile.Center.Y + Main.rand.NextFloat(-15f, 15f));
				Vector2 spearspeed = new Vector2(projectile.velocity.X, projectile.velocity.Y);
				stealthStrikeTimer++;
				if (stealthStrikeTimer >= 18)
				{
					Projectile.NewProjectile(spearposition, spearspeed, ModContent.ProjectileType<ProfanedPartisanspear>(), projectile.damage/10, projectile.knockBack, projectile.owner, 0f, 0f);
					stealthStrikeTimer = 0;
				}
			}
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 50, default(Color), 2.6f);
            }
            Main.PlaySound(SoundID.Item45, projectile.position);
            Vector2 explode = new Vector2(0f, 0f);
            Projectile.NewProjectile(projectile.Center, explode, ModContent.ProjectileType<PartisanExplosion>(), projectile.damage/2, projectile.knockBack * 1.3f, projectile.owner);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
        }
    }
}
