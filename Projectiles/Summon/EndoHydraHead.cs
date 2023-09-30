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
    public class EndoHydraHead : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public Vector2 DeltaPosition;
        public Vector2 DeltaPositionMoving;
        public Vector2[] OldVelocities = new Vector2[20];
        public int BodyUUIDIndex => Projectile.GetByUUID(Projectile.owner, Projectile.ai[0]);
        public float Time
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.coldDamage = true;
            Projectile.DamageType = DamageClass.Summon;
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
            Player player = Main.player[Projectile.owner];
            if (BodyUUIDIndex < 0 || BodyUUIDIndex >= Main.projectile.Length)
            {
                Projectile.Kill();
                return;
            }
            Projectile body = Main.projectile[BodyUUIDIndex];
            if (!body.active)
            {
                Projectile.Kill();
                return;
            }

            int totalHeads = CalamityUtils.CountProjectiles(Projectile.type);
            if (Projectile.localAI[0] == 0f)
            {
                DeltaPosition = DeltaPositionMoving = new Vector2(Main.rand.NextFloat(-72f - 8f * totalHeads, 72f + 8f * totalHeads), -Main.rand.NextFloat(8f, 84f + 4f * totalHeads));
                Projectile.netUpdate = true;

                if (!Main.dedServ)
                {
                    for (int i = 0; i < 18; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, 113);
                        dust.velocity = new Vector2(0f, -5f).RotatedBy(i / 18f * MathHelper.TwoPi);
                        dust.noGravity = true;
                        dust.scale = 1.2f;
                    }
                }

                Projectile.localAI[0] = 1f;
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
                bool targetAliveAndInLineOfSight = target.active && Projectile.Distance(target.Center) < EndoHydraBody.DistanceToCheck;
                if (targetAliveAndInLineOfSight && target.CanBeChasedBy() && Main.myPlayer == Projectile.owner)
                {
                    if (Time % 40f == 24f)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDirectionTo(target.Center) * 6f, ModContent.ProjectileType<EndoRay>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }

                    if (Time % 40f >= 33f)
                        Projectile.frame = Main.projFrames[Projectile.type] - 1;
                    else if (Time % 40f >= 27f)
                        Projectile.frame = Main.projFrames[Projectile.type] - 2;
                    else if (Time % 40f >= 22f)
                        Projectile.frame = Main.projFrames[Projectile.type] - 3;
                    else if (Time % 40f >= 17f)
                        Projectile.frame = Main.projFrames[Projectile.type] - 4;

                    Projectile.direction = Projectile.spriteDirection = (player.Center.X - Projectile.Center.X > 0).ToDirectionInt();
                    if (Math.Abs(player.Center.X - Projectile.Center.X) < 80f)
                        Projectile.direction = Projectile.spriteDirection = player.direction;
                    returnPosition.X -= 48f * (target.Center.X - Projectile.Center.X > 0).ToDirectionInt();
                }
                else
                {
                    Projectile.direction = Projectile.spriteDirection = (player.Center.X - Projectile.Center.X > 0).ToDirectionInt();
                    if (Math.Abs(player.Center.X - Projectile.Center.X) < 80f)
                        Projectile.direction = Projectile.spriteDirection = player.direction;
                    Projectile.frame = 0;
                }
            }
            else
            {
                Projectile.direction = Projectile.spriteDirection = player.direction;
                Projectile.frame = 0;
            }

            float distanceFromTarget = Projectile.Distance(returnPosition);
            if (distanceFromTarget > 7f)
            {
                float speed = MathHelper.Lerp(2f, 17f, Utils.GetLerpValue(10f, 70f, distanceFromTarget, true));
                Projectile.velocity = (Projectile.velocity * 9f + Projectile.SafeDirectionTo(returnPosition) * speed) / 10f;
            }

            if (Projectile.Center.Y > body.Center.Y - 50f)
                Projectile.Center = new Vector2(Projectile.Center.X, body.Center.Y - 50f);

            if (Projectile.Distance(returnPosition) > 120f)
            {
                Projectile.Center = returnPosition + Projectile.DirectionFrom(returnPosition) * 120f;
            }

            Projectile.MinionAntiClump(0.15f);
            AdjustOldVelocityArray();
        }

        public void AdjustOldVelocityArray()
        {
            for (int i = OldVelocities.Length - 1; i > 0; i--)
            {
                OldVelocities[i] = OldVelocities[i - 1];
            }
            OldVelocities[0] = Projectile.velocity;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 67);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (BodyUUIDIndex < 0 || BodyUUIDIndex >= Main.projectile.Length)
                return false;
            Projectile body = Main.projectile[BodyUUIDIndex];
            if (!body.active)
                return false;

            Texture2D chain = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/EndoHydraChain").Value;
            Vector2 start = body.Center + new Vector2(body.spriteDirection == 1 ? 12 : -14, -30f);
            Vector2 end = Projectile.Center + (Projectile.spriteDirection == 1).ToInt() * 10 * Vector2.UnitX;

            List<Vector2> controlPoints = new List<Vector2>
            {
                start
            };
            for (int i = 0; i < OldVelocities.Length; i++)
            {
                // Incorporate the past movement into neck turns, giving it rubber band-like movment.
                // Become less responsive at the neck ends. Having the ends have typical movement can look strange sometimes.
                float swayResponsiveness = Utils.GetLerpValue(0f, 6f, i, true) * Utils.GetLerpValue(OldVelocities.Length, OldVelocities.Length - 6f, i, true);
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
                if (Vector2.Distance(positionAtPoint, Projectile.Center) < 10f)
                    continue;
                float angleAtPoint = i == chainPoints.Count - 1 ? (end - chainPoints[i]).ToRotation() : (chainPoints[i + 1] - chainPoints[i]).ToRotation();
                angleAtPoint += MathHelper.PiOver2;
                Main.EntitySpriteDraw(chain,
                                 positionAtPoint - Main.screenPosition,
                                 null,
                                 Color.Lerp(Color.White, Color.Transparent, 0.6f),
                                 angleAtPoint,
                                 chain.Size() / 2f,
                                 1f,
                                 SpriteEffects.None,
                                 0);
            }

            // PreDraw is used instead of PostDraw because of draw order. Drawing the chains after the head
            // would cause them to be drawn on top of the head, which we do not want.
            Texture2D headTexture = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(headTexture,
                             Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY,
                             headTexture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame),
                             lightColor,
                             Projectile.rotation,
                             Projectile.Size * 0.5f,
                             Projectile.scale,
                             Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                             0);
            return false;
        }
        public override bool? CanDamage() => false;
    }
}
