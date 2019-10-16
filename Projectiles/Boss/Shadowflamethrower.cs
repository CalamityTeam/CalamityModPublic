using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class Shadowflamethrower : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowflamethrower");
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
        }

        public override void AI()
        {
            if (projectile.ai[0] > 7f)
            {
                float num302 = 1f;
                if (projectile.ai[0] == 8f)
                    num302 = 0.25f;
                else if (projectile.ai[0] == 9f)
                    num302 = 0.5f;
                else if (projectile.ai[0] == 10f)
                    num302 = 0.75f;

                projectile.ai[0] += 1f;
                if (Main.rand.NextBool(2))
                {
                    int num305 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 27, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default, 1f);
                    Dust dust;

                    if (Main.rand.Next(3) != 0 || Main.rand.NextBool(3))
                    {
                        Main.dust[num305].noGravity = true;
                        dust = Main.dust[num305];
                        dust.scale *= 3f;
                        dust.velocity.X *= 2f;
                        dust.velocity.Y *= 2f;
                    }

                    dust = Main.dust[num305];
                    dust.scale *= 2f;
                    dust.velocity.X *= 1.2f;
                    dust.velocity.Y *= 1.2f;
                    dust.scale *= num302;
                    dust.velocity += projectile.velocity;

                    if (!dust.noGravity)
                        dust.velocity *= 0.5f;
                }
            }
            else
                projectile.ai[0] += 1f;

            projectile.rotation += 0.3f * (float)projectile.direction;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.rand.NextBool(6))
                target.AddBuff(ModContent.BuffType<ShadowflameFireball>(), 240, true);
            else if (Main.rand.NextBool(4))
                target.AddBuff(ModContent.BuffType<ShadowflameFireball>(), 150, true);
            else if (Main.rand.NextBool(2))
                target.AddBuff(ModContent.BuffType<ShadowflameFireball>(), 90, true);
        }
    }
}
