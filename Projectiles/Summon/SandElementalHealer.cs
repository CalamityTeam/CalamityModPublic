using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Healing;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class SandElementalHealer : ModProjectile
    {
        public int dust = 3;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sand Healer");
            Main.projFrames[Projectile.type] = 6;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 98;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 0f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            if (!modPlayer.sandBoobWaifu && !modPlayer.allWaifus)
            {
                Projectile.active = false;
                return;
            }

            bool correctMinion = Projectile.type == ModContent.ProjectileType<SandElementalHealer>();
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.dWaifu = false;
                }
                if (modPlayer.dWaifu)
                {
                    Projectile.timeLeft = 2;
                }
            }

            dust--;
            if (dust >= 0)
            {
                int dustAmt = 50;
                for (int d = 0; d < dustAmt; d++)
                {
                    int sand = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 16f), Projectile.width, Projectile.height - 16, 32, 0f, 0f, 0, default, 1f);
                    Main.dust[sand].velocity *= 2f;
                    Main.dust[sand].scale *= 1.15f;
                }
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 16)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }

            if (Math.Abs(Projectile.velocity.X) > 0.2f)
            {
                Projectile.spriteDirection = -Projectile.direction;
            }

            float lightScalar = (float)Main.rand.Next(90, 111) * 0.01f;
            lightScalar *= Main.essScale;
            Lighting.AddLight(Projectile.Center, 0.7f * lightScalar, 0.6f * lightScalar, 0f * lightScalar);

            Projectile.MinionAntiClump();

            if (Vector2.Distance(player.Center, Projectile.Center) > 400f)
            {
                Projectile.ai[0] = 1f;
                Projectile.tileCollide = false;
                Projectile.netUpdate = true;
            }

            float safeDist = 100f; //150
            bool returning = false;
            if (!returning)
            {
                returning = Projectile.ai[0] == 1f;
            }
            float returnSpeed = 7f; //6
            if (returning)
            {
                returnSpeed = 18f; //15
            }
            Vector2 playerVec = player.Center - Projectile.Center + new Vector2(-250f, -60f); //-60
            float playerDist = playerVec.Length();
            if (playerDist > 200f && returnSpeed < 10f) //200 and 8
            {
                returnSpeed = 10f; //8
            }
            if (playerDist < safeDist && returning && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.ai[0] = 0f;
                Projectile.netUpdate = true;
            }
            if (playerDist > 2000f)
            {
                Projectile.position.X = player.Center.X - (float)(Projectile.width / 2);
                Projectile.position.Y = player.Center.Y - (float)(Projectile.height / 2);
                Projectile.netUpdate = true;
            }
            if (playerDist > 70f)
            {
                playerVec.Normalize();
                playerVec *= returnSpeed;
                Projectile.velocity = (Projectile.velocity * 40f + playerVec) / 41f;
            }
            else if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
            {
                Projectile.velocity.X = -0.22f;
                Projectile.velocity.Y = -0.12f;
            }

            if (Projectile.ai[1] > 0f)
            {
                Projectile.ai[1] += (float)Main.rand.Next(1, 4);
            }
            if (Projectile.ai[1] > 220f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }
            if (Projectile.localAI[0] < 120f)
            {
                Projectile.localAI[0] += 1f;
            }
            if (Projectile.ai[0] == 0f)
            {
                int healProj = ModContent.ProjectileType<CactusHealOrb>();
                if (Projectile.ai[1] == 0f && Projectile.localAI[0] >= 120f)
                {
                    Projectile.ai[1] += 1f;
                    if (Main.myPlayer == Projectile.owner && player.statLife < player.statLifeMax2)
                    {
                        SoundEngine.PlaySound(SoundID.Dig6, Projectile.position);
                        int dustAmt = 36;
                        for (int d = 0; d < dustAmt; d++)
                        {
                            Vector2 source = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                            source = source.RotatedBy((double)((float)(d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Projectile.Center;
                            Vector2 dustVel = source - Projectile.Center;
                            int green = Dust.NewDust(source + dustVel, 0, 0, 107, dustVel.X * 1.5f, dustVel.Y * 1.5f, 100, new Color(0, 200, 0), 1f);
                            Main.dust[green].noGravity = true;
                            Main.dust[green].noLight = true;
                            Main.dust[green].velocity = dustVel;
                        }
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -Vector2.UnitY * 6f, healProj, 0, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
            }
        }
    }
}
