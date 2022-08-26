using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ScourgeoftheDesertProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/ScourgeoftheDesert";

        private int StealthDamageCap = 0;
        private int BaseDamage = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scourge");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.aiStyle = ProjAIStyleID.StickProjectile;
            AIType = ProjectileID.BoneJavelin;
            Projectile.penetrate = 3;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.Calamity().stealthStrike)
            {
                if (StealthDamageCap == 0)
                {
                    BaseDamage = Projectile.damage;
                }

                Projectile.damage = (int)((BaseDamage * ((StealthDamageCap > 10 ? 10 : StealthDamageCap) * 20) / 100) + BaseDamage); //20% damage boost per hit, max of 200%
                StealthDamageCap++;
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (Projectile.Calamity().stealthStrike)
            {
                if (StealthDamageCap == 0)
                {
                    BaseDamage = Projectile.damage;
                }

                Projectile.damage = (int)((BaseDamage * ((StealthDamageCap > 10 ? 10 : StealthDamageCap) * 20) / 100) + BaseDamage); //20% damage boost per hit, max of 200%
                StealthDamageCap++;
            }
        }

        public override void AI()
        {
            if (Projectile.Calamity().stealthStrike)
            {
                Projectile.penetrate = 20 - StealthDamageCap;
                Projectile.velocity.Y *= 1.025f;
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 32);
            }
            Projectile.velocity.X *= 1.025f;
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + MathHelper.PiOver4;
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation -= 1.57f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }
    }
}
