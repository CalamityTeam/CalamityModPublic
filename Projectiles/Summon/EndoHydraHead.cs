using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class EndoHydraHead : ModProjectile
    {
        public Vector2 DeltaPosition;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hydra Head");
            Main.projFrames[projectile.type] = 5;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            DeltaPosition = reader.ReadVector2();
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(DeltaPosition);
        }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (projectile.ai[0] < 0 || projectile.ai[0] >= Main.projectile.Length)
            {
                projectile.Kill();
                return;
            }
            Projectile body = Main.projectile[(int)projectile.ai[0]];
            if (!body.active)
            {
                projectile.Kill();
                return;
            }

            if (projectile.localAI[0] == 0f)
            {
                int totalHeads = CalamityUtils.CountProjectiles(projectile.type);
                DeltaPosition = new Vector2(Main.rand.NextFloat(-72f - 8f * totalHeads, 72f + 8f * totalHeads), -Main.rand.NextFloat(8f, 84f + 4f * totalHeads));
                projectile.netUpdate = true;

                projectile.Calamity().spawnedPlayerMinionDamageValue = (player.allDamage + player.minionDamage - 1f);
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
				projectile.localAI[0] = 1f;
			}
            if ((player.allDamage + player.minionDamage - 1f) != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    (player.allDamage + player.minionDamage - 1f));
                projectile.damage = trueDamage;
            }
            Vector2 bodyTop = body.Center + new Vector2(body.spriteDirection == 1 ? 12 : -14, -50f) + DeltaPosition;

            // Beam shooting
            if (body.ai[0] >= 0 && body.ai[0] < Main.projectile.Length)
            {
                if (Main.npc[(int)body.ai[0]].active &&
                    Main.npc[(int)body.ai[0]].CanBeChasedBy(null, false) &&
                    projectile.Distance(Main.npc[(int)body.ai[0]].Center) < EndoHydraBody.DistanceToCheck &&
                    Collision.CanHit(projectile.Center, 1, 1, Main.npc[(int)body.ai[0]].Center, 1, 1) &&
                    Main.myPlayer == projectile.owner)
                {
                    projectile.ai[1]++;
                    if (projectile.ai[1] % 40f == 24f)
                    {
                        Projectile.NewProjectile(projectile.Center,
                            6f * projectile.DirectionTo(Main.npc[(int)body.ai[0]].Center),
                            ModContent.ProjectileType<EndoRay>(), projectile.damage, 2f, projectile.owner);
                    }

                    if (projectile.ai[1] % 40f >= 33f)
                        projectile.frame = Main.projFrames[projectile.type] - 1;
                    else if (projectile.ai[1] % 40f >= 27f)
                        projectile.frame = Main.projFrames[projectile.type] - 2;
                    else if (projectile.ai[1] % 40f >= 22f)
                        projectile.frame = Main.projFrames[projectile.type] - 3;
                    else if (projectile.ai[1] % 40f >= 17f)
                        projectile.frame = Main.projFrames[projectile.type] - 4;

                    projectile.direction = projectile.spriteDirection = (Main.npc[(int)body.ai[0]].Center.X - projectile.Center.X > 0).ToDirectionInt();
                    bodyTop.X -= 48f * (Main.npc[(int)body.ai[0]].Center.X - projectile.Center.X > 0).ToDirectionInt();
                }
                else
                {
                    projectile.direction = projectile.spriteDirection = (player.Center.X - projectile.Center.X > 0).ToDirectionInt();
                    projectile.frame = 0;
                }
            }
            else
            {
                projectile.direction = projectile.spriteDirection = (player.Center.X - projectile.Center.X > 0).ToDirectionInt();
                projectile.frame = 0;
            }

            if (projectile.Distance(bodyTop) > 28f)
            {
                projectile.velocity += new Vector2(Math.Sign(bodyTop.X - projectile.Center.X), Math.Sign(bodyTop.Y - projectile.Center.Y)) * new Vector2(0.015f, 0.025f) * Main.rand.NextFloat(0.98f, 1.02f);
                if (Math.Abs(projectile.velocity.X) > 2.5f)
                {
                    projectile.velocity.X = 2.5f * Math.Sign(projectile.velocity.X);
                }
                if (Math.Abs(projectile.velocity.Y) > 1.6f)
                {
                    projectile.velocity.Y = 1.6f * Math.Sign(projectile.velocity.Y);
                }
                projectile.velocity += (bodyTop - projectile.Center) / 10f;
            }
            else projectile.velocity *= 0.98f;
            projectile.Center = new Vector2(projectile.Center.X, MathHelper.Clamp(projectile.Center.Y, 1f, bodyTop.Y - 8f));
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 67);
            }
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.ai[0] < 0 || projectile.ai[0] >= Main.projectile.Length)
            {
                return;
            }
            Projectile body = Main.projectile[(int)projectile.ai[0]];
            if (!body.active)
            {
                return;
            }

            Texture2D chain = ModContent.GetTexture("CalamityMod/Projectiles/Summon/EndoHydraChain");
            Vector2 start = projectile.Center + (projectile.spriteDirection == 1).ToInt() * 10 * Vector2.UnitX;
            Vector2 end = body.Center - projectile.DirectionTo(body.Center) * 20f;
            Vector2 bodyTop = body.Center + new Vector2(body.spriteDirection == 1 ? 12 : -12, -32f);

            for (int i = 0; i <= (int)projectile.Distance(end) / 10 + 1; i++)
            {
                float ratio = i / (projectile.Distance(end) / 10 + 1);
                Vector2 positionAtPoint = start + (bodyTop - start) * ratio;
                if (projectile.Distance(positionAtPoint) > 9)
                {
                    float angleAtPoint = (start - positionAtPoint).ToRotation();
                    if (i < (int)projectile.Distance(end) / 10 + 1)
                    {
                        float nextRatio = (i + 1f) / (projectile.Distance(end) / 10 + 1);
                        Vector2 nextPositionAtPoint = start + (bodyTop - start) * nextRatio;

                        angleAtPoint = (nextPositionAtPoint - positionAtPoint).ToRotation();
                    }
                    angleAtPoint += MathHelper.PiOver2;
                    spriteBatch.Draw(chain,
                        positionAtPoint - Main.screenPosition,
                        null,
                        Color.Lerp(Color.White, Color.Transparent, 0.6f),
                        angleAtPoint,
                        chain.Size() / 2f,
                        1f,
                        SpriteEffects.None,
                        0f);
                }
            }
        }
        public override bool CanDamage() => false;
    }
}
