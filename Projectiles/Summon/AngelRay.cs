using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class AngelRay : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ray");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.ignoreWater = true;
            projectile.penetrate = 10;
            projectile.extraUpdates = 200;
            projectile.timeLeft = 600;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.2f, 0.01f, 0.1f);
            projectile.ai[0] += 1f;
            if (projectile.ai[0] % 6f == 0f && projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<AngelOrb>(), projectile.damage, projectile.knockBack, projectile.owner);
            }

            if (projectile.ai[0] > 16f && projectile.ai[0] % 2f == 0f)
            {
                Vector2 source = projectile.position;
                int pink = Dust.NewDust(source, 1, 1, (int)CalamityDusts.ProfanedFire, 0f, 0f, 0, default, 1.25f);
                Main.dust[pink].noGravity = true;
                Main.dust[pink].noLight = true;
                Main.dust[pink].position = source;
                Main.dust[pink].scale = Main.rand.Next(70, 110) * 0.013f;
                Main.dust[pink].velocity *= 0.1f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.penetrate--;
            if (projectile.penetrate <= 0)
            {
                projectile.Kill();
            }
            else
            {
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(ModContent.BuffType<BanishingFire>(), 300);

        public override void OnHitPvp(Player target, int damage, bool crit) => target.AddBuff(ModContent.BuffType<BanishingFire>(), 300);
    }
}
