using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SkyfinNuke : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/SkyfinBombers";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skyfin Nuke");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 720;
            projectile.alpha = 0;
            projectile.Calamity().rogue = true;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

            const float turnSpeed = 20f;
            float speedMult = projectile.Calamity().stealthStrike ? 9f : 12f;
            const float homingRange = 300f;
            if (!projectile.Calamity().stealthStrike) //normal attack
            {
                projectile.ai[0]++;
                if (projectile.ai[0] > 30f) //0.5 seconds
                {
                    NPC target = projectile.Center.ClosestNPCAt(homingRange);
                    // Ignore targets above the nuke
                    if (target != null)
                    {
                        if (target.Bottom.Y < projectile.Top.Y)
                        {
                            target = null;
                        }
                    }
                    if (target != null)
                    {
                        Vector2 distNorm = (target.Center - projectile.Center).SafeNormalize(Vector2.UnitX);
                        projectile.velocity = (projectile.velocity * (turnSpeed - 1f) + distNorm * speedMult) / turnSpeed;
                    }
                }
            }
            else
            {
                //More range
                CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, homingRange, speedMult, turnSpeed);
            }
        }

        public override void Kill(int timeLeft)
        {
            projectile.extraUpdates = 0;
            //Dust
            for (int i = 0; i < 30;i++)
            {
                Vector2 dspeed = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f));
                Dust.NewDust(projectile.Center, 1, 1, (int)CalamityDusts.SulfurousSeaAcid, dspeed.X, dspeed.Y, 0, default, 1.1f);
            }
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();

            int cloudAmt = Main.rand.Next(2, 5);
            if (projectile.owner == Main.myPlayer)
            {
                for (int c = 0; c < cloudAmt; c++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(50f, 10f, 50f, 0.01f);
                    Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<SkyBomberGas>(), (int)(projectile.damage * 0.4), projectile.knockBack * 0.4f, projectile.owner);
                }
                if (projectile.Calamity().stealthStrike)
                {
                    int explode = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<BileExplosion>(), (int)(projectile.damage * 0.4), projectile.knockBack * 0.4f, projectile.owner, 1f);
                    Main.projectile[explode].usesLocalNPCImmunity = true;
                    Main.projectile[explode].localNPCHitCooldown = 30;
                }
            }

            Main.PlaySound(SoundID.Item14, projectile.Bottom);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
        }
    }
}
