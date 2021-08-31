using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class Cyclone : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/TornadoProj";

        public int dustvortex = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cyclone");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.alpha = 255;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.timeLeft = 300;
            projectile.extraUpdates = 2;
            projectile.penetrate = 2;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            projectile.ai[0]++;
            projectile.ai[1]++;

            //Code so it doesnt collide on tiles instantly
            if (projectile.ai[0] >= 12)
                projectile.tileCollide = true;

            projectile.rotation += 2.5f;
            projectile.alpha -= 5;
            if (projectile.alpha < 50)
            {
                projectile.alpha = 50;
                if (projectile.ai[1] >= 15)
                {

                    for (int i = 1; i <= 6; i++)
                    {
                        Vector2 dustspeed = new Vector2(3f, 3f).RotatedBy(MathHelper.ToRadians(dustvortex));
                        int d = Dust.NewDust(projectile.Center, projectile.width / 2, projectile.height / 2, 31, dustspeed.X, dustspeed.Y, 200, new Color(232, 251, 250, 200), 1.3f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity = dustspeed;
                        dustvortex += 60;
                    }
                    dustvortex -= 355;
                    projectile.ai[1] = 0;
                }
            }
            float num472 = projectile.Center.X;
            float num473 = projectile.Center.Y;
            float num474 = 600f;
            for (int num475 = 0; num475 < 200; num475++)
            {
                NPC npc = Main.npc[num475];
                if (npc.CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1) && !CalamityPlayer.areThereAnyDamnBosses)
                {
                    float npcCenterX = npc.position.X + (float)(npc.width / 2);
                    float npcCenterY = npc.position.Y + (float)(npc.height / 2);
                    float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - npcCenterX) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - npcCenterY);
                    if (num478 < num474)
                    {
                        if (npc.position.X < num472)
                        {
                            npc.velocity.X += 0.05f;
                        }
                        else
                        {
                            npc.velocity.X -= 0.05f;
                        }
                        if (npc.position.Y < num473)
                        {
                            npc.velocity.Y += 0.05f;
                        }
                        else
                        {
                            npc.velocity.Y -= 0.05f;
                        }
                    }
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(204, 255, 255, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 60, 0.6f);

            for (int i = 0; i <= 360; i += 3)
            {
                Vector2 dustspeed = new Vector2(3f, 3f).RotatedBy(MathHelper.ToRadians(i));
                int d = Dust.NewDust(projectile.Center, projectile.width, projectile.height, 31, dustspeed.X, dustspeed.Y, 200, new Color(232, 251, 250, 200), 1.4f);
                Main.dust[d].noGravity = true;
                Main.dust[d].position = projectile.Center;
                Main.dust[d].velocity = dustspeed;
            }
        }
    }
}
