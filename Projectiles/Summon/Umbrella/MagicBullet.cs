using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon.Umbrella
{
    public class MagicBullet : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Ranged/BloodClotFriendly";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bullet");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.light = 0.5f;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 7;
            Projectile.scale = 1.18f;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.BulletHighVelocity;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (!target.betsysCurse)
                target.AddBuff(BuffID.BetsysCurse, 180);
            if (!target.ichor)
                target.AddBuff(BuffID.Ichor, 180);
            if (target.Calamity().marked <= 0)
                target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 180);
            if (target.Calamity().aCrunch <= 0)
                target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 180);
            if (target.Calamity().wDeath <= 0)
                target.AddBuff(ModContent.BuffType<WhisperingDeath>(), 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 180);
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 180);
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 180);
            target.AddBuff(ModContent.BuffType<WhisperingDeath>(), 180);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(189, 51, 164, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor) => Projectile.timeLeft < 600;
    }
}
