using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class JawsShockwave : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaper Tooth Shockwave");
        }

        public override void SetDefaults()
        {
            projectile.width = 320;
            projectile.height = 320;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 10;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.timeLeft >= 5)
            {
                for (int i = 0; i < 50; i++)
                {
                    int dustToUse = Main.rand.Next(0, 4);
                    int dustType = 0;
                    switch (dustToUse)
                    {
                        case 0:
                            dustType = 33;
                            break;
                        case 1:
                            dustType = 101;
                            break;
                        case 2:
                            dustType = 111;
                            break;
                        case 3:
                            dustType = 180;
                            break;
                    }

                    Vector2 dustVelocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                    dustVelocity.Normalize();
                    dustVelocity *= 16;

                    int dust = Dust.NewDust(projectile.Center, 1, 1, dustType, dustVelocity.X, dustVelocity.Y, 0, default, 1.5f);
                    Main.dust[dust].noGravity = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 240);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 240);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, 160f, targetHitbox);
    }
}
