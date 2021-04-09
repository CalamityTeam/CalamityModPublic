using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class GalaxyBlastType3 : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blast");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.aiStyle = 1;
            projectile.penetrate = 1;
            projectile.extraUpdates = 5;
            projectile.tileCollide = false;
            projectile.melee = true;
        }

        public override void AI()
        {
            if (projectile.ai[1] != -1f && projectile.position.Y > projectile.ai[1])
            {
                projectile.tileCollide = true;
            }
            if (projectile.position.HasNaNs())
            {
                projectile.Kill();
                return;
            }
            bool flag5 = WorldGen.SolidTile(Framing.GetTileSafely((int)projectile.position.X / 16, (int)projectile.position.Y / 16));
            Dust dust6 = Main.dust[Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 57, 0f, 0f, 0, default, 1f)];
            dust6.position = projectile.Center;
            dust6.velocity = Vector2.Zero;
            dust6.noGravity = true;
            if (flag5)
            {
                dust6.noLight = true;
            }
            if (projectile.ai[1] == -1f)
            {
                projectile.ai[0] += 1f;
                projectile.velocity = Vector2.Zero;
                projectile.tileCollide = false;
                projectile.penetrate = -1;
                projectile.position = projectile.Center;
                projectile.width = projectile.height = 140;
                projectile.Center = projectile.position;
                projectile.alpha -= 10;
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                }
                if (++projectile.frameCounter >= projectile.MaxUpdates * 3)
                {
                    projectile.frameCounter = 0;
                    projectile.frame++;
                }
                if (projectile.ai[0] >= (float)(Main.projFrames[projectile.type] * projectile.MaxUpdates * 3))
                {
                    projectile.Kill();
                }
                return;
            }
            projectile.alpha = 255;
            if (projectile.numUpdates == 0)
            {
                int num158 = -1;
                float num159 = 60f;
                for (int num160 = 0; num160 < 200; num160++)
                {
                    NPC nPC = Main.npc[num160];
                    if (nPC.CanBeChasedBy(projectile, false))
                    {
                        float num161 = projectile.Distance(nPC.Center);
                        if (num161 < num159 && Collision.CanHitLine(projectile.Center, 0, 0, nPC.Center, 0, 0))
                        {
                            num159 = num161;
                            num158 = num160;
                        }
                    }
                }
                if (num158 != -1)
                {
                    projectile.ai[0] = 0f;
                    projectile.ai[1] = -1f;
                    projectile.netUpdate = true;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.position);
            bool flag = WorldGen.SolidTile(Framing.GetTileSafely((int)projectile.position.X / 16, (int)projectile.position.Y / 16));
            for (int m = 0; m < 4; m++)
            {
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 57, 0f, 0f, 100, default, 1.5f);
            }
            for (int n = 0; n < 4; n++)
            {
                int num10 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 57, 0f, 0f, 0, default, 2.5f);
                Main.dust[num10].noGravity = true;
                Main.dust[num10].velocity *= 3f;
                if (flag)
                {
                    Main.dust[num10].noLight = true;
                }
                num10 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 57, 0f, 0f, 100, default, 1.5f);
                Main.dust[num10].velocity *= 2f;
                Main.dust[num10].noGravity = true;
                if (flag)
                {
                    Main.dust[num10].noLight = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 180);
            projectile.Kill();
        }
    }
}
