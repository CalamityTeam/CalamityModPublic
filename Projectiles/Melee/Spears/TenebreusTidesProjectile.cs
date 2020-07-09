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
            //projectile.Calamity().trueMelee = true;
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
                float startOffsetX = Main.rand.NextFloat(1000f, 1400f) * (Main.rand.NextBool() ? -1f : 1f);
                float startOffsetY = Main.rand.NextFloat(80f, 900f) * (Main.rand.NextBool() ? -1f : 1f);
                Vector2 startPos = new Vector2(targetPos.X + startOffsetX, targetPos.Y + startOffsetY);
                Vector2 projVel = targetPos - startPos;

                // Add some randomness / inaccuracy to the projectile target location
                projVel.X += Main.rand.NextFloat(-5f, 5f);
                projVel.Y += Main.rand.NextFloat(-5f, 5f);
                float speed = Main.rand.NextFloat(25f, 35f);
                float dist = projVel.Length();
                dist = speed / dist;
                projVel.X *= dist;
                projVel.Y *= dist;
                if (projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(startPos, projVel, type, projectile.damage / 2, projectile.knockBack * 0.5f, projectile.owner, 0f, 0f);
                }
            }
        }
    }
}
