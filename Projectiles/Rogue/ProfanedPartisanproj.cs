using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
	public class ProfanedPartisanproj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/ProfanedPartisan";

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

            if (Main.rand.NextBool(3))
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, (int)CalamityDusts.ProfanedFire, projectile.velocity.X, projectile.velocity.Y, 100, default(Color), 1.1f);
                Main.dust[d].position = projectile.Center;
                Main.dust[d].velocity *= 0.3f;
                Main.dust[d].velocity += projectile.velocity * 0.85f;
            }
            Lighting.AddLight(projectile.Center, 1f, 0.8f, 0.2f);

            if (projectile.Calamity().stealthStrike) //Stealth strike
			{
				Vector2 spearPosition = new Vector2(projectile.Center.X + Main.rand.NextFloat(-15f, 15f), projectile.Center.Y + Main.rand.NextFloat(-15f, 15f));
				Vector2 spearSpeed = projectile.velocity;
				if (projectile.timeLeft % 18 == 0)
				{
					Projectile.NewProjectile(spearPosition, spearSpeed, ModContent.ProjectileType<ProfanedPartisanspear>(), projectile.damage / 10, projectile.knockBack * 0.1f, projectile.owner);
				}
			}
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, (int)CalamityDusts.ProfanedFire, 0f, 0f, 50, default(Color), 2.6f);
            }
            Main.PlaySound(SoundID.Item45, projectile.position);
            Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<PartisanExplosion>(), projectile.damage / 2, projectile.knockBack * 0.5f, projectile.owner);
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
