using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee.Yoyos;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Items.Weapons.Melee;

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
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Projectile parent = FindParent();
            if (parent != null && parent.active)
            {
                Projectile.Center = parent.Center;
                Projectile.timeLeft = 2;
            }
            else
            {
                Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);
            if (target.life <= 0 && (FindParent().ModProjectile as MicrowaveYoyo).soundCooldown <= 0)
            {
                SoundEngine.PlaySound(TheMicrowave.BeepSound, Projectile.Center);
                (FindParent().ModProjectile as MicrowaveYoyo).soundCooldown = 60;
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);
            if (target.statLife <= 0 && (FindParent().ModProjectile as MicrowaveYoyo).soundCooldown <= 0)
            {
                SoundEngine.PlaySound(TheMicrowave.BeepSound, Projectile.Center);
                (FindParent().ModProjectile as MicrowaveYoyo).soundCooldown = 60;
            }
        }

        private Projectile FindParent()
        {
            Projectile parent = null;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.identity == Projectile.ai[0] && p.active && p.type == ModContent.ProjectileType<MicrowaveYoyo>() && p.owner == Projectile.owner)
                {
                    parent = p;
                    break;
                }
            }
            return parent;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, radius, targetHitbox);
    }
}
