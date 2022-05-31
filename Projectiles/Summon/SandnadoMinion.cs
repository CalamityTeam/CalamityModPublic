using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class SandnadoMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sandnado");
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 40;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (Projectile.localAI[1] == 0f)
            {
                int dustAmt = 36;
                for (int d = 0; d < dustAmt; d++)
                {
                    Vector2 source = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                    source = source.RotatedBy((double)((float)(d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Projectile.Center;
                    Vector2 dustVel = source - Projectile.Center;
                    int sand = Dust.NewDust(source + dustVel, 0, 0, 85, dustVel.X * 1.5f, dustVel.Y * 1.5f, 100, default, 1.4f);
                    Main.dust[sand].noGravity = true;
                    Main.dust[sand].noLight = true;
                    Main.dust[sand].velocity = dustVel;
                }
                Projectile.localAI[1] += 1f;
            }
            bool correctMinion = Projectile.type == ModContent.ProjectileType<SandnadoMinion>();
            player.AddBuff(ModContent.BuffType<Sandnado>(), 3600);
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.sandnado = false;
                }
                if (modPlayer.sandnado)
                {
                    Projectile.timeLeft = 2;
                }
            }
            Projectile.MinionAntiClump(0.1f);
            if (Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.alpha += 20;
                if (Projectile.alpha > 150)
                {
                    Projectile.alpha = 150;
                }
            }
            else
            {
                Projectile.alpha -= 50;
                if (Projectile.alpha < 60)
                {
                    Projectile.alpha = 60;
                }
            }
            Vector2 targetPos = Projectile.position;
            float range = 1800f;
            bool foundTarget = false;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float npcDist = Vector2.Distance(npc.Center, Projectile.Center);
                    if ((!foundTarget && npcDist < range) && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                    {
                        range = npcDist;
                        targetPos = npc.Center;
                        foundTarget = true;
                        int num11 = npc.whoAmI;
                    }
                }
            }
            if (!foundTarget)
            {
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC npc = Main.npc[k];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float npcDist = Vector2.Distance(npc.Center, Projectile.Center);
                        if ((!foundTarget && npcDist < range) && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                        {
                            range = npcDist;
                            targetPos = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }
            float sepAnxietyDist = 1200f;
            if (foundTarget)
            {
                sepAnxietyDist = 3000f;
            }
            if (Vector2.Distance(player.Center, Projectile.Center) > sepAnxietyDist)
            {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }
            if (foundTarget && Projectile.ai[0] == 0f)
            {
                Vector2 targetDir = targetPos - Projectile.Center;
                float targetDist = targetDir.Length();
                targetDir.Normalize();
                if (targetDist > 400f)
                {
                    float speedMult = 6f;
                    targetDir *= speedMult;
                    Projectile.velocity = (Projectile.velocity * 20f + targetDir) / 21f;
                }
                else
                {
                    Projectile.velocity *= 0.96f;
                }
                if (targetDist > 200f)
                {
                    float speedMult = 12f;
                    targetDir *= speedMult;
                    Projectile.velocity = (Projectile.velocity * 40f + targetDir) / 41f;
                }
                if (Projectile.velocity.Y > -1f)
                {
                    Projectile.velocity.Y -= 0.1f;
                }
            }
            else
            {
                if (!Collision.CanHitLine(Projectile.Center, 1, 1, player.Center, 1, 1))
                {
                    Projectile.ai[0] = 1f;
                }
                float returnSpeed = 12f;
                Vector2 playerVec = player.Center - Projectile.Center + new Vector2(0f, -20f);
                float playerDist = playerVec.Length();
                if (playerDist > 200f && returnSpeed < 12f)
                {
                    returnSpeed = 12f;
                }
                if (playerDist < 100f && Projectile.ai[0] == 1f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                if (playerDist > 2000f)
                {
                    Projectile.position = player.position;
                    Projectile.netUpdate = true;
                }
                if (Math.Abs(playerVec.X) > 40f || Math.Abs(playerVec.Y) > 10f)
                {
                    playerVec.Normalize();
                    playerVec *= returnSpeed;
                    playerVec *= new Vector2(1.25f, 0.65f);
                    Projectile.velocity = (Projectile.velocity * 20f + playerVec) / 21f;
                }
                else
                {
                    if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
                    {
                        Projectile.velocity.X = -0.15f;
                        Projectile.velocity.Y = -0.05f;
                    }
                    Projectile.velocity *= 1.01f;
                }
            }
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            Projectile.frameCounter++;
            int two = 2;
            if (Projectile.frameCounter >= 6 * two)
            {
                Projectile.frameCounter = 0;
            }
            Projectile.frame = Projectile.frameCounter / two;
            if (Main.rand.NextBool(5))
            {
                int sand = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 85, 0f, 0f, 100, default, 2f);
                Main.dust[sand].velocity *= 0.3f;
                Main.dust[sand].noGravity = true;
                Main.dust[sand].noLight = true;
            }
            if (Projectile.velocity.X > 0f)
            {
                Projectile.spriteDirection = Projectile.direction = -1;
            }
            else if (Projectile.velocity.X < 0f)
            {
                Projectile.spriteDirection = Projectile.direction = 1;
            }
            if (Projectile.ai[1] > 0f)
            {
                Projectile.ai[1] += 1f;
                if (Main.rand.Next(3) != 0)
                {
                    Projectile.ai[1] += 1f;
                }
            }
            if (Projectile.ai[1] > 75f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }
            if (Projectile.ai[0] == 0f)
            {
                float speed = 14f;
                int projType = ModContent.ProjectileType<MiniSandShark>();
                if (foundTarget)
                {
                    if (!Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                    {
                        if (Projectile.ai[1] == 0f)
                        {
                            Projectile.ai[1] += 1f;
                            if (Main.myPlayer == Projectile.owner)
                            {
                                Vector2 velocity = targetPos - Projectile.Center;
                                velocity.Normalize();
                                velocity *= speed;
                                int shark = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, projType, Projectile.damage, Projectile.knockBack, Projectile.owner);
                                Main.projectile[shark].originalDamage = Projectile.originalDamage;
                                Main.projectile[shark].netUpdate = true;
                                Projectile.netUpdate = true;
                            }
                        }
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frame = frameHeight * Projectile.frame;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frame, texture.Width, frameHeight)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture.Width / 2f, (float)frameHeight / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool? CanDamage() => false;
    }
}
