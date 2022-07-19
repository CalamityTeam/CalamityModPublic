using CalamityMod.Buffs.DamageOverTime;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class ShadeNimbusHostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shade Nimbus");
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 28;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.timeLeft = 360;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 5)
                {
                    Projectile.frame = 0;
                }
            }
            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] >= 300f)
            {
                Projectile.alpha += 5;
                if (Projectile.alpha > 255)
                {
                    Projectile.alpha = 255;
                    Projectile.Kill();
                }
            }
            else
            {
                Projectile.ai[0] += 1f;
                if (Projectile.ai[0] >= (Main.getGoodWorld ? 10f : 36f))
                {
                    Projectile.ai[0] = 0f;
                    int num414 = (int)(Projectile.position.X + 14f + (float)Main.rand.Next(Projectile.width - 28));
                    int num415 = (int)(Projectile.position.Y + (float)Projectile.height + 4f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), (float)num414, (float)num415, 0f, 4f, ModContent.ProjectileType<ShaderainHostile>(), Projectile.damage, 0f, Main.myPlayer, 0f, 0f);
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Shadowflame>(), 120);
        }
    }
}
