using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class BrackishFlaskProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/BrackishFlask";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brackish Flask");
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.aiStyle = 2;
            projectile.timeLeft = 180;
            aiType = ProjectileID.ThrowingKnife;
            projectile.Calamity().rogue = true;
            projectile.ignoreWater = true;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item107, projectile.Center);
            int randomDust = Utils.SelectRandom(Main.rand, new int[]
            {
                33,
                89
            });
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, randomDust, projectile.oldVelocity.X, projectile.oldVelocity.Y);
            }
            float spread = 45f * 0.0174f;
            double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
            double deltaAngle = spread / 8f;
            double offsetAngle;
            if (projectile.owner == Main.myPlayer)
            {
                if (projectile.Calamity().stealthStrike)
                {
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, -32f, ModContent.ProjectileType<BrackishSpear>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                }

                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<BrackishWaterBlast>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);

                for (int i = 0; i < 4; i++)
                {
                    offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<BrackishWater>(), projectile.damage / 2, projectile.knockBack * 0.5f, projectile.owner, 0f, 0f);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<BrackishWater>(), projectile.damage / 2, projectile.knockBack * 0.5f, projectile.owner, 1f, 0f);
                }
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
