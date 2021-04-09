using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneLaser : ModProjectile
    {
        private int splitTimer = 60;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Laser");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.aiStyle = 1;
            projectile.hostile = true;
            projectile.scale = 2f;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.alpha = 120;
            aiType = ProjectileID.DeathLaser;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(splitTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            splitTimer = reader.ReadInt32();
        }

        public override void AI()
        {
            splitTimer--;
            if (splitTimer <= 0)
            {
                int numProj = 2;
                float rotation = MathHelper.ToRadians(20);
                if (projectile.owner == Main.myPlayer)
                {
                    for (int i = 0; i < numProj + 1; i++)
                    {
                        Vector2 perturbedSpeed = new Vector2(projectile.velocity.X, projectile.velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<BrimstoneLaserSplit>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                    }
                }
                projectile.Kill();
            }
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.5f / 255f, (255 - projectile.alpha) * 0.05f / 255f, (255 - projectile.alpha) * 0.05f / 255f);
            projectile.velocity.X *= 1.01f;
            projectile.velocity.Y *= 1.01f;
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 50, 50, projectile.alpha);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
        }
    }
}
