using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
	public class StellarKnifeProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/StellarKnife";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stellar Knife");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 34;
            projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.tileCollide = true;
            projectile.Calamity().rogue = true;
        }

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(projectile.localAI[1]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			projectile.localAI[1] = reader.ReadSingle();
		}

        public override void AI()
        {
			//synthesized timeLeft
			projectile.localAI[1]++;
			if (projectile.localAI[1] > 600f)
				projectile.Kill();

            if (projectile.ai[0] == 1f)
            {
                projectile.ai[0] = 0f;
                projectile.damage = (int)(projectile.damage * (projectile.ai[1] == 1f ? 0.9f : 0.75f));
                projectile.ai[1] = 0f;
            }
            projectile.ai[1] += 0.75f;
            if (projectile.ai[1] <= 60f)
            {
                projectile.rotation -= 1f;
                projectile.velocity.X *= 0.985f;
                projectile.velocity.Y *= 0.985f;
            }
            else
            {
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);

				Vector2 center = projectile.Center;
				float maxDistance = 600f;
				bool homeIn = false;

				for (int i = 0; i < Main.maxNPCs; i++)
				{
					if (Main.npc[i].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
					{
						float extraDistance = (float)(Main.npc[i].width / 2) + (float)(Main.npc[i].height / 2);

						if (Vector2.Distance(Main.npc[i].Center, projectile.Center) < (maxDistance + extraDistance))
						{
							center = Main.npc[i].Center;
							homeIn = true;
							break;
						}
					}
				}

				if (homeIn)
				{
					projectile.timeLeft = 600; //when homing in, the projectile cannot run out of timeLeft, but synthesized timeLeft still runs

                    Vector2 moveDirection = projectile.SafeDirectionTo(center, Vector2.UnitY);
                    projectile.velocity = (projectile.velocity * 10f + moveDirection * 30f) / (10f + 1f);
				}
                else
                {
					//shorten knife lifespan if it hasn't found a target
					if (projectile.timeLeft > 60)
						projectile.timeLeft = 60;
                    projectile.velocity.X *= 0.92f;
                    projectile.velocity.Y *= 0.92f;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, ModContent.DustType<AstralBlue>(), projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
