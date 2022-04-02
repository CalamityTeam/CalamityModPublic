using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
	public class MagicHammer : ModProjectile
    {
		private int counter = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hammer");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 60;
            projectile.height = 60;
            projectile.alpha = 255;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = 6;
            projectile.timeLeft = 180;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 6;
        }

        public override void AI()
        {
            projectile.localAI[1] += 1f;
            if (projectile.localAI[1] > 10f && Main.rand.NextBool(3))
            {
                projectile.alpha -= 5;
                if (projectile.alpha < 50)
                {
                    projectile.alpha = 50;
                }
            }

            projectile.rotation += 0.075f;

			counter++;
			if (counter == 30)
			{
				projectile.netUpdate = true;
			}
			else if (counter < 30)
			{
				return;
			}

            int targetIndex = -1;
            Vector2 targetVec = projectile.Center;
            float maxDistance = MagicHat.Range;
            if (projectile.localAI[0] > 0f)
            {
                projectile.localAI[0] -= 1f;
            }
			Player player = Main.player[projectile.owner];
            if (projectile.ai[0] == 0f && projectile.localAI[0] == 0f)
            {
				if (player.HasMinionAttackTargetNPC)
				{
					NPC npc = Main.npc[player.MinionAttackTargetNPC];
					if (npc.CanBeChasedBy(projectile, false) && (projectile.ai[0] == 0f || projectile.ai[0] == player.MinionAttackTargetNPC + 1f))
					{
						float targetDist = Vector2.Distance(npc.Center, targetVec);
						if (targetDist < maxDistance)
						{
							targetVec = npc.Center;
							targetIndex = player.MinionAttackTargetNPC;
						}
					}
				}
				else
				{
					for (int i = 0; i < Main.npc.Length; i++)
					{
						NPC npc = Main.npc[i];
						if (npc.CanBeChasedBy(projectile, false) && (projectile.ai[0] == 0f || projectile.ai[0] == i + 1f))
						{
							float targetDist = Vector2.Distance(npc.Center, targetVec);
							if (targetDist < maxDistance)
							{
								targetVec = npc.Center;
								targetIndex = i;
							}
						}
					}
				}
                if (targetIndex >= 0)
                {
                    projectile.ai[0] = targetIndex + 1f;
                    projectile.netUpdate = true;
                }
            }
            if (projectile.localAI[0] == 0f && projectile.ai[0] == 0f)
            {
                projectile.localAI[0] = 30f;
            }
            bool canHome = false;
            if (projectile.ai[0] != 0f)
            {
                int target = (int)(projectile.ai[0] - 1f);
				NPC npc = Main.npc[target];
                if (npc.CanBeChasedBy(projectile, false) && npc.immune[projectile.owner] == 0)
                {
					float targetDist = Vector2.Distance(npc.Center, targetVec);
                    if (targetDist < maxDistance * 1.25f)
                    {
                        canHome = true;
                        targetVec = npc.Center;
                    }
                }
                else
                {
                    projectile.ai[0] = 0f;
                    canHome = false;
                    projectile.netUpdate = true;
                }
            }
            if (canHome)
            {
                Vector2 velocity = targetVec - projectile.Center;
                float trajectory = projectile.velocity.ToRotation();
                float target = velocity.ToRotation();
                float rotateAmt = target - trajectory;
                rotateAmt = MathHelper.WrapAngle(rotateAmt);
                projectile.velocity = projectile.velocity.RotatedBy(rotateAmt * 0.25, default);
            }
            float speed = projectile.velocity.Length();
            projectile.velocity.Normalize();
            projectile.velocity *= speed + 0.0025f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 56, 0, projectile.alpha);

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 dspeed = new Vector2(Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-7f, 7f));
                int dust = Dust.NewDust(projectile.Center, 1, 1, 67, dspeed.X, dspeed.Y, 50, default, 1.2f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
