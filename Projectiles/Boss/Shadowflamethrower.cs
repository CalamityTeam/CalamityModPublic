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
            projectile.width = 6;
            projectile.height = 6;
            projectile.hostile = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.extraUpdates = 3;
            projectile.timeLeft = 60;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void AI()
        {
            if (projectile.ai[0] > 7f)
            {
                int[] dustTypes = new int[] { 27, 27, 112, 173 };
                if(true)
                {
                    int dustType = dustTypes[Main.rand.Next(4)];
                    int dustID;
                    Vector2 corner = projectile.position - new Vector2(6f, 6f);
                    int width = 18;
                    int height = 18;
                    switch (dustType)
                    {
                        case 27:
                            dustID = Dust.NewDust(corner, width, height, dustType);
                            Main.dust[dustID].noGravity = false;
                            Main.dust[dustID].scale = Main.rand.NextFloat(1f, 1.2f);
                            Main.dust[dustID].velocity *= 0.9f;
                            Main.dust[dustID].velocity += projectile.velocity * 0.7f;
                            break;
                        case 112:
                            dustID = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType);
                            Main.dust[dustID].noGravity = true;
                            Main.dust[dustID].scale = Main.rand.NextFloat(1.4f, 2f);
                            Main.dust[dustID].velocity *= 2.2f;
                            Main.dust[dustID].velocity += projectile.velocity * Main.rand.NextFloat(0.15f, 0.4f);
                            break;
                        case 173:
                            dustID = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType);
                            Main.dust[dustID].noGravity = false;
                            Main.dust[dustID].scale = Main.rand.NextFloat(1.9f, 3f);
                            Main.dust[dustID].velocity *= 1.5f;
                            Main.dust[dustID].velocity += projectile.velocity * Main.rand.NextFloat(0.6f, 1f);
                            break;
                    }
                }
            }

            projectile.ai[0]++;
            projectile.rotation += 0.3f * (float)projectile.direction;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.rand.NextBool(6))
                target.AddBuff(ModContent.BuffType<Shadowflame>(), 240, true);
            else if (Main.rand.NextBool(4))
                target.AddBuff(ModContent.BuffType<Shadowflame>(), 150, true);
            else if (Main.rand.NextBool(2))
                target.AddBuff(ModContent.BuffType<Shadowflame>(), 90, true);
        }
    }
}
