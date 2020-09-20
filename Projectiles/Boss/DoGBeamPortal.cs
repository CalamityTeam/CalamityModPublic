using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class DoGBeamPortal : ModProjectile
    {
        private int beamTimer = 180;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam Portal");
            Main.projFrames[projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            projectile.width = 80;
            projectile.height = 80;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 600;
            projectile.penetrate = -1;
            cooldownSlot = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(beamTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            beamTimer = reader.ReadInt32();
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, 0.95f, 1.15f);
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 5)
            {
                projectile.frame = 0;
            }
            beamTimer--;
            if (beamTimer <= 0)
            {
                Main.PlaySound(SoundID.Item33, (int)projectile.position.X, (int)projectile.position.Y);
				if (projectile.owner == Main.myPlayer)
				{
					float speed = 3f;
					if (CalamityWorld.death)
					{
						speed = 5f;
					}
					else if (CalamityWorld.revenge)
					{
						speed = 4.5f;
					}
					else if (Main.expertMode)
					{
						speed = 4f;
					}
					speed *= projectile.ai[0];
					int totalProjectiles = 8;
					float radians = MathHelper.TwoPi / totalProjectiles;
					for (int i = 0; i < totalProjectiles; i++)
					{
						Vector2 vector255 = new Vector2(0f, -speed).RotatedBy(radians * i);
						Projectile.NewProjectile(projectile.Center, vector255, ModContent.ProjectileType<DoGBeam>(), 0, 0f, projectile.owner, projectile.damage, 0f);
					}
				}
                beamTimer = 180;
            }
            int num103 = Player.FindClosest(projectile.Center, 1, 1);
            float scaleFactor2 = projectile.velocity.Length();
            Vector2 vector11 = Main.player[num103].Center - projectile.Center;
            if (Vector2.Distance(Main.player[num103].Center, projectile.Center) > 2000f)
            {
                projectile.position.X = Main.player[num103].Center.X / 16 * 16f - (projectile.width / 2);
                projectile.position.Y = Main.player[num103].Center.Y / 16 * 16f - (projectile.height / 2) - 250f;
                projectile.ai[1] = 0f;
                beamTimer = 90;
            }
            vector11.Normalize();
            vector11 *= scaleFactor2;
            projectile.velocity = (projectile.velocity * 24f + vector11) / 25f;
            projectile.velocity.Normalize();
            projectile.velocity *= scaleFactor2;
            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                Main.PlaySound(SoundID.Item92, (int)projectile.Center.X, (int)projectile.Center.Y);
                for (int num621 = 0; num621 < 30; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 173, 0f, 0f, 100, default, 1.2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 60; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 173, 0f, 0f, 100, default, 1.7f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 173, 0f, 0f, 100, default, 1f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
			if (projectile.timeLeft < 85)
				projectile.damage = 0;
        }

        public override bool CanHitPlayer(Player target)
		{
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft < 85)
            {
                byte b2 = (byte)(projectile.timeLeft * 3);
                byte a2 = (byte)(100f * (b2 / 255f));
                return new Color(b2, b2, b2, a2);
            }
            return new Color(255, 255, 255, 100);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)	
        {
			target.Calamity().lastProjectileHit = projectile;
		}
    }
}
