using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Spears
{
    public class VulcaniteLanceProj : BaseSpearProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lance");
        }

        public override void SetDefaults()
        {
            Projectile.width = 95;  //The width of the .png file in pixels divided by 2.
            Projectile.aiStyle = ProjAIStyleID.Spear;
            Projectile.DamageType = TrueMeleeDamageClass.Instance;
            Projectile.timeLeft = 90;
            Projectile.height = 95;  //The height of the .png file in pixels divided by 2.
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
        }
        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 2.4f;
        public override float ForwardSpeed => 0.95f;
        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, Main.rand.NextBool(3) ? 16 : 127, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);

            Vector2 goreVec = Projectile.Center + Projectile.velocity;
            if (Main.rand.NextBool(8) && Main.netMode != NetmodeID.Server)
            {
                int smoke = Gore.NewGore(Projectile.GetSource_FromAI(), goreVec, default, Main.rand.Next(375, 378), 1f);
                Main.gore[smoke].behindTiles = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 6;
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
            if (Projectile.owner == Main.myPlayer)
            {
                int boom = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FuckYou>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
                if (boom.WithinBounds(Main.maxProjectiles))
                    Main.projectile[boom].DamageType = DamageClass.Melee;
            }
            if (crit)
            {
                var source = Projectile.GetSource_FromThis();
                for (int i = 0; i < 2; i++)
                {
                    if (Projectile.owner == Main.myPlayer)
                    {
                        CalamityUtils.ProjectileBarrage(source, Projectile.Center, targetPos, Main.rand.NextBool(), 800f, 800f, 0f, 800f, 10f, ModContent.ProjectileType<TinyFlare>(), (int)(Projectile.damage * 0.5), 2f, Projectile.owner, true);
                    }
                }
            }
        }
    }
}
