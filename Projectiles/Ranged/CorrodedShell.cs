using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class CorrodedShell : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shell");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.arrow = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 6;
            projectile.timeLeft = 600;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.aiStyle = 1;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            projectile.velocity.X *= 0.9995f;
            projectile.velocity.Y *= 0.9995f;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (projectile.timeLeft % 3 == 0)
            {
                if (projectile.owner == Main.myPlayer)
                {
                    int aura = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<IrradiatedAura>(), (int)(projectile.damage * 0.15), projectile.knockBack, projectile.owner);
                    if (aura.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[aura].Calamity().forceRanged = true;
                        Main.projectile[aura].timeLeft = 40;
                    }
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 32;
            projectile.position = projectile.position - projectile.Size / 2f;
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.Damage();
            Main.PlaySound(SoundID.Item92, (int)projectile.Center.X, (int)projectile.Center.Y);
            int count = Main.rand.Next(6, 15);
            for (int i = 0; i < count; i++)
            {
                int idx = Dust.NewDust(projectile.Center - projectile.velocity / 2f, 0, 0, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default, 2f);
                Main.dust[idx].velocity *= 2f;
                Main.dust[idx].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 60);
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 30);
        }
    }
}
