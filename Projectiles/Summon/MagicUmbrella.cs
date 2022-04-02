using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
	public class MagicUmbrella : ModProjectile
    {
		private int counter = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Umbrella");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 180;
            projectile.penetrate = 10;
            projectile.tileCollide = false;
            projectile.minion = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
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

			projectile.ChargingMinionAI(MagicHat.Range, 1600f, 2200f, 150f, 0, 40f, 9f, 4f, new Vector2(0f, -60f), 40f, 9f, true, true);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(75, 255, 255, projectile.alpha);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
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

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			if (Main.rand.NextBool(4))
			{
				SpawnBaseballBats(target.Center);
			}
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
			if (Main.rand.NextBool(4))
			{
				SpawnBaseballBats(target.Center);
			}
        }

		private void SpawnBaseballBats(Vector2 targetPos)
		{
            for (int n = 0; n < Main.rand.Next(1, 3); n++) //1 to 2 baseball bats
            {
                float x = targetPos.X + Main.rand.Next(-400, 401);
                float y = targetPos.Y - Main.rand.Next(500, 801);
                Vector2 source = new Vector2(x, y);
				Vector2 velocity = targetPos - source;
                velocity.X += Main.rand.Next(-100, 101);
                float speed = 29f;
                float targetDist = velocity.Length();
                targetDist = speed / targetDist;
                velocity.X *= targetDist;
                velocity.Y *= targetDist;
                Projectile.NewProjectile(source, velocity, ModContent.ProjectileType<MagicBat>(), (int)(projectile.damage * Main.rand.NextFloat(0.3f, 0.6f)), projectile.knockBack * Main.rand.NextFloat(0.7f, 1f), projectile.owner, 0f, 0f);
            }
		}
    }
}
