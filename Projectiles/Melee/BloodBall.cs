using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class BloodBall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Ball");
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 200;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            try
            {
                int num223 = (int)(Projectile.position.X / 16f) - 1;
                int num224 = (int)((Projectile.position.X + (float)Projectile.width) / 16f) + 2;
                int num225 = (int)(Projectile.position.Y / 16f) - 1;
                int num226 = (int)((Projectile.position.Y + (float)Projectile.height) / 16f) + 2;
                if (num223 < 0)
                {
                    num223 = 0;
                }
                if (num224 > Main.maxTilesX)
                {
                    num224 = Main.maxTilesX;
                }
                if (num225 < 0)
                {
                    num225 = 0;
                }
                if (num226 > Main.maxTilesY)
                {
                    num226 = Main.maxTilesY;
                }
                for (int num227 = num223; num227 < num224; num227++)
                {
                    for (int num228 = num225; num228 < num226; num228++)
                    {
                        if (Main.tile[num227, num228] != null && Main.tile[num227, num228].HasUnactuatedTile && (Main.tileSolid[(int)Main.tile[num227, num228].TileType] || (Main.tileSolidTop[(int)Main.tile[num227, num228].TileType] && Main.tile[num227, num228].TileFrameY == 0)))
                        {
                            Vector2 vector19;
                            vector19.X = (float)(num227 * 16);
                            vector19.Y = (float)(num228 * 16);
                            if (Projectile.position.X + (float)Projectile.width - 4f > vector19.X && Projectile.position.X + 4f < vector19.X + 16f && Projectile.position.Y + (float)Projectile.height - 4f > vector19.Y && Projectile.position.Y + 4f < vector19.Y + 16f)
                            {
                                Projectile.velocity.X = 0f;
                                Projectile.velocity.Y = -0.2f;
                            }
                        }
                    }
                }
            } catch
            {
            }
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
            {
                Projectile.tileCollide = false;
                Projectile.ai[1] = 0f;
                Projectile.alpha = 255;
                Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
                Projectile.width = 128;
                Projectile.height = 128;
                Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
                Projectile.knockBack = 8f;
            }
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 10f)
            {
                Projectile.ai[0] = 10f;
                if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X * 0.97f;
                    if ((double)Projectile.velocity.X > -0.01 && (double)Projectile.velocity.X < 0.01)
                    {
                        Projectile.velocity.X = 0f;
                        Projectile.netUpdate = true;
                    }
                }
                Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
            }
            Projectile.rotation += Projectile.velocity.X * 0.1f;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit, (int)Projectile.position.X, (int)Projectile.position.Y, 20);
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            for (int num648 = 0; num648 < 20; num648++)
            {
                int num649 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 5, 0f, 0f, 100, default, 1.5f);
                Main.dust[num649].velocity *= 1.4f;
            }
            for (int num650 = 0; num650 < 10; num650++)
            {
                int num651 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 5, 0f, 0f, 100, default, 2.5f);
                Main.dust[num651].noGravity = true;
                Main.dust[num651].velocity *= 5f;
                num651 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 5, 0f, 0f, 100, default, 1.5f);
                Main.dust[num651].velocity *= 3f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.Kill();
        }
    }
}
