using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Buffs.StatDebuffs;

namespace CalamityMod.Projectiles.Magic
{
    public class IceBarrageMain : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int pwidth = 58;
        private int pheight = 58;
        public override void SetDefaults()
        {
            Projectile.width = pwidth;
            Projectile.height = pheight;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 280;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.coldDamage = true;
        }

        public override void AI()
        {
            NPC closestTarget = Projectile.Center.ClosestNPCAt(500f, true, true);
            if (closestTarget != null)
            {
                Projectile.Center = closestTarget.Center;
            }

            Projectile.ai[0]++;
            Projectile.ai[1]++;
            for (int j = 0; j < 3; j++)
            {
                int dustType = Main.rand.NextBool() ? 68 : 67;
                if (Main.rand.NextBool(4))
                {
                    dustType = 80;
                }
                if (Projectile.ai[0] < 140f)
                {
                    Vector2 direction = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f));
                    int dust = Dust.NewDust(Projectile.Center, 1, 1, dustType, direction.X, direction.Y, 50, default, 1.3f);
                    Main.dust[dust].noGravity = true;
                }
                else
                {
                    int direct = Main.rand.NextBool() ? 1 : -1;
                    Vector2 dir1 = new Vector2(0f, 10f) * direct;
                    int dust1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, dir1.X, dir1.Y, 50, default, 1.3f);
                    Main.dust[dust1].noGravity = true;
                    Main.dust[dust1].velocity = dir1;
                    int direct2 = Main.rand.NextBool() ? 1 : -1;
                    Vector2 dir2 = new Vector2(10f, 0f) * direct2;
                    int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, dir2.X, dir2.Y, 50, default, 1.3f);
                    Main.dust[dust2].noGravity = true;
                    Main.dust[dust2].velocity = dir2;
                }
            }
            if (Projectile.ai[0] < 55)
            {
                for (int i = 0; i < 10; i++)
                {
                    int dtype1 = Main.rand.NextBool() ? 68 : 67;
                    if (Main.rand.NextBool(4))
                    {
                        dtype1 = 80;
                    }
                    Vector2 Dpos = Projectile.Center + new Vector2(Main.rand.NextFloat(250f, 270f), Main.rand.NextFloat(250f, 270f)).RotatedBy(MathHelper.ToRadians(Main.rand.Next(1, 360)));
                    Vector2 Dspeed = Projectile.Center - Dpos;
                    Dspeed.Normalize();
                    Dspeed *= 0.5f;
                    float Dscale = Main.rand.NextFloat(1.5f, 2f);
                    int d1 = Dust.NewDust(Dpos, 1, 1, dtype1, Dspeed.X, Dspeed.Y, 0, default, Dscale);
                    Main.dust[d1].velocity = Dspeed;
                    Main.dust[d1].noGravity = true;
                }
            }
            else if (Projectile.ai[0] == 55f)
            {
                for (int i = 0; i < 270; i++)
                {
                    int dtype2 = Main.rand.NextBool() ? 68 : 67;
                    if (Main.rand.NextBool(4))
                    {
                        dtype2 = 80;
                    }
                    Vector2 Dpos = Projectile.Center + new Vector2(Main.rand.NextFloat(250f, 270f), Main.rand.NextFloat(250f, 270f)).RotatedBy(MathHelper.ToRadians(Main.rand.Next(1, 360)));
                    Vector2 Dspeed = Projectile.Center - Dpos;
                    Dspeed.Normalize();
                    Dspeed *= Main.rand.NextFloat(8f,34f);
                    float Dscale = Main.rand.NextFloat(1.5f, 2f);
                    int d1 = Dust.NewDust(Dpos, 1, 1, dtype2, Dspeed.X, Dspeed.Y, 0, default, Dscale);
                    Main.dust[d1].velocity = Dspeed;
                    Main.dust[d1].noGravity = true;
                }

            }
            else if (Projectile.ai[0] == 140f)
            {
                Vector2 projcenter = Projectile.Center;
                Projectile.width = 200;
                Projectile.height = 200;
                Projectile.Center = projcenter;
                Projectile.Damage();
                for (int i = 0; i < 180; i++)
                {
                    int dtype3 = Main.rand.NextBool() ? 68 : 67;
                    if (Main.rand.NextBool(4))
                    {
                        dtype3 = 80;
                    }
                    Vector2 explosiondir = new Vector2(Main.rand.NextFloat(-18f, 18f), Main.rand.NextFloat(-18f, 18f));
                    int d2 = Dust.NewDust(Projectile.Center, 1, 1, dtype3, explosiondir.X, explosiondir.Y, 50, default, 1.5f);
                    Main.dust[d2].noGravity = true;
                }
                for (int k = 0; k < 45; k++)
                {
                    Vector2 projspeed = new Vector2(Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-8f, 8f));
                    int ice = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, projspeed, ProjectileID.NorthPoleSnowflake, (int)(Projectile.damage * 0.05f), 2f, Projectile.owner, 0f, (float)Main.rand.Next(3));
                    Main.projectile[ice].timeLeft = 600;
                    Main.projectile[ice].DamageType = DamageClass.Magic;
                }
                Projectile.width = pwidth;
                Projectile.height = pheight;
                Projectile.Center = projcenter;
                Vector2 pos1 = new Vector2(Projectile.Center.X, Projectile.Center.Y - (Projectile.height * 0.5f) - 44f);
                int block1 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos1, Vector2.Zero, ModContent.ProjectileType<IceBlock>(), (int)(Projectile.damage * 0.3f), 5f, Projectile.owner, 0f, 0f);
                Main.projectile[block1].Center = pos1;
                Vector2 pos2 = new Vector2(Projectile.Center.X + (Projectile.width * 0.5f) + 48f, Projectile.Center.Y);
                int block2 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos2, Vector2.Zero, ModContent.ProjectileType<IceBlock>(), (int)(Projectile.damage * 0.3f), 5f, Projectile.owner, 1f, 0f);
                Main.projectile[block2].Center = pos2;
                Vector2 pos3 = new Vector2(Projectile.Center.X, Projectile.Center.Y + (Projectile.height * 0.5f) + 44f);
                int block3 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos3, Vector2.Zero, ModContent.ProjectileType<IceBlock>(), (int)(Projectile.damage * 0.3f), 5f, Projectile.owner, 2f, 0f);
                Main.projectile[block3].Center = pos3;
                Vector2 pos4 = new Vector2(Projectile.Center.X - (Projectile.width * 0.5f) - 49f, Projectile.Center.Y);
                int block4 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos4, Vector2.Zero, ModContent.ProjectileType<IceBlock>(), (int)(Projectile.damage * 0.3f), 5f, Projectile.owner, 3f, 0f);
                Main.projectile[block4].Center = pos4;
            }
            if (Projectile.ai[0] > 90)
            {
                if (Projectile.ai[1] >= 5f)
                {
                    Vector2 projspeed = new Vector2(Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f));
                    int ice = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, projspeed, ProjectileID.NorthPoleSnowflake, (int)(Projectile.damage * 0.05f), 2f, Projectile.owner, 0f, (float)Main.rand.Next(3));
                    if (ice.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[ice].timeLeft = 600;
                        Main.projectile[ice].DamageType = DamageClass.Magic;
                    }
                    Projectile.ai[1] = 0f;
                }
            }
        }

        public override bool? CanDamage()
        {
            if (Projectile.ai[0] == 140f)
                return null;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GlacialState>(), 60);
        }
    }
}
