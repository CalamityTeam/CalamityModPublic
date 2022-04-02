using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class EndoHydraHead : ModProjectile
    {
        public Vector2 DeltaPosition;
        public Vector2 DeltaPositionMoving;
        public Vector2[] OldVelocities = new Vector2[20];
        public int BodyUUIDIndex => Projectile.GetByUUID(projectile.owner, projectile.ai[0]);
        public float Time
        {
            get => projectile.ai[1];
            set => projectile.ai[1] = value;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hydra Head");
            Main.projFrames[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 20;
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
            projectile.coldDamage = true;
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            DeltaPosition = reader.ReadVector2();
            DeltaPositionMoving = reader.ReadVector2();
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(DeltaPosition);
            writer.WriteVector2(DeltaPositionMoving);
        }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (BodyUUIDIndex < 0 || BodyUUIDIndex >= Main.projectile.Length)
            {
                projectile.Kill();
                return;
            }
            Projectile body = Main.projectile[BodyUUIDIndex];
            if (!body.active)
            {
                projectile.Kill();
                return;
            }

            int totalHeads = CalamityUtils.CountProjectiles(projectile.type);
            if (projectile.localAI[0] == 0f)
            {
                DeltaPosition = DeltaPositionMoving = new Vector2(Main.rand.NextFloat(-72f - 8f * totalHeads, 72f + 8f * totalHeads), -Main.rand.NextFloat(8f, 84f + 4f * totalHeads));
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;

                projectile.netUpdate = true;

                if (!Main.dedServ)
                {
                    for (int i = 0; i < 18; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(projectile.Center, 113);
                        dust.velocity = new Vector2(0f, -5f).RotatedBy(i / 18f * MathHelper.TwoPi);
                        dust.noGravity = true;
                        dust.scale = 1.2f;
                    }
                }

                projectile.localAI[0] = 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = trueDamage;
            }

            Time++;

            if (Time % (60f + totalHeads * 6f) == 59f + totalHeads * 6f)
                DeltaPosition = new Vector2(Main.rand.NextFloat(-72f - 8f * totalHeads, 72f + 8f * totalHeads), -Main.rand.NextFloat(8f, 84f + 4f * totalHeads));
            if (Vector2.Distance(DeltaPosition, DeltaPositionMoving) > 0.2f)
                DeltaPositionMoving = Vector2.Lerp(DeltaPositionMoving, DeltaPosition, 0.125f);

            Vector2 returnPosition = body.Center + new Vector2(body.spriteDirection == 1 ? 12 : -14, -50f) + DeltaPositionMoving;

            // Beam shooting
            if (body.ai[0] >= 0 && body.ai[0] < Main.maxNPCs)
            {
                NPC target = Main.npc[(int)body.ai[0]];
                bool targetAliveAndInLineOfSight = target.active && projectile.Distance(target.Center) < EndoHydraBody.DistanceToCheck;
                if (targetAliveAndInLineOfSight && target.CanBeChasedBy() && Main.myPlayer == projectile.owner)
                {
                    if (Time % 40f == 24f)
                        Projectile.NewProjectile(projectile.Center, projectile.SafeDirectionTo(target.Center) * 6f, ModContent.ProjectileType<EndoRay>(), projectile.damage, projectile.knockBack, projectile.owner);

                    if (Time % 40f >= 33f)
                        projectile.frame = Main.projFrames[projectile.type] - 1;
                    else if (Time % 40f >= 27f)
                        projectile.frame = Main.projFrames[projectile.type] - 2;
                    else if (Time % 40f >= 22f)
                        projectile.frame = Main.projFrames[projectile.type] - 3;
                    else if (Time % 40f >= 17f)
                        projectile.frame = Main.projFrames[projectile.type] - 4;

                    projectile.direction = projectile.spriteDirection = (player.Center.X - projectile.Center.X > 0).ToDirectionInt();
                    if (Math.Abs(player.Center.X - projectile.Center.X) < 80f)
                        projectile.direction = projectile.spriteDirection = player.direction;
                    returnPosition.X -= 48f * (target.Center.X - projectile.Center.X > 0).ToDirectionInt();
                }
                else
                {
                    projectile.direction = projectile.spriteDirection = (player.Center.X - projectile.Center.X > 0).ToDirectionInt();
                    if (Math.Abs(player.Center.X - projectile.Center.X) < 80f)
                        projectile.direction = projectile.spriteDirection = player.direction;
                    projectile.frame = 0;
                }
            }
            else
            {
                projectile.direction = projectile.spriteDirection = player.direction;
                projectile.frame = 0;
            }

            float distanceFromTarget = projectile.Distance(returnPosition);
            if (distanceFromTarget > 7f)
            {
                float speed = MathHelper.Lerp(2f, 17f, Utils.InverseLerp(10f, 70f, distanceFromTarget, true));
                projectile.velocity = (projectile.velocity * 9f + projectile.SafeDirectionTo(returnPosition) * speed) / 10f;
            }

            if (projectile.Center.Y > body.Center.Y - 50f)
                projectile.Center = new Vector2(projectile.Center.X, body.Center.Y - 50f);

            if (projectile.Distance(returnPosition) > 120f)
            {
                projectile.Center = returnPosition + projectile.DirectionFrom(returnPosition) * 120f;
            }

            projectile.MinionAntiClump(0.15f);
            AdjustOldVelocityArray();
        }

        public void AdjustOldVelocityArray()
        {
            for (int i = OldVelocities.Length - 1; i > 0; i--)
            {
                OldVelocities[i] = OldVelocities[i - 1];
            }
            OldVelocities[0] = projectile.velocity;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 67);
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (BodyUUIDIndex < 0 || BodyUUIDIndex >= Main.projectile.Length)
                return false;
            Projectile body = Main.projectile[BodyUUIDIndex];
            if (!body.active)
                return false;

            Texture2D chain = ModContent.GetTexture("CalamityMod/Projectiles/Summon/EndoHydraChain");
            Vector2 start = body.Center + new Vector2(body.spriteDirection == 1 ? 12 : -14, -30f);
            Vector2 end = projectile.Center + (projectile.spriteDirection == 1).ToInt() * 10 * Vector2.UnitX;

            List<Vector2> controlPoints = new List<Vector2>
            {
                start
            };
            for (int i = 0; i < OldVelocities.Length; i++)
            {
                // Incorporate the past movement into neck turns, giving it rubber band-like movment.
                // Become less responsive at the neck ends. Having the ends have typical movement can look strange sometimes.
                float swayResponsiveness = Utils.InverseLerp(0f, 6f, i, true) * Utils.InverseLerp(OldVelocities.Length, OldVelocities.Length - 6f, i, true);
                swayResponsiveness *= 2.5f;
                Vector2 swayTotalOffset = OldVelocities[i] * swayResponsiveness;
                controlPoints.Add(Vector2.Lerp(start, end, i / (float)OldVelocities.Length) + swayTotalOffset);
            }
            controlPoints.Add(end);

            int chainPointCount = (int)(Vector2.Distance(controlPoints.First(), controlPoints.Last()) / 5f);
            if (chainPointCount < 12)
                chainPointCount = 12;
            BezierCurve bezierCurve = new BezierCurve(controlPoints.ToArray());
            List<Vector2> chainPoints = bezierCurve.GetPoints(chainPointCount);

            for (int i = 0; i < chainPoints.Count; i++)
            {
                Vector2 positionAtPoint = chainPoints[i];
                if (Vector2.Distance(positionAtPoint, projectile.Center) < 10f)
                    continue;
                float angleAtPoint = i == chainPoints.Count - 1 ? (end - chainPoints[i]).ToRotation() : (chainPoints[i + 1] - chainPoints[i]).ToRotation();
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

            // PreDraw is used instead of PostDraw because of draw order. Drawing the chains after the head
            // would cause them to be drawn on top of the head, which we do not want.
            Texture2D headTexture = ModContent.GetTexture(Texture);
            spriteBatch.Draw(headTexture,
                             projectile.Center - Main.screenPosition + Vector2.UnitY * projectile.gfxOffY,
                             headTexture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame),
                             lightColor,
                             projectile.rotation,
                             projectile.Size * 0.5f,
                             projectile.scale,
                             projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                             0f);
            return false;
        }
        public override bool CanDamage() => false;
    }
}
