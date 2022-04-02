using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class HolyLaser : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laser");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 2;
            projectile.extraUpdates = 100;
            projectile.timeLeft = 180;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 9f)
            {
                int num249 = Main.rand.Next(2);
                if (num249 == 0)
                {
                    num249 = 244;
                }
                else
                {
                    num249 = 246;
                }
                Vector2 vector33 = projectile.position;
                vector33 -= projectile.velocity * 0.25f;
                int num448 = Dust.NewDust(vector33, 1, 1, num249, 0f, 0f, 0, default, 0.25f);
                Main.dust[num448].position = vector33;
                Main.dust[num448].scale = (float)Main.rand.Next(70, 110) * 0.013f;
                Main.dust[num448].velocity *= 0.1f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);

            if (projectile.owner == Main.myPlayer)
            {
                int proj = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<FuckYou>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
                if (proj.WithinBounds(Main.maxProjectiles))
                    Main.projectile[proj].Calamity().forceMagic = true;
            }
        }
    }
}
