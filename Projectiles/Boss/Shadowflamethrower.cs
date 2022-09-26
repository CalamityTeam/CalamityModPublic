using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class Shadowflamethrower : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowflame Breath");
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 60;
        }

        public override void AI()
        {
            if (Projectile.ai[0] > 7f)
            {
                int[] dustTypes = new int[] { 27, 27, 112, 173 };
                if(true)
                {
                    int dustType = dustTypes[Main.rand.Next(4)];
                    int dustID;
                    Vector2 corner = Projectile.position - new Vector2(6f, 6f);
                    int width = 18;
                    int height = 18;
                    switch (dustType)
                    {
                        case 27:
                            dustID = Dust.NewDust(corner, width, height, dustType);
                            Main.dust[dustID].noGravity = false;
                            Main.dust[dustID].scale = Main.rand.NextFloat(1f, 1.2f);
                            Main.dust[dustID].velocity *= 0.9f;
                            Main.dust[dustID].velocity += Projectile.velocity * 0.7f;
                            break;
                        case 112:
                            dustID = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType);
                            Main.dust[dustID].noGravity = true;
                            Main.dust[dustID].scale = Main.rand.NextFloat(1.4f, 2f);
                            Main.dust[dustID].velocity *= 2.2f;
                            Main.dust[dustID].velocity += Projectile.velocity * Main.rand.NextFloat(0.15f, 0.4f);
                            break;
                        case 173:
                            dustID = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType);
                            Main.dust[dustID].noGravity = false;
                            Main.dust[dustID].scale = Main.rand.NextFloat(1.9f, 3f);
                            Main.dust[dustID].velocity *= 1.5f;
                            Main.dust[dustID].velocity += Projectile.velocity * Main.rand.NextFloat(0.6f, 1f);
                            break;
                    }
                }
            }

            Projectile.ai[0]++;
            Projectile.rotation += 0.3f * (float)Projectile.direction;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<Shadowflame>(), 300, true);
        }
    }
}
