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
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = 3;
            Projectile.timeLeft = 180;
            AIType = ProjectileID.WoodenBoomerang;
            Projectile.Calamity().rogue = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
            int numProj = Projectile.Calamity().stealthStrike ? 3 : 2;
            float rotation = MathHelper.ToRadians(20);
            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < numProj + 1; i++)
                {
                    Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, perturbedSpeed.X * 0.25f, perturbedSpeed.Y * 0.25f, ModContent.ProjectileType<Brimblade2>(), (int)(Projectile.damage * 0.6), Projectile.knockBack * 0.5f, Projectile.owner, 0f, 0f);
                }
            }
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, (int)CalamityDusts.Brimstone, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
            int numProj = Projectile.Calamity().stealthStrike ? 3 : 2;
            float rotation = MathHelper.ToRadians(20);
            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < numProj + 1; i++)
                {
                    Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, perturbedSpeed.X * 0.25f, perturbedSpeed.Y * 0.25f, ModContent.ProjectileType<Brimblade2>(), (int)(Projectile.damage * 0.6), Projectile.knockBack * 0.5f, Projectile.owner, 0f, 0f);
                }
            }
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, (int)CalamityDusts.Brimstone, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
