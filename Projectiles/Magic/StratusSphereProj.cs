using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Projectiles.Magic
{
    public class StratusSphereProj : ModProjectile
    {
        int roundsGone = 0;
        int dust_nut = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stratus Sphere");
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.width = 314;
            Projectile.height = 198;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.knockBack = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(230, 230, 255, Projectile.alpha);
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            if (Projectile.ai[0] < 200)
            {
                if (Projectile.ai[0] > 100)
                {
                    Projectile.velocity.X = Projectile.velocity.X * 40 / 41;
                    Projectile.velocity.Y = Projectile.velocity.Y * 45 / 46 - 0.005f;
                }
            }
            else
            {
                Projectile.velocity.X = Projectile.velocity.X * 10 / 11;
                Projectile.velocity.Y = Projectile.velocity.Y * 10 / 11;
                if (roundsGone <= 4)
                    Projectile.ai[1]++;

                int rand = Main.rand.Next(-50, 51);
                int rand2 = Main.rand.Next(-50, 51);
                Vector2 targetDir = Projectile.Center + new Vector2(rand, rand2);
                if (Projectile.ai[1] > 40)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDirectionTo(targetDir) * 12f, ModContent.ProjectileType<Crescent>(), Projectile.damage / 2, 0.4f, Projectile.owner, Projectile.whoAmI);

                if (Projectile.ai[1] > 46)
                {
                    Projectile.ai[1] = 0;
                    roundsGone++;
                }
                if (roundsGone > 4 && Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<Crescent>()] == 0)
                    Projectile.Kill();
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 6)
                {
                    Projectile.frame = 0;
                }
            }

            if (Main.rand.NextFloat() < 1f)
            {
                Dust dust;
                // You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
                Vector2 position = Projectile.Center;
                dust = Main.dust[Terraria.Dust.NewDust(position, 0, 0, 226, 0f, 0f, 0, new Color(255, 255, 255), 0.7236842f)];
            }

            dust_nut++;

            if (dust_nut > 22)
            {
                int num20 = 36;
                for (int i = 0; i < num20; i++)
                {
                    Vector2 spinningpoint = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f * 0.5f;
                    spinningpoint = spinningpoint.RotatedBy((double)((float)(i - (num20 / 2 - 1)) * 6.28318548f / (float)num20), default(Vector2)) + Projectile.Center;
                    Vector2 vector = spinningpoint - Projectile.Center;
                    int num21 = Dust.NewDust(spinningpoint + vector, 0, 0, 226, vector.X * 2f, vector.Y * 2f, 0, new Color(255, 255, 255), 0.7236842f);
                    Main.dust[num21].noGravity = true;
                    Main.dust[num21].noLight = true;
                    Main.dust[num21].velocity = Vector2.Normalize(vector) * 3f;
                }
                dust_nut = 0;
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 25; i++)
            {
                Dust dust;
                // You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
                Vector2 position = Projectile.Center - new Vector2(34, 34);
                dust = Main.dust[Terraria.Dust.NewDust(position, 68, 68, 226, 0f, 0f, 0, new Color(255, 255, 255), 0.7236842f)];
            }

            int num20 = 36;
            for (int i = 0; i < num20; i++)
            {
                Vector2 spinningpoint = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f * 0.5f;
                spinningpoint = spinningpoint.RotatedBy((double)((float)(i - (num20 / 2 - 1)) * 6.28318548f / (float)num20), default(Vector2)) + Projectile.Center;
                Vector2 vector = spinningpoint - Projectile.Center;
                int num21 = Dust.NewDust(spinningpoint + vector, 0, 0, 226, vector.X * 2f, vector.Y * 2f, 0, new Color(255, 255, 255), 0.7236842f);
                Main.dust[num21].noGravity = true;
                Main.dust[num21].noLight = true;
                Main.dust[num21].velocity = Vector2.Normalize(vector) * 3f;
            }
        }
    }
}
