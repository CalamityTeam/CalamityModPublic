using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class BrimbladeProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Brimblade";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimblade");
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.aiStyle = 3;
            projectile.timeLeft = 180;
            aiType = ProjectileID.WoodenBoomerang;
            projectile.Calamity().rogue = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
            int numProj = projectile.Calamity().stealthStrike ? 3 : 2;
            float rotation = MathHelper.ToRadians(20);
            if (projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < numProj + 1; i++)
                {
                    Vector2 perturbedSpeed = new Vector2(projectile.velocity.X, projectile.velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, perturbedSpeed.X * 0.25f, perturbedSpeed.Y * 0.25f, ModContent.ProjectileType<Brimblade2>(), (int)(projectile.damage * 0.6), projectile.knockBack * 0.5f, projectile.owner, 0f, 0f);
                }
            }
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, (int)CalamityDusts.Brimstone, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
            int numProj = projectile.Calamity().stealthStrike ? 3 : 2;
            float rotation = MathHelper.ToRadians(20);
            if (projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < numProj + 1; i++)
                {
                    Vector2 perturbedSpeed = new Vector2(projectile.velocity.X, projectile.velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, perturbedSpeed.X * 0.25f, perturbedSpeed.Y * 0.25f, ModContent.ProjectileType<Brimblade2>(), (int)(projectile.damage * 0.6), projectile.knockBack * 0.5f, projectile.owner, 0f, 0f);
                }
            }
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, (int)CalamityDusts.Brimstone, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
