using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 60;
            projectile.height = 60;
            projectile.alpha = 255;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = 10;
            projectile.timeLeft = 180;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
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
			if (counter <= 30)
			{
				counter++;
				return;
			}
            int num716 = -1;
            Vector2 vector59 = projectile.Center;
            float num717 = 700f;
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
					if (npc.CanBeChasedBy(projectile, false) && (projectile.ai[0] == 0f || projectile.ai[0] == (float)(player.MinionAttackTargetNPC + 1)))
					{
						Vector2 center4 = npc.Center;
						float num719 = Vector2.Distance(center4, vector59);
						if (num719 < num717)
						{
							num717 = num719;
							vector59 = center4;
							num716 = player.MinionAttackTargetNPC;
						}
					}
				}
				else
				{
					for (int num718 = 0; num718 < Main.npc.Length; num718++)
					{
						NPC nPC6 = Main.npc[num718];
						if (nPC6.CanBeChasedBy(projectile, false) && (projectile.ai[0] == 0f || projectile.ai[0] == (float)(num718 + 1)))
						{
							Vector2 center4 = nPC6.Center;
							float num719 = Vector2.Distance(center4, vector59);
							if (num719 < num717)
							{
								num717 = num719;
								vector59 = center4;
								num716 = num718;
							}
						}
					}
				}
                if (num716 >= 0)
                {
                    projectile.ai[0] = (float)(num716 + 1);
                    projectile.netUpdate = true;
                }
            }
            if (projectile.localAI[0] == 0f && projectile.ai[0] == 0f)
            {
                projectile.localAI[0] = 30f;
            }
            bool flag32 = false;
            if (projectile.ai[0] != 0f)
            {
                int num720 = (int)(projectile.ai[0] - 1f);
                if (Main.npc[num720].active && !Main.npc[num720].dontTakeDamage && Main.npc[num720].immune[projectile.owner] == 0)
                {
                    float num721 = Main.npc[num720].position.X + (float)(Main.npc[num720].width / 2);
                    float num722 = Main.npc[num720].position.Y + (float)(Main.npc[num720].height / 2);
                    float num723 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num721) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num722);
                    if (num723 < 1000f)
                    {
                        flag32 = true;
                        vector59 = Main.npc[num720].Center;
                    }
                }
                else
                {
                    projectile.ai[0] = 0f;
                    flag32 = false;
                    projectile.netUpdate = true;
                }
            }
            if (flag32)
            {
                Vector2 v = vector59 - projectile.Center;
                float num724 = projectile.velocity.ToRotation();
                float num725 = v.ToRotation();
                double num726 = (double)(num725 - num724);
                num726 = MathHelper.WrapAngle((float)num726);
                projectile.velocity = projectile.velocity.RotatedBy(num726 * 0.25, default);
            }
            float num727 = projectile.velocity.Length();
            projectile.velocity.Normalize();
            projectile.velocity *= num727 + 0.0025f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 56, 0, projectile.alpha);
        }

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
