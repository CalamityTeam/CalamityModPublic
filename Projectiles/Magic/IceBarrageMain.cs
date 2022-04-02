using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Buffs.StatDebuffs;

namespace CalamityMod.Projectiles.Magic
{
    public class IceBarrageMain : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int pwidth = 58;
        private int pheight = 58;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Barrage");
        }

        public override void SetDefaults()
        {
            projectile.width = pwidth;
            projectile.height = pheight;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.magic = true;
            projectile.timeLeft = 280;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.coldDamage = true;
        }

        public override void AI()
        {
            NPC closestTarget = projectile.Center.ClosestNPCAt(500f, true, true);
            if (closestTarget != null)
            {
                projectile.Center = closestTarget.Center;
            }

            projectile.ai[0]++;
            projectile.ai[1]++;
            for (int j = 0; j < 3; j++)
            {
                int dustType = Main.rand.NextBool(2) ? 68 : 67;
                if (Main.rand.NextBool(4))
                {
                    dustType = 80;
                }
                if (projectile.ai[0] < 140f)
                {
                    Vector2 direction = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f));
                    int dust = Dust.NewDust(projectile.Center, 1, 1, dustType, direction.X, direction.Y, 50, default, 1.3f);
                    Main.dust[dust].noGravity = true;
                }
                else
                {
                    int direct = Main.rand.NextBool(2) ? 1 : -1;
                    Vector2 dir1 = new Vector2(0f, 10f) * direct;
                    int dust1 = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, dir1.X, dir1.Y, 50, default, 1.3f);
                    Main.dust[dust1].noGravity = true;
                    Main.dust[dust1].velocity = dir1;
                    int direct2 = Main.rand.NextBool(2) ? 1 : -1;
                    Vector2 dir2 = new Vector2(10f, 0f) * direct2;
                    int dust2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, dir2.X, dir2.Y, 50, default, 1.3f);
                    Main.dust[dust2].noGravity = true;
                    Main.dust[dust2].velocity = dir2;
                }
            }
            if (projectile.ai[0] < 55)
            {
                for (int i = 0; i < 10; i++)
                {
                    int dtype1 = Main.rand.NextBool(2) ? 68 : 67;
                    if (Main.rand.NextBool(4))
                    {
                        dtype1 = 80;
                    }
                    Vector2 Dpos = projectile.Center + new Vector2(Main.rand.NextFloat(250f, 270f), Main.rand.NextFloat(250f, 270f)).RotatedBy(MathHelper.ToRadians(Main.rand.Next(1, 360)));
                    Vector2 Dspeed = projectile.Center - Dpos;
                    Dspeed.Normalize();
                    Dspeed *= 0.5f;
                    float Dscale = Main.rand.NextFloat(1.5f, 2f);
                    int d1 = Dust.NewDust(Dpos, 1, 1, dtype1, Dspeed.X, Dspeed.Y, 0, default, Dscale);
                    Main.dust[d1].velocity = Dspeed;
                    Main.dust[d1].noGravity = true;
                }
            }
            else if (projectile.ai[0] == 55f)
            {
                for (int i = 0; i < 270; i++)
                {
                    int dtype2 = Main.rand.NextBool(2) ? 68 : 67;
                    if (Main.rand.NextBool(4))
                    {
                        dtype2 = 80;
                    }
                    Vector2 Dpos = projectile.Center + new Vector2(Main.rand.NextFloat(250f, 270f), Main.rand.NextFloat(250f, 270f)).RotatedBy(MathHelper.ToRadians(Main.rand.Next(1, 360)));
                    Vector2 Dspeed = projectile.Center - Dpos;
                    Dspeed.Normalize();
                    Dspeed *= Main.rand.NextFloat(8f,34f);
                    float Dscale = Main.rand.NextFloat(1.5f, 2f);
                    int d1 = Dust.NewDust(Dpos, 1, 1, dtype2, Dspeed.X, Dspeed.Y, 0, default, Dscale);
                    Main.dust[d1].velocity = Dspeed;
                    Main.dust[d1].noGravity = true;
                }
                
            }
            else if (projectile.ai[0] == 140f)
            {
                Vector2 projcenter = projectile.Center;
                projectile.width = 200;
                projectile.height = 200;
                projectile.Center = projcenter;
                projectile.Damage();
                for (int i = 0; i < 180; i++)
                {
                    int dtype3 = Main.rand.NextBool(2) ? 68 : 67;
                    if (Main.rand.NextBool(4))
                    {
                        dtype3 = 80;
                    }
                    Vector2 explosiondir = new Vector2(Main.rand.NextFloat(-18f, 18f), Main.rand.NextFloat(-18f, 18f));
                    int d2 = Dust.NewDust(projectile.Center, 1, 1, dtype3, explosiondir.X, explosiondir.Y, 50, default, 1.5f);
                    Main.dust[d2].noGravity = true;
                }
                for (int k = 0; k < 45; k++)
                {
                    Vector2 projspeed = new Vector2(Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-8f, 8f));
                    int ice = Projectile.NewProjectile(projectile.Center, projspeed, ProjectileID.NorthPoleSnowflake, (int)(projectile.damage * 0.05f), 2f, projectile.owner, 0f, (float)Main.rand.Next(3));
                    Main.projectile[ice].timeLeft = 600;
                    Main.projectile[ice].melee = false;
                    Main.projectile[ice].Calamity().forceMagic = true;
                }
                projectile.width = pwidth;
                projectile.height = pheight;
                projectile.Center = projcenter;
                Vector2 pos1 = new Vector2(projectile.Center.X, projectile.Center.Y - (projectile.height * 0.5f) - 44f);
                int block1 = Projectile.NewProjectile(pos1, Vector2.Zero, ModContent.ProjectileType<IceBlock>(), (int)(projectile.damage * 0.3f), 5f, projectile.owner, 0f, 0f);
                Main.projectile[block1].Center = pos1;
                Vector2 pos2 = new Vector2(projectile.Center.X + (projectile.width * 0.5f) + 48f, projectile.Center.Y);
                int block2 = Projectile.NewProjectile(pos2, Vector2.Zero, ModContent.ProjectileType<IceBlock>(), (int)(projectile.damage * 0.3f), 5f, projectile.owner, 1f, 0f);
                Main.projectile[block2].Center = pos2;
                Vector2 pos3 = new Vector2(projectile.Center.X, projectile.Center.Y + (projectile.height * 0.5f) + 44f);
                int block3 = Projectile.NewProjectile(pos3, Vector2.Zero, ModContent.ProjectileType<IceBlock>(), (int)(projectile.damage * 0.3f), 5f, projectile.owner, 2f, 0f);
                Main.projectile[block3].Center = pos3;
                Vector2 pos4 = new Vector2(projectile.Center.X - (projectile.width * 0.5f) - 49f, projectile.Center.Y);
                int block4 = Projectile.NewProjectile(pos4, Vector2.Zero, ModContent.ProjectileType<IceBlock>(), (int)(projectile.damage * 0.3f), 5f, projectile.owner, 3f, 0f);
                Main.projectile[block4].Center = pos4;
            }
            if (projectile.ai[0] > 90)
            {
                if (projectile.ai[1] >= 5f)
                {
                    Vector2 projspeed = new Vector2(Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f));
                    int ice = Projectile.NewProjectile(projectile.Center, projspeed, ProjectileID.NorthPoleSnowflake, (int)(projectile.damage * 0.05f), 2f, projectile.owner, 0f, (float)Main.rand.Next(3));
                    if (ice.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[ice].timeLeft = 600;
                        Main.projectile[ice].melee = false;
                        Main.projectile[ice].Calamity().forceMagic = true;
                    }
                    projectile.ai[1] = 0f;
                }
            }
        }

        public override bool CanDamage()
        {
            if (projectile.ai[0] == 140f)
                return true;
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<ExoFreeze>(), 30);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 60);
        }
    }
}
