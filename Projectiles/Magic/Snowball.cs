using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class Snowball : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.coldDamage = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.coldDamage = true;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Snowball");
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 180);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item27, projectile.position);
            //crystal bullet shards
            for (int index1 = 0; index1 < 10; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 68, 0f, 0f, 0, new Color(), 1f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 2f;
            }
            if (projectile.owner == Main.myPlayer)
            {
                for (int index = 0; index < 6; ++index)
                {
                    float SpeedX = -projectile.velocity.X * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                    float SpeedY = -projectile.velocity.Y * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                    int p = Projectile.NewProjectile(projectile.Center.X + SpeedX, projectile.Center.Y + SpeedY, SpeedX, SpeedY, ProjectileID.CrystalShard, projectile.damage / 2, 0f, projectile.owner);
                    Main.projectile[p].ranged = false;
                    Main.projectile[p].magic = true;
                }
            }
            //insert ice shattering dust here
        }
    }
}
