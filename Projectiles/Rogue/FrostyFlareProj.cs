using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class FrostyFlareProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/FrostyFlare";

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.coldDamage = true;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 300;
            projectile.Calamity().rogue = true;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frosty Flare");
        }

        public override void AI()
        {
            if (projectile.owner != Main.myPlayer)
                return;

            bool shoot = false;
            if (projectile.timeLeft % 30f == 0f)
            {
                if (projectile.owner == Main.myPlayer)
                    shoot = true;
            }

            if (projectile.ai[0] == 0f)
            {
                projectile.velocity.X *= 0.99f;
                projectile.velocity.Y += 0.25f;
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

                if (shoot)
                {
                    Vector2 vel = new Vector2(Main.rand.Next(-300, 301), Main.rand.Next(500, 801));
                    Vector2 pos = projectile.Center - vel;
                    vel.X += Main.rand.Next(-50, 51);
                    vel.Normalize();
                    vel *= 30f;
                    int shard = Projectile.NewProjectile(pos, vel + projectile.velocity / 4f, ModContent.ProjectileType<FrostShardFriendly>(), projectile.damage, projectile.knockBack, projectile.owner);
                    Main.projectile[shard].alpha = projectile.alpha;
                }

                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 172);
                Main.dust[index2].noGravity = true;
            }
            else
            {
                projectile.ignoreWater = true;
                projectile.tileCollide = false;
                int id = (int)projectile.ai[1];
                if (id >= 0 && id < Main.maxNPCs && Main.npc[id].active && !Main.npc[id].dontTakeDamage)
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
                        int shard = Projectile.NewProjectile(pos, vel + Main.npc[id].velocity, ModContent.ProjectileType<FrostShardFriendly>(), projectile.damage, projectile.knockBack, projectile.owner);
                        Main.projectile[shard].alpha = projectile.alpha;
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
            target.AddBuff(BuffID.Frostburn, 180);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
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
            for (int i = 0; i < Main.maxProjectiles; i++)
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
            if (flaresFound >= maxFlares && oldestFlare >= 0)
            {
                Main.projectile[oldestFlare].Kill();
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 180);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
        }

        public override bool CanDamage() => projectile.ai[0] == 0f;
    }
}
