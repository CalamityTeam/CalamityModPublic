using CalamityMod.Dusts;
using CalamityMod.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class DarkEnergyBall2 : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/DarkEnergyBall";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Energy");
            Main.projFrames[projectile.type] = 6;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

        public override void SetDefaults()
        {
			projectile.Calamity().canBreakPlayerDefense = true;
			projectile.width = 80;
            projectile.height = 80;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 600;
            projectile.penetrate = -1;
            cooldownSlot = 1;
        }

        public override void AI()
        {
			if (Vector2.Distance(projectile.Center, Main.npc[CalamityGlobalNPC.voidBoss].Center) < 80f)
				projectile.Kill();

			if (projectile.velocity.Length() < 10f)
				projectile.velocity *= 1.05f;

			projectile.Opacity = MathHelper.Clamp(1f - ((projectile.timeLeft - 570) / 30f), 0f, 1f);

			projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 5)
                projectile.frame = 0;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);

			Rectangle frame = new Rectangle(0, projectile.frame * Main.projectileTexture[projectile.type].Height, Main.projectileTexture[projectile.type].Width, Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]);

			Color color = Color.Lerp(Color.White, Color.Fuchsia, 0.5f) * projectile.Opacity;

			spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Boss/DarkEnergyBallGlow"), projectile.Center - Main.screenPosition, frame, color, projectile.rotation, projectile.Size / 2, 1f, SpriteEffects.None, 0f);

			color = Color.Lerp(Color.White, Color.Cyan, 0.5f) * projectile.Opacity;

			spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Boss/DarkEnergyBallGlow2"), projectile.Center - Main.screenPosition, frame, color, projectile.rotation, projectile.Size / 2, 1f, SpriteEffects.None, 0f);

			return false;
		}

		public override bool CanHitPlayer(Player target)
		{
			Rectangle targetHitbox = target.Hitbox;

			float dist1 = Vector2.Distance(projectile.Center, targetHitbox.TopLeft());
			float dist2 = Vector2.Distance(projectile.Center, targetHitbox.TopRight());
			float dist3 = Vector2.Distance(projectile.Center, targetHitbox.BottomLeft());
			float dist4 = Vector2.Distance(projectile.Center, targetHitbox.BottomRight());

			float minDist = dist1;
			if (dist2 < minDist)
				minDist = dist2;
			if (dist3 < minDist)
				minDist = dist3;
			if (dist4 < minDist)
				minDist = dist4;

			return minDist <= 35f && projectile.Opacity == 1f;
		}

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			target.AddBuff(BuffID.VortexDebuff, 60);
		}

		public override void Kill(int timeLeft)
        {
			for (int num621 = 0; num621 < 3; num621++)
			{
				int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (int)CalamityDusts.PurpleCosmolite, 0f, 0f, 100, default, 1.2f);
				Main.dust[num622].velocity *= 3f;
				Main.dust[num622].noGravity = true;
				if (Main.rand.NextBool(2))
				{
					Main.dust[num622].scale = 0.5f;
					Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
				}
			}
			for (int num623 = 0; num623 < 5; num623++)
			{
				int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (int)CalamityDusts.PurpleCosmolite, 0f, 0f, 100, default, 1.7f);
				Main.dust[num624].noGravity = true;
				Main.dust[num624].velocity *= 5f;
				num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (int)CalamityDusts.PurpleCosmolite, 0f, 0f, 100, default, 1f);
				Main.dust[num624].noGravity = true;
				Main.dust[num624].velocity *= 2f;
			}
		}
    }
}
