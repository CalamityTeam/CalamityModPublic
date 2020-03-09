using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class BloodBreath : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fire");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.penetrate = 2;
            projectile.extraUpdates = 3;
            projectile.timeLeft = 40;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 9;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.77f / 255f, (255 - projectile.alpha) * 0.15f / 255f, (255 - projectile.alpha) * 0.08f / 255f);
            if (projectile.ai[0] > 7f)
            {
                float dustScale = 1f;
                if (projectile.ai[0] == 8f)
                {
                    dustScale = 0.25f;
                }
                else if (projectile.ai[0] == 9f)
                {
                    dustScale = 0.5f;
                }
                else if (projectile.ai[0] == 10f)
                {
                    dustScale = 0.75f;
                }
                projectile.ai[0] += 1f;
                if (Main.rand.NextBool(2))
                {
                    for (int i = 0; i < 1; i++)
                    {
                        int idx = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 5, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default, 1f);
                        Dust dust = Main.dust[idx];
                        if (Main.rand.NextBool(3))
                        {
                            dust.noGravity = true;
                            dust.scale *= 3f;
                            dust.velocity.X *= 2f;
                            dust.velocity.Y *= 2f;
                        }
                        else
                        {
                            dust.scale *= 1.5f;
                        }
                        dust.velocity.X *= 1.2f;
                        dust.velocity.Y *= 1.2f;
                        dust.scale *= dustScale;
                    }
                }
            }
            else
            {
                projectile.ai[0] += 1f;
            }
            projectile.rotation += 0.3f * projectile.direction;
        }
    }
}
