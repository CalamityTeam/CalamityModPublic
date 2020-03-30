using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Hybrid
{
    public class AccretionDiskProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Accretion Disk");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 56;
            projectile.height = 56;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 6;
            projectile.penetrate = -1;
            projectile.aiStyle = 3;
            projectile.timeLeft = 400;
            aiType = ProjectileID.WoodenBoomerang;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(2))
            {
                for (int num468 = 0; num468 < 1; num468++)
                {
                    int num250 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 66, (float)(projectile.direction * 2), 0f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.3f);
                    Main.dust[num250].noGravity = true;
                    Main.dust[num250].velocity *= 0f;
                }
            }
            Lighting.AddLight(projectile.Center, 0.15f, 1f, 0.25f);

			int[] array = new int[20];
			int num428 = 0;
			float distance = 300f;
			bool flag14 = false;
			for (int num430 = 0; num430 < 200; num430++)
			{
				if (Main.npc[num430].CanBeChasedBy(projectile, false))
				{
					float extraDistance = (float)(Main.npc[num430].width / 2) + (float)(Main.npc[num430].height / 2);

					bool useCollisionDetection = extraDistance < distance;
					bool canHit = true;
					if (useCollisionDetection)
						canHit = Collision.CanHit(projectile.Center, 1, 1, Main.npc[num430].Center, 1, 1);

					if (Vector2.Distance(Main.npc[num430].Center, projectile.Center) < (distance + extraDistance) && canHit)
					{
						if (num428 < 20)
						{
							array[num428] = num430;
							num428++;
						}
						flag14 = true;
					}
				}
			}

			if (flag14)
            {
                int num434 = Main.rand.Next(num428);
                num434 = array[num434];
                float num435 = Main.npc[num434].position.X + (float)(Main.npc[num434].width / 2);
                float num436 = Main.npc[num434].position.Y + (float)(Main.npc[num434].height / 2);
                projectile.localAI[0] += 1f;
                if (projectile.localAI[0] > 8f)
                {
                    projectile.localAI[0] = 0f;
                    float num437 = 6f;
                    Vector2 value10 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                    value10 += projectile.velocity * 4f;
                    float num438 = num435 - value10.X;
                    float num439 = num436 - value10.Y;
                    float num440 = (float)Math.Sqrt((double)(num438 * num438 + num439 * num439));
                    num440 = num437 / num440;
                    if (Main.rand.NextBool(3))
                    {
                        if (projectile.owner == Main.myPlayer)
                        {
                            float spread = 45f * 0.0174f;
                            double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
                            double deltaAngle = spread / 8f;
                            double offsetAngle;
                            int i;
                            for (i = 0; i < 4; i++)
                            {
                                offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                                int disk = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<AccretionDisk2>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
								if (projectile.melee)
									Main.projectile[disk].Calamity().forceMelee = true;
								else
									Main.projectile[disk].Calamity().forceRogue = true;
                                int disk2 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<AccretionDisk2>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
								if (projectile.melee)
									Main.projectile[disk2].Calamity().forceMelee = true;
								else
									Main.projectile[disk2].Calamity().forceRogue = true;
                            }
                        }
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 120);
            target.AddBuff(ModContent.BuffType<Plague>(), 120);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 120);
            target.AddBuff(ModContent.BuffType<Plague>(), 120);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 2);
            return false;
        }
    }
}
