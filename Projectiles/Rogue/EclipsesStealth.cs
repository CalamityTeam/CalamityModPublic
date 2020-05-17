using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class EclipsesStealth : ModProjectile
    {
		private bool changedTimeLeft = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eclipse's Stealth");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 25;
            projectile.height = 25;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 300;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 30;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.timeLeft % 5 == 0) //congrats Pinkie... every 5 ticks
            {
                if (Main.rand.NextBool(2) && Main.myPlayer == projectile.owner)
                {
					float dmgKBMult = Main.rand.NextFloat(0.32f, 0.56f);
                    int spearAmt = Main.rand.Next(1, 4); //1 to 3 spears
                    for (int n = 0; n < spearAmt; n++)
                    {
                        float xStart = projectile.position.X + Main.rand.Next(-400, 400);
                        float yStart = projectile.position.Y - Main.rand.Next(500, 800);
                        Vector2 startPos = new Vector2(xStart, yStart);
						Vector2 velocity = projectile.Center - startPos;
                        velocity.X += (float)Main.rand.Next(-100, 101);
						float travelDist = velocity.Length();
                        float spearSpeed = 29f;
                        travelDist = spearSpeed / travelDist;
                        velocity.X *= travelDist;
                        velocity.Y *= travelDist;
                        Projectile.NewProjectile(startPos, velocity, ModContent.ProjectileType<EclipsesSmol>(), (int)(projectile.damage * dmgKBMult), projectile.knockBack * dmgKBMult, projectile.owner, 0f, 0f);
                    }
                }
            }
            if (projectile.ai[0] == 0f)
            {
                projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 0.785f;
                if (Main.rand.NextBool(8)) //dust
                {
                    Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 138, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
                }
            }
			if (projectile.ai[0] == 1f && !changedTimeLeft)
			{
				projectile.timeLeft = 900;
				changedTimeLeft = true;
			}
            //Sticky Behaviour
            CalamityUtils.StickyProjAI(projectile, 15);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            CalamityUtils.ModifyHitNPCSticky(projectile, 1, true);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }
    }
}
