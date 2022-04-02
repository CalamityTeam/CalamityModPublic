using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class AuralisBullet : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bullet");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 5;
            projectile.alpha = 255;
            projectile.timeLeft = 200;
            projectile.extraUpdates = 10;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            projectile.ai[0] += 1f;
            if (projectile.ai[0] > 6f)
            {
                for (int d = 0; d < 5; d++)
                {
                    Dust dust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, projectile.velocity.X, projectile.velocity.Y, 100, CalamityUtils.ColorSwap(Auralis.blueColor, Auralis.greenColor, 1f), 1f)];
                    dust.velocity = Vector2.Zero;
                    dust.position -= projectile.velocity / 5f * d;
                    dust.noGravity = true;
                    dust.scale = 0.65f;
                    dust.noLight = true;
                    dust.color = CalamityUtils.ColorSwap(Auralis.blueColor, Auralis.greenColor, 1f);
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int duration = 420;
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), duration);
            target.AddBuff(BuffID.Ichor, duration);
            target.AddBuff(BuffID.CursedInferno, duration);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            int duration = 420;
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), duration);
            target.AddBuff(BuffID.Ichor, duration);
            target.AddBuff(BuffID.CursedInferno, duration);
        }
    }
}
