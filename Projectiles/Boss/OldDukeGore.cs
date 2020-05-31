using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
	public class OldDukeGore : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphurous Gore");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

		public override void SetDefaults()
		{
			projectile.width = 24;
			projectile.height = 24;
			projectile.hostile = true;
			projectile.ignoreWater = true;
			projectile.penetrate = 1;
			projectile.timeLeft = 300;
			projectile.alpha = 255;
			cooldownSlot = 1;
		}

        public override void AI()
        {
			projectile.alpha -= 50;
			if (projectile.alpha < 0)
				projectile.alpha = 0;

			projectile.ai[0] += 1f;
			if (projectile.ai[0] >= 15f)
			{
				projectile.velocity.Y += 0.1f;
			}

			if (projectile.velocity.Y > 16f)
			{
				projectile.velocity.Y = 16f;
			}

			projectile.tileCollide = projectile.timeLeft < 240;

			projectile.rotation += projectile.velocity.X * 0.1f;

			int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 5, 0f, 0f, 100, default, 1f);
            Main.dust[num469].noGravity = true;
            Main.dust[num469].velocity *= 0f;

			int num470 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default, 1f);
			Main.dust[num470].noGravity = true;
			Main.dust[num470].velocity *= 0f;
		}

        public override void Kill(int timeLeft)
		{
			Main.PlaySound(SoundID.NPCDeath12, projectile.position);

			int num226 = 8;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                Vector2 vector7 = vector6 - projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 5, vector7.X, vector7.Y, 100, default, 1.2f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].noLight = true;
                Main.dust[num228].velocity = vector7;
            }

			for (int num623 = 0; num623 < 6; num623++)
			{
				int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default(Color), 3f);
				Main.dust[num624].noGravity = true;
				Main.dust[num624].velocity *= 5f;
				num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default(Color), 2f);
				Main.dust[num624].velocity *= 2f;
				Main.dust[num624].noGravity = true;
			}
		}

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
			target.AddBuff(BuffID.Poisoned, 180, true);
			target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
		}

		public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
		{
			target.Calamity().lastProjectileHit = projectile;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }
    }
}
