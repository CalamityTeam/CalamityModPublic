using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class MiniGuardianHealer : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Healer Guardian");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.width = 68;
            Projectile.height = 82;
            Projectile.friendly = true;
            Projectile.minionSlots = 0f;
            Projectile.minion = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18000;
            Projectile.timeLeft *= 5;
        }

        public override bool? CanCutTiles()
        {
            CalamityPlayer modPlayer = Main.player[Projectile.owner].Calamity();
            bool psa = modPlayer.pArtifact && !modPlayer.profanedCrystal;
            if (!psa && !modPlayer.profanedCrystalBuffs)
                return false;
            return null;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (player.dead)
            {
                modPlayer.gHealer = false;
            }
            if (modPlayer.gHealer)
            {
                Projectile.timeLeft = 2;
            }
            if (!modPlayer.pArtifact || player.dead)
            {
                Projectile.active = false;
                return;
            }
            Projectile.MinionAntiClump();
            Player projOwner = Main.player[Projectile.owner];
            float num16 = 0.5f;
            Projectile.tileCollide = false;
            int num17 = 100;
            Vector2 vector3 = Projectile.Center;
            float num18 = projOwner.Center.X - vector3.X;
            float num19 = projOwner.Center.Y - vector3.Y;
            num19 += (float)Main.rand.Next(-10, 21);
            num18 += (float)Main.rand.Next(-10, 21);
            num18 += (float)(60 * -(float)projOwner.direction);
            num19 -= 60f;
            float num20 = (float)Math.Sqrt((double)(num18 * num18 + num19 * num19));
            float num21 = 18f;

            if (num20 < (float)num17 && projOwner.velocity.Y == 0f &&
                Projectile.position.Y + (float)Projectile.height <= projOwner.position.Y + (float)projOwner.height &&
                !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.ai[0] = 0f;
                if (Projectile.velocity.Y < -6f)
                {
                    Projectile.velocity.Y = -6f;
                }
            }
            if (num20 > 2000f)
            {
                Projectile.position = projOwner.position;
                Projectile.netUpdate = true;
            }
            if (num20 < 50f)
            {
                if (Math.Abs(Projectile.velocity.X) > 2f || Math.Abs(Projectile.velocity.Y) > 2f)
                {
                    Projectile.velocity *= 0.90f;
                }
                num16 = 0.01f;
            }
            else
            {
                if (num20 < 100f)
                {
                    num16 = 0.1f;
                }
                if (num20 > 300f)
                {
                    num16 = 1f;
                }
                num20 = num21 / num20;
                num18 *= num20;
                num19 *= num20;
            }

            if (Projectile.velocity.X < num18)
            {
                Projectile.velocity.X += num16;
                if (num16 > 0.05f && Projectile.velocity.X < 0f)
                {
                    Projectile.velocity.X += num16;
                }
            }
            if (Projectile.velocity.X > num18)
            {
                Projectile.velocity.X -= num16;
                if (num16 > 0.05f && Projectile.velocity.X > 0f)
                {
                    Projectile.velocity.X -= num16;
                }
            }
            if (Projectile.velocity.Y < num19)
            {
                Projectile.velocity.Y += num16;
                if (num16 > 0.05f && Projectile.velocity.Y < 0f)
                {
                    Projectile.velocity.Y += num16 * 2f;
                }
            }
            if (Projectile.velocity.Y > num19)
            {
                Projectile.velocity.Y -= num16;
                if (num16 > 0.05f && Projectile.velocity.Y > 0f)
                {
                    Projectile.velocity.Y -= num16 * 2f;
                }
            }
            if (Projectile.velocity.X > 0.25f)
            {
                Projectile.direction = -1;
            }
            else if (Projectile.velocity.X < -0.25f)
            {
                Projectile.direction = 1;
            }

            if (Math.Abs(Projectile.velocity.X) > 0.2f)
            {
                Projectile.spriteDirection = -Projectile.direction;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 5)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
        }
    }
}
