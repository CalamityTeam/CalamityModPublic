using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
	public class ToxicantTwisterProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Toxicant Twister");
        }

        public override void SetDefaults()
        {
            projectile.width = 42;
            projectile.height = 46;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = 5;
            projectile.aiStyle = 3;
            projectile.timeLeft = 180;
            aiType = ProjectileID.WoodenBoomerang;
            projectile.Calamity().rogue = true;
			projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 12;
        }
        public override void AI()
        {
            if (projectile.Calamity().stealthStrike)
            {
                if (projectile.timeLeft % 50f == 0f)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Projectile.NewProjectile(projectile.Center,
                            (-1f * projectile.velocity).RotatedByRandom(0.1f) * 0.6f,
                            ModContent.ProjectileType<ToxicantTwisterDust>(), projectile.damage, 0f, projectile.owner);
                    }
                }
                projectile.rotation += 0.06f * (projectile.velocity.X > 0).ToDirectionInt();
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 180);
            Main.PlaySound(SoundID.Item20, projectile.position);
            for (int k = 0; k < 10; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, 
                    projectile.width, projectile.height, (int)CalamityDusts.SulfurousSeaAcid,
                    projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 180);
            Main.PlaySound(SoundID.Item20, projectile.position);
            for (int k = 0; k < 10; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity,
                    projectile.width, projectile.height, (int)CalamityDusts.SulfurousSeaAcid,
                    projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
