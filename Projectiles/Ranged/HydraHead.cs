using CalamityMod.DataStructures;
using CalamityMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class HydraHead : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Items/Weapons/Ranged/Hydra";

        public Vector2 CurrentPositionOffset;
        public Vector2 IdealPositionOffset;
        public Vector2[] OldVelocities = new Vector2[20];
        public Player Owner => Main.player[Projectile.owner];

        public ref float Time => ref Projectile.ai[0];
        public ref float AttackType => ref Projectile.ai[1];

        public Vector2 DrawStartPosition
        {
            get
            {
                if (Projectile.owner < 0 || Projectile.owner >= Main.player.Length)
                    return Vector2.Zero;
                return Main.player[Projectile.owner].Top + Vector2.UnitY * 8f;
            }
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30; //66x30 sprite
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ContinuouslyUpdateDamageStats = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WritePackedVector2(CurrentPositionOffset);
            writer.WritePackedVector2(IdealPositionOffset);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            CurrentPositionOffset = reader.ReadPackedVector2();
            IdealPositionOffset = reader.ReadPackedVector2();
        }

        public override void AI()
        {
            if (Owner.dead || !Owner.active || Owner.ActiveItem().type != ModContent.ItemType<Hydra>())
                Projectile.Kill();
            else
                Projectile.timeLeft = 2; //Infinite lifespan

            Vector2 returnPosition = DrawStartPosition + CurrentPositionOffset;

            if (Projectile.localAI[0] == 0f)
            {
                PerformInitializationEffects();
                Projectile.localAI[0] = 1f;
            }

            Vector2 aimDestination = Owner.Calamity().mouseWorld;
            float idealRotation = Projectile.AngleTo(aimDestination);
            if (Time < 0f)
                Projectile.rotation = MathHelper.Lerp(Projectile.rotation, idealRotation, (Time + 15f) / 15f);
            else
                Projectile.rotation = idealRotation;

            Projectile.Center = new Vector2(Projectile.Center.X, MathHelper.Clamp(Projectile.Center.Y, 1f, returnPosition.Y - 8f));
            MoveTowardsDestination(returnPosition);
            AdjustOldVelocityArray();

            PerformAttacks(aimDestination);
        }

        public void PerformInitializationEffects()
        {
            int totalHeads = Owner.ownedProjectileCounts[Projectile.type];
            CurrentPositionOffset = IdealPositionOffset = new Vector2(Main.rand.NextFloat(-72f - 8f * totalHeads, 72f + 8f * totalHeads), -Main.rand.NextFloat(8f, 84f + 4f * totalHeads));
            Projectile.netUpdate = true;
        }

        public void MoveTowardsDestination(Vector2 returnPosition)
        {
            float distanceFromTarget = Projectile.Distance(returnPosition);
            if (distanceFromTarget > 7f)
            {
                float flySpeed = MathHelper.Lerp(2f, 17f, Utils.GetLerpValue(10f, 70f, distanceFromTarget, true));
                Projectile.velocity = (Projectile.velocity * 9f + Projectile.SafeDirectionTo(returnPosition) * flySpeed) / 10f;
            }

            int totalHeads = Owner.ownedProjectileCounts[Projectile.type];
            int moveRate = 40 + totalHeads * 4;

            // Reset the ideal offset from time to time.
            Time++;
            if (Time % moveRate == moveRate - 1f)
                IdealPositionOffset = new Vector2(Main.rand.NextFloat(-72f - 8f * totalHeads, 72f + 8f * totalHeads), -Main.rand.NextFloat(8f, 84f + 4f * totalHeads));

            if (Vector2.Distance(IdealPositionOffset, CurrentPositionOffset) > 0.2f)
                CurrentPositionOffset = Vector2.Lerp(CurrentPositionOffset, IdealPositionOffset, 0.125f);

            // Clamp the Y position.
            if (Projectile.Center.Y > Owner.Top.Y - 46f)
                Projectile.Center = new Vector2(Projectile.Center.X, Owner.Top.Y - 46f);

            // Clamp the center so that it does not go too far from the return position.
            if (Projectile.Distance(returnPosition) > 120f)
                Projectile.Center = returnPosition + Projectile.DirectionFrom(returnPosition) * 120f;

            // Disallow heads from getting too close to each other.
            Projectile.MinionAntiClump(0.15f);
        }

        public void AdjustOldVelocityArray()
        {
            for (int i = OldVelocities.Length - 1; i > 0; i--)
                OldVelocities[i] = OldVelocities[i - 1];

            OldVelocities[0] = Projectile.velocity;
        }

        public void PerformAttacks(Vector2 aimDestination)
        {
            Item heldItem = Owner.ActiveItem();
            
            Vector2 shootDirection = Projectile.SafeDirectionTo(aimDestination);

            //Normal shot
            if (AttackType > 0f)
            {
                //Calculation for damage and co
                Owner.PickAmmo(heldItem, out _, out float itemVelocity, out int itemDamage, out float itemKB, out _);
                int type = ModContent.ProjectileType<HydrasBlood>();
                for (int i = 0; i < 3; i++)
                {
                    Vector2 spreadDirection = shootDirection.RotatedByRandom(MathHelper.ToRadians(Hydra.ShotSpread / 2f));
                    float spreadVelocity = itemVelocity * Main.rand.NextFloat(1f, 1.4f);
                    Vector2 shootPos = Projectile.Center + shootDirection * 24f;

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), shootPos, spreadVelocity * spreadDirection, type, itemDamage, itemKB, Projectile.owner);
                }

                //Minor recoil
                Projectile.rotation -= MathHelper.ToRadians(25f) * Math.Sign(Projectile.rotation);
                Time = -15f; //Reset the movement shift and let it recoil normally
                Projectile.velocity += shootDirection * -10f;

                //Reset the attack
                AttackType = 0f;
            }
            //Yeet
            else if (AttackType < 0f)
            {
                //Calculation for damage and co
                Owner.PickAmmo(heldItem, out _, out float itemVelocity, out int itemDamage, out float itemKB, out _);
                int gunType = ModContent.ProjectileType<HydraHeadLaunch>();
                int gunDamage = itemDamage * 5;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, itemVelocity * shootDirection, gunType, gunDamage, itemKB, Projectile.owner);

                Projectile.Kill();
            }
        }

        public override void Kill(int timeLeft)
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 10; i++)
                Dust.NewDustPerfect(Projectile.Center, 171, Main.rand.NextVector2CircularEdge(4f, 4f)).noGravity = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D chain = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/HydraHeadChain").Value;
            Vector2 end = Projectile.Center + (Projectile.spriteDirection == 1).ToInt() * 10 * Vector2.UnitX;

            List<Vector2> controlPoints = new List<Vector2>
            {
                DrawStartPosition
            };
            for (int i = 0; i < OldVelocities.Length; i++)
            {
                // Incorporate the past movement into neck turns, giving it rubber band-like movment.
                // Become less responsive at the neck ends. Having the ends have typical movement can look strange sometimes.
                float swayResponsiveness = Utils.GetLerpValue(0f, 6f, i, true) * Utils.GetLerpValue(OldVelocities.Length, OldVelocities.Length - 6f, i, true);
                Vector2 swayTotalOffset = OldVelocities[i] * swayResponsiveness;
                controlPoints.Add(Vector2.Lerp(DrawStartPosition, end, i / (float)OldVelocities.Length) + swayTotalOffset);
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

            bool shouldFlip = Math.Abs(Projectile.rotation) > MathHelper.PiOver2;
            Texture2D headTexture = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(headTexture,
                             Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY,
                             null,
                             lightColor,
                             Projectile.rotation,
                             Projectile.Size * 0.5f,
                             Projectile.scale,
                             shouldFlip ? SpriteEffects.FlipVertically : SpriteEffects.None,
                             0);

            return false;
        }

        public override bool? CanDamage() => false;
    }
}