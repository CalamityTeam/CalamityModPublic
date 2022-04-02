using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class BrackishWater : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Water");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 60;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0f / 255f, (255 - projectile.alpha) * 0.25f / 255f, (255 - projectile.alpha) * 0.5f / 255f);
            if (projectile.localAI[0] == 0f && projectile.ai[0] == 1f)
            {
                Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 21);
                projectile.localAI[0] += 1f;
            }
            if (projectile.timeLeft % 2 == 0)
            {
                int randomDust = Utils.SelectRandom(Main.rand, new int[]
                {
                    33,
                    89
                });
                int water = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, randomDust, 0f, 0f, 100, default, 1.2f);
                Main.dust[water].noGravity = true;
                Main.dust[water].velocity *= 0.5f;
                Main.dust[water].velocity += projectile.velocity * 0.1f;
            }
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<BrackishWaterBlast>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Venom, 90);
            target.AddBuff(BuffID.Poisoned, 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Venom, 90);
            target.AddBuff(BuffID.Poisoned, 180);
        }
    }
}
