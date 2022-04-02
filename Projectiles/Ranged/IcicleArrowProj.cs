using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class IcicleArrowProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Ammo/IcicleArrow";

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.arrow = true;
            projectile.coldDamage = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.coldDamage = true;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Icicle Arrow");
        }

        public override void AI()
        {
            //icicle dust
            if (Main.rand.NextBool(2))
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 68, projectile.velocity.X, projectile.velocity.Y, 0, default, 1.1f);
                Main.dust[index2].noGravity = true;
            }
            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 180);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item27, projectile.position);
            for (int index1 = 0; index1 < 5; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 68, 0f, 0f, 0, new Color(), 1f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 1.5f;
                Main.dust[index2].scale *= 0.9f;
            }
            if (projectile.owner == Main.myPlayer)
            {
                for (int index = 0; index < 3; ++index)
                {
                    float SpeedX = -projectile.velocity.X * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                    float SpeedY = -projectile.velocity.Y * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                    Projectile.NewProjectile(projectile.Center.X + SpeedX, projectile.Center.Y + SpeedY, SpeedX, SpeedY, ProjectileID.CrystalShard, projectile.damage / 3, 0f, projectile.owner);
                }
            }
        }
    }
}
