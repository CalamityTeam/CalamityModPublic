using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
	public class BabyBloodCrawler : ModProjectile
    {
        public float dust = 0f;
		public int spiderCount = 0;
		public bool countedAlready = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Baby Blood Crawler");
            Main.projFrames[projectile.type] = 11;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.minionSlots = 1f;
            projectile.aiStyle = 26;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            aiType = ProjectileID.VenomSpider;
            projectile.tileCollide = false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = false;
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

			for (int j = 0; j < Main.maxProjectiles; j++)
			{
                Projectile proj = Main.projectile[j];
				// Short circuits to make the loop as fast as possible
				if (!proj.active || proj.owner != projectile.owner || !proj.minion || proj.Calamity().lineColor != 0)
					continue;
				if (proj.type == projectile.type)
				{
					spiderCount += (int)proj.minionSlots;
					proj.Calamity().lineColor = 1;
				}
			}

            if (dust == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int num226 = 16;
                for (int num227 = 0; num227 < num226; num227++)
                {
                    Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                    Vector2 vector7 = vector6 - projectile.Center;
                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 5, vector7.X * 1f, vector7.Y * 1f, 100, default, 1.1f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].noLight = true;
                    Main.dust[num228].velocity = vector7;
                }
                dust += 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }
            bool flag64 = projectile.type == ModContent.ProjectileType<BabyBloodCrawler>();
            player.AddBuff(ModContent.BuffType<ScabRipperBuff>(), 3600);
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.scabRipper = false;
                }
                if (modPlayer.scabRipper)
                {
                    projectile.timeLeft = 2;
                }
            }
        }

		public override bool OnTileCollide(Vector2 oldVelocity) => false;

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			//1 spider = 15 frames, 5 spiders, 10 frames
			int immuneTime = 16 - spiderCount;
			if (immuneTime < 5)
				immuneTime = 5; //cap to prevent potential insanity
            target.immune[projectile.owner] = immuneTime;

			OnHitEffects(target.Center);
		}

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
			OnHitEffects(target.Center);
        }

		private void OnHitEffects(Vector2 targetPos)
		{
			if (Main.rand.NextBool(2))
			{
				int projAmt = Main.rand.Next(1, 3);
				for (int n = 0; n < projAmt; n++)
				{
					CalamityUtils.ProjectileRain(targetPos, 400f, 100f, 500f, 800f, 29f, ModContent.ProjectileType<BloodRain>(), (int)(projectile.damage * Main.rand.NextFloat(0.7f, 1f)), projectile.knockBack * Main.rand.NextFloat(0.7f, 1f), projectile.owner);
				}
			}
        }
    }
}
