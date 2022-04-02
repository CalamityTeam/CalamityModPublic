using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    class PhantomicShield : ModProjectile
    {
        public const float floatDist = 50f;

        public int deathTimer = 240;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantomic Bulwark");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 56;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 18000;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(deathTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            deathTimer = reader.ReadInt32();
        }

        public override void Kill(int timeLeft)
        {
            for (int d = 0; d < 6; d++)
            {
                int shadow = Dust.NewDust(projectile.position, projectile.width, projectile.height, 27, 0f, 0f, 100, new Color(0, 0, 0), 2f);
                Main.dust[shadow].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[shadow].scale = 0.5f;
                    Main.dust[shadow].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int d = 0; d < 18; d++)
            {
                int shadow = Dust.NewDust(projectile.position, projectile.width, projectile.height, 27, 0f, 0f, 100, new Color(0, 0, 0), 3f);
                Main.dust[shadow].noGravity = true;
                Main.dust[shadow].velocity *= 5f;
                shadow = Dust.NewDust(projectile.position, projectile.width, projectile.height, 27, 0f, 0f, 100, new Color(0, 0, 0), 2f);
                Main.dust[shadow].velocity *= 2f;
            }
        }

        public override void AI()
        {
            Player owner = Main.player[projectile.owner];
            NPC target = CalamityUtils.MinionHoming(owner.position, 1500f, owner);
            if (target != null)
            {
                Vector2 pos1 = owner.position;
                Vector2 pos2 = target.position;
                projectile.ai[0] = (pos2 - pos1).ToRotation();
            } 
            else
            {
                projectile.ai[0] -= MathHelper.ToRadians(2f);
            }
            if (target == null)
            {
                deathTimer--;
            }
            else
            {
                deathTimer = 240;
            }
            if (owner.dead || deathTimer <= 0)
                projectile.Kill();
            projectile.Center = owner.Center + projectile.ai[0].ToRotationVector2() * floatDist;
            projectile.rotation = (owner.Center - projectile.Center).ToRotation();

            projectile.frameCounter++;
            if (projectile.frameCounter > 5)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
                if (projectile.frame % 2 == 0)
                    projectile.netUpdate = true;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }
        }
    }
}
