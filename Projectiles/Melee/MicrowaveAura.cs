using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee.Yoyos;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class MicrowaveAura : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int radius = 100;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Microwave Radiation");
        }

        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.width = 200;
            projectile.height = 200;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Projectile parent = FindParent();
            if (parent != null && parent.active)
            {
                projectile.Center = parent.Center;
                projectile.timeLeft = 2;
            }
            else
            {
                projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);
            if (target.life <= 0 && (FindParent().modProjectile as MicrowaveYoyo).soundCooldown <= 0)
            {
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/MicrowaveBeep"), (int)projectile.Center.X, (int)projectile.Center.Y);
                (FindParent().modProjectile as MicrowaveYoyo).soundCooldown = 60;
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);
            if (target.statLife <= 0 && (FindParent().modProjectile as MicrowaveYoyo).soundCooldown <= 0)
            {
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/MicrowaveBeep"), (int)projectile.Center.X, (int)projectile.Center.Y);
                (FindParent().modProjectile as MicrowaveYoyo).soundCooldown = 60;
            }
        }

        private Projectile FindParent()
        {
            Projectile parent = null;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.identity == projectile.ai[0] && p.active && p.type == ModContent.ProjectileType<MicrowaveYoyo>() && p.owner == projectile.owner)
                {
                    parent = p;
                    break;
                }
            }
            return parent;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, radius, targetHitbox);
    }
}
