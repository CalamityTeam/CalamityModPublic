using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Permafrost
{
	public class FrostyFlare : ModProjectile
	{
		public override void SetDefaults()
		{
			projectile.width = 10;
			projectile.height = 10;
            projectile.coldDamage = true;
            projectile.friendly = true;
			projectile.penetrate = -1;
            projectile.timeLeft = 300;
			projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).rogue = true;
		}
		
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Frosty Flare");
        }

		public override void AI()
		{
            bool shoot = false;
            projectile.localAI[0]--;
            if (projectile.localAI[0] <= 0f)
            {
                projectile.localAI[0] = 30f;
                if (projectile.owner == Main.myPlayer)
                    shoot = true;
            }

            if (projectile.ai[0] == 0f)
            {
                projectile.velocity.X *= 0.99f;
                projectile.velocity.Y += 0.35f;
                projectile.rotation = projectile.velocity.ToRotation();

                if (shoot)
                {
                    Vector2 vel = new Vector2(Main.rand.Next(-300, 301), Main.rand.Next(500, 801));
                    Vector2 pos = projectile.Center - vel;
                    vel.X += Main.rand.Next(-50, 51);
                    vel.Normalize();
                    vel *= 30f;
                    int p = Projectile.NewProjectile(pos, vel + projectile.velocity / 4f, mod.ProjectileType("FrostShardFriendly"), projectile.damage, projectile.knockBack, projectile.owner);
                    Main.projectile[p].minion = false;
                }

                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 172);
                Main.dust[index2].noGravity = true;
            }
            else
            {
                projectile.ignoreWater = true;
                projectile.tileCollide = false;
                int id = (int)projectile.ai[1];
                if (id >= 0 && id < 200 && Main.npc[id].active && !Main.npc[id].dontTakeDamage)
                {
                    projectile.Center = Main.npc[id].Center - projectile.velocity * 2f;
                    projectile.gfxOffY = Main.npc[id].gfxOffY;

                    if (shoot)
                    {
                        Vector2 vel = new Vector2(Main.rand.Next(-300, 301), Main.rand.Next(500, 801));
                        Vector2 pos = Main.npc[id].Center - vel;
                        vel.X += Main.rand.Next(-50, 51);
                        vel.Normalize();
                        vel *= 30f;
                        int p = Projectile.NewProjectile(pos, vel + Main.npc[id].velocity, mod.ProjectileType("FrostShardFriendly"), projectile.damage, projectile.knockBack, projectile.owner);
                        Main.projectile[p].minion = false;
                    }
                }
                else
                {
                    projectile.Kill();
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 300);
            target.AddBuff(mod.BuffType("GlacialState"), 120);
            target.immune[projectile.owner] = 0;
            projectile.ai[0] = 1f;
            projectile.ai[1] = target.whoAmI;
            projectile.velocity = target.Center - projectile.Center;
            projectile.velocity *= 0.75f;
            projectile.netUpdate = true;

            const int maxFlares = 5;
            int flaresFound = 0;
            int oldestFlare = -1;
            int oldestFlareTimeLeft = 300;
            for (int i = 0; i < 1000; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == projectile.type && i != projectile.whoAmI && Main.projectile[i].ai[1] == target.whoAmI)
                {
                    flaresFound++;
                    if (Main.projectile[i].timeLeft < oldestFlareTimeLeft)
                    {
                        oldestFlareTimeLeft = Main.projectile[i].timeLeft;
                        oldestFlare = Main.projectile[i].whoAmI;
                    }
                    if (flaresFound >= maxFlares)
                        break;
                }
            }
            //Main.NewText("found " + flaresFound.ToString());
            if (flaresFound >= maxFlares && oldestFlare >= 0)
            {
                //Main.NewText("killing flare " + oldestFlare.ToString());
                Main.projectile[oldestFlare].Kill();
            }
        }

        public override bool CanDamage()
        {
            return projectile.ai[0] == 0f;
        }
	}
}