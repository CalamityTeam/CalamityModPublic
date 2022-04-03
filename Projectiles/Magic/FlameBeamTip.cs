using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class FlameBeamTip : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = 4;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override bool PreAI()
        {
            if (Projectile.ai[0] != 0f)
                if (Projectile.alpha < 170 && Projectile.alpha + 5 >= 170)
                    Projectile.alpha += 5;

            return true;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0f)
            {
                Projectile.alpha -= 50;
                if (Projectile.alpha <= 0)
                {
                    Projectile.alpha = 0;
                    Projectile.ai[0] = 1f;
                    if (Projectile.ai[1] == 0f)
                    {
                        Projectile.ai[1] += 1f;
                        Projectile.position += Projectile.velocity;
                    }
                    if (Main.myPlayer == Projectile.owner)
                    {
                        int num48 = Projectile.type;
                        if (Projectile.ai[1] >= (float)(15 + Main.rand.Next(3)))
                        {
                            num48 = ModContent.ProjectileType<FlameBeamTip2>();
                        }

                        int number = Projectile.NewProjectile(Projectile.position.X + Projectile.velocity.X + (float)(Projectile.width / 2), Projectile.position.Y + Projectile.velocity.Y + (float)(Projectile.height / 2),
                            Projectile.velocity.X, Projectile.velocity.Y, num48, Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, Projectile.ai[1] + 1f);
                        NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, number, 0f, 0f, 0f, 0, 0, 0);
                    }
                }
            }
            else
            {
                if (Projectile.alpha == 150)
                {
                    for (int num55 = 0; num55 < 10; num55++)
                    {
                        int num56 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 60, Projectile.velocity.X * 0.01f, Projectile.velocity.Y * 0.01f, 200, default, 2f);
                        Main.dust[num56].noGravity = true;
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 8;
            target.AddBuff(BuffID.OnFire, 240);
        }
    }
}
