using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Projectiles.BaseProjectiles;

namespace CalamityMod.Projectiles.Melee.Spears
{
    public class TenebreusTidesProjectile : BaseSpearProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tenebreus Tides");
        }

        public override void SetDefaults()
        {
            projectile.width = 46;  //The width of the .png file in pixels divided by 2.
            projectile.height = 46;  //The height of the .png file in pixels divided by 2.
            projectile.aiStyle = 19;
            projectile.melee = true;  //Dictates whether projectile is a melee-class weapon.
            projectile.timeLeft = 90;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.ownerHitCheck = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 15;
        }

        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 2.4f;
        public override float ForwardSpeed => 0.95f;
        public override Action<Projectile> EffectBeforeReelback => (proj) =>
        {
            Projectile.NewProjectile(projectile.Center.X + projectile.velocity.X, projectile.Center.Y + projectile.velocity.Y, projectile.velocity.X * 2.4f, projectile.velocity.Y * 2.4f, ModContent.ProjectileType<TenebreusTidesWaterProjectile>(), (int)(projectile.damage * 0.65), projectile.knockBack * 0.85f, projectile.owner, 0f, 0f);
        };
        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 33, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 300);
            SwordSpam(target.Center);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 300);
            SwordSpam(target.Center);
        }

        // Spawns a storm of water projectiles on-hit.
        public void SwordSpam(Vector2 targetPos)
        {
            int projAmt = 3;
            for (int i = 0; i < projAmt; ++i)
            {
                int type = Main.rand.NextBool() ? ModContent.ProjectileType<TenebreusTidesWaterSword>() : ModContent.ProjectileType<TenebreusTidesWaterSpear>();
                if (projectile.owner == Main.myPlayer)
                {
                    CalamityUtils.ProjectileBarrage(projectile.Center, targetPos, Main.rand.NextBool(), 1000f, 1400f, 80f, 900f, Main.rand.NextFloat(25f, 35f), type, projectile.damage / 2, projectile.knockBack * 0.5f, projectile.owner);
                }
            }
        }
    }
}
