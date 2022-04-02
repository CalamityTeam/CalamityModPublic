using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.BaseProjectiles;

namespace CalamityMod.Projectiles.Melee.Spears
{
    public class ExsanguinationLanceProjectile : BaseSpearProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lance");
        }

        public override void SetDefaults()
        {
            projectile.width = 95;  //The width of the .png file in pixels divided by 2.
            projectile.aiStyle = 19;
            projectile.melee = true;  //Dictates whether this is a melee-class weapon.
            projectile.timeLeft = 90;
            projectile.height = 95;  //The height of the .png file in pixels divided by 2.
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.ownerHitCheck = true;
            projectile.Calamity().trueMelee = true;
        }
        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 2.4f;
        public override float ForwardSpeed => 0.95f;
        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, Main.rand.NextBool(3) ? 16 : 127, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);

            Vector2 goreVec = projectile.Center + projectile.velocity;
            if (Main.rand.NextBool(8))
            {
                int smoke = Gore.NewGore(goreVec, default, Main.rand.Next(375, 378), 1f);
                Main.gore[smoke].behindTiles = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 6;
            OnHitEffects(target.Center, crit);
            target.AddBuff(BuffID.OnFire, 240);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            OnHitEffects(target.Center, crit);
            target.AddBuff(BuffID.OnFire, 240);
        }

        private void OnHitEffects(Vector2 targetPos, bool crit)
        {
            if (projectile.owner == Main.myPlayer)
            {
                int boom = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<FuckYou>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
                if (boom.WithinBounds(Main.maxProjectiles))
                    Main.projectile[boom].Calamity().forceMelee = true;
            }
            if (crit)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (projectile.owner == Main.myPlayer)
                    {
                        CalamityUtils.ProjectileBarrage(projectile.Center, targetPos, Main.rand.NextBool(), 800f, 800f, 0f, 800f, 10f, ModContent.ProjectileType<TinyFlare>(), (int)(projectile.damage * 0.5), 2f, projectile.owner, true);
                    }
                }
            }
        }
    }
}
