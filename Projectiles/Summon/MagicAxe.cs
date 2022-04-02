using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
	public class MagicAxe : ModProjectile
    {
		private int counter = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Axe");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 52;
            projectile.height = 52;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 0;
            projectile.timeLeft = 180;
            projectile.penetrate = -1;
            projectile.minion = true;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 8;
			projectile.alpha = 255;
        }

        public override void AI()
        {
            projectile.rotation += 0.075f;
			projectile.alpha -= 50;
			counter++;
			if (counter == 30)
			{
				projectile.netUpdate = true;
			}
			else if (counter < 30)
			{
				return;
			}

			projectile.ChargingMinionAI(MagicHat.Range, 1200f, 2500f, 400f, 1, 30f, 24f, 12f, new Vector2(0f, -60f), 30f, 16f, true, true, 3);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(0, 255, 111, projectile.alpha);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 90);
			target.AddBuff(BuffID.Frostburn, 90);
			target.AddBuff(ModContent.BuffType<HolyFlames>(), 90);
		}

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
			target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 90);
			target.AddBuff(BuffID.Frostburn, 90);
			target.AddBuff(ModContent.BuffType<HolyFlames>(), 90);
		}

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 dspeed = new Vector2(Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-7f, 7f));
                int dust = Dust.NewDust(projectile.Center, 1, 1, 66, dspeed.X, dspeed.Y, 160, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 0.75f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
