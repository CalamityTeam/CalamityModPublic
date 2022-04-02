using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.BaseProjectiles;

namespace CalamityMod.Projectiles.Melee.Spears
{
    public class HellionFlowerSpearProjectile : BaseSpearProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spear");
        }

        public override void SetDefaults()
        {
            projectile.width = 40;  //The width of the .png file in pixels divided by 2.
            projectile.aiStyle = 19;
            projectile.melee = true;  //Dictates whether this is a melee-class weapon.
            projectile.timeLeft = 90;
            projectile.height = 40;  //The height of the .png file in pixels divided by 2.
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.ownerHitCheck = true;
        }

        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 2.4f;
        public override float ForwardSpeed => 0.95f;
        public override Action<Projectile> EffectBeforeReelback => (proj) =>
        {
            Projectile.NewProjectile(projectile.Center.X + projectile.velocity.X, projectile.Center.Y + projectile.velocity.Y, projectile.velocity.X * 2.4f, projectile.velocity.Y * 2.4f, ModContent.ProjectileType<HellionSpike>(), (int)(projectile.damage * 0.65), projectile.knockBack * 0.85f, projectile.owner, 0f, 0f);
        };

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 8;
            OnHitEffects(target.Center, crit);
            target.AddBuff(BuffID.Venom, 300);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            OnHitEffects(target.Center, crit);
            target.AddBuff(BuffID.Venom, 300);
        }

        private void OnHitEffects(Vector2 targetPos, bool crit)
        {
            if (crit)
            {
                if (projectile.owner == Main.myPlayer)
                {
                    Projectile petal = CalamityUtils.ProjectileBarrage(projectile.Center, targetPos, Main.rand.NextBool(), 800f, 800f, 0f, 800f, 10f, ProjectileID.FlowerPetal, (int)(projectile.damage * 0.5), projectile.knockBack * 0.5f, projectile.owner, true);
                    if (petal.whoAmI.WithinBounds(Main.maxProjectiles))
                    {
                        petal.Calamity().forceMelee = true;
                        petal.localNPCHitCooldown = -1;
                    }
                }
            }
        }
    }
}
