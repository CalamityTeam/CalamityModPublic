using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
	public class ToxicantTwisterTwoPointZero : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/ToxicantTwister";

		private int lifeTime = 300;
		private int targetIndex = -1;

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
			projectile.ignoreWater = true;
			projectile.penetrate = -1;
            projectile.timeLeft = lifeTime;
            projectile.Calamity().rogue = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 12;
        }

        public override void AI()
        {
            if (projectile.Calamity().stealthStrike)
            {
                if (projectile.timeLeft % 20 == 0 && Main.myPlayer == projectile.owner)
                {
                    for (int i = 0; i < 2; i++)
                        Projectile.NewProjectile(projectile.Center, projectile.velocity.RotatedByRandom(0.1f) * -0.6f, ModContent.ProjectileType<ToxicantTwisterDust>(), (int)(projectile.damage * 0.35), 0f, projectile.owner);
                }
                projectile.rotation += 0.06f * (projectile.velocity.X > 0).ToDirectionInt();
            }

            // Boomerang rotation
            projectile.rotation += 0.4f * projectile.direction;

            // Boomerang sound
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 8;
                Main.PlaySound(SoundID.Item7, projectile.position);
            }

			projectile.ai[1]++;
            if (projectile.ai[0] != 0f)
            {
                float returnSpeed = 30f;
                float acceleration = 1.4f;

                Player owner = Main.player[projectile.owner];

                // Delete the projectile if it's excessively far away.
				Vector2 projVector = owner.Center - projectile.Center;
                float dist = projVector.Length();
                if (dist > 3000f)
                    projectile.Kill();

                dist = returnSpeed / dist;
                projVector.X *= dist;
                projVector.Y *= dist;

                // Home back in on the player.
                if (projectile.velocity.X < projVector.X)
                {
                    projectile.velocity.X += acceleration;
                    if (projectile.velocity.X < 0f && projVector.X > 0f)
                        projectile.velocity.X += acceleration;
                }
                else if (projectile.velocity.X > projVector.X)
                {
                    projectile.velocity.X -= acceleration;
                    if (projectile.velocity.X > 0f && projVector.X < 0f)
                        projectile.velocity.X -= acceleration;
                }
                if (projectile.velocity.Y < projVector.Y)
                {
                    projectile.velocity.Y += acceleration;
                    if (projectile.velocity.Y < 0f && projVector.Y > 0f)
                        projectile.velocity.Y += acceleration;
                }
                else if (projectile.velocity.Y > projVector.Y)
                {
                    projectile.velocity.Y -= acceleration;
                    if (projectile.velocity.Y > 0f && projVector.Y < 0f)
                        projectile.velocity.Y -= acceleration;
                }

                // Delete the projectile if it touches its owner.
                if (Main.myPlayer == projectile.owner)
                    if (projectile.Hitbox.Intersects(owner.Hitbox))
                        projectile.Kill();
            }
			else if (projectile.ai[1] > 40f)
			{
				NPC closestTarget = projectile.Center.ClosestNPCAt(1769f, true, true);
				if (closestTarget != null)
				{
					projectile.extraUpdates = 1;
					targetIndex = closestTarget.whoAmI;
					float inertia = 20f;
					float homingVelocity = projectile.Calamity().stealthStrike ? 30f : 20f;
                    Vector2 moveDirection = projectile.SafeDirectionTo(closestTarget.Center, Vector2.UnitY);

                    projectile.velocity = (projectile.velocity * inertia + moveDirection * homingVelocity) / (inertia + 1f);
				}
			}
			else
			{
                projectile.velocity *= 0.985f;
			}
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if (projectile.ai[1] <= 40f && projectile.ai[0] != 1f)
			{
				damage /= 3;
			}
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			if (targetIndex == target.whoAmI)
				projectile.ai[0] = 1f;

            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 180);
            Main.PlaySound(SoundID.Item20, projectile.position);
            for (int k = 0; k < 10; k++)
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, (int)CalamityDusts.SulfurousSeaAcid, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
			projectile.ai[0] = 1f;
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 180);
            Main.PlaySound(SoundID.Item20, projectile.position);
            for (int k = 0; k < 10; k++)
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, (int)CalamityDusts.SulfurousSeaAcid, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
        }
    }
}
