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
            projectile.tileCollide = true;
            projectile.alpha = 0;
            projectile.Calamity().rogue = true;
			projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

			if (!projectile.Calamity().stealthStrike) //normal attack
			{
				projectile.ai[0]++;
				if (projectile.ai[0] > 30f) //0.5 seconds
				{
					NPC target = projectile.Center.ClosestNPCAt(800f);
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
						const float turnSpeed = 10f;
						const float speedMult = 40f;
						Vector2 distNorm = (target.Center - projectile.Center).SafeNormalize(Vector2.UnitX);
						projectile.velocity = (projectile.velocity * (turnSpeed - 1f) + distNorm * speedMult) / turnSpeed;
					}
				}
			}
			else
			{
				//More range
				CalamityGlobalProjectile.HomeInOnNPC(projectile, false, 800f, 40f, 10f);
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

            int num220 = Main.rand.Next(2, 5);
            if (projectile.owner == Main.myPlayer)
            {
                for (int num221 = 0; num221 < num220; num221++)
                {
                    Vector2 value17 = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
                    value17.Normalize();
                    value17 *= (float)Main.rand.Next(10, 51) * 0.01f;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value17.X, value17.Y, ModContent.ProjectileType<SkyBomberGas>(), (int)(projectile.damage * 0.4), projectile.knockBack * 0.4f, projectile.owner, 0f, 0f);
                }
				if (projectile.Calamity().stealthStrike)
				{
					int explode = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<BileExplosion>(), (int)(projectile.damage * 0.5), projectile.knockBack * 0.5f, projectile.owner, 1f);
					Main.projectile[explode].usesLocalNPCImmunity = true;
					Main.projectile[explode].localNPCHitCooldown = 20;
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
