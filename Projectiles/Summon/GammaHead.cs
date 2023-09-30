using CalamityMod.Buffs.Summon;
using CalamityMod.DataStructures;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class GammaHead : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public Vector2 CurrentPositionOffset;
        public Vector2 IdealPositionOffset;
        public Vector2[] OldVelocities = new Vector2[20];
        public Player Owner => Main.player[Projectile.owner];
        public ref float Time => ref Projectile.ai[0];
        public Vector2 DrawStartPosition
        {
            get
            {
                if (Projectile.owner < 0 || Projectile.owner >= Main.player.Length)
                    return Vector2.Zero;
                return Main.player[Projectile.owner].Top + Vector2.UnitY * 8f;
            }
        }
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 36;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
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
            Vector2 returnPosition = DrawStartPosition + CurrentPositionOffset;

            if (Projectile.localAI[0] == 0f)
            {
                PerformInitializationEffects();
                Projectile.localAI[0] = 1f;
            }

            ApplyMinionBuffs();

            Projectile.direction = Projectile.spriteDirection = Owner.direction;

            float idealRotation = 0f;
            NPC potentialTarget = Projectile.Center.MinionHoming(3000f, Owner);
            if (potentialTarget != null)
                NoticeTarget(potentialTarget, ref returnPosition, ref idealRotation);
            else
                Projectile.frame = 0;

            Projectile.rotation = Projectile.rotation.AngleLerp(idealRotation, 0.15f);
            Projectile.rotation = Projectile.rotation.AngleTowards(idealRotation, 0.15f);

            Projectile.Center = new Vector2(Projectile.Center.X, MathHelper.Clamp(Projectile.Center.Y, 1f, returnPosition.Y - 8f));
            MoveTowardsDestination(returnPosition);
            AdjustOldVelocityArray();

            Time++;
        }

        public void PerformInitializationEffects()
        {
            int totalHeads = CalamityUtils.CountProjectiles(Projectile.type);
            CurrentPositionOffset = IdealPositionOffset = new Vector2(Main.rand.NextFloat(-72f - 8f * totalHeads, 72f + 8f * totalHeads), -Main.rand.NextFloat(8f, 84f + 4f * totalHeads));
            Projectile.netUpdate = true;

            if (Main.dedServ)
                return;

            // Release a circle of sulphuric dust.
            for (int i = 0; i < 18; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, (int)CalamityDusts.SulfurousSeaAcid);
                dust.velocity = new Vector2(0f, -5f).RotatedBy(i / 18f * MathHelper.TwoPi);
                dust.noGravity = true;
                dust.scale = 1.35f;
            }
        }

        public void ApplyMinionBuffs()
        {
            Owner.AddBuff(ModContent.BuffType<GammaHydraBuff>(), 3600);
            if (Projectile.type == ModContent.ProjectileType<GammaHead>())
            {
                if (Owner.dead)
                    Owner.Calamity().gammaHead = false;
                if (Owner.Calamity().gammaHead)
                    Projectile.timeLeft = 2;
            }
        }

        public void NoticeTarget(NPC target, ref Vector2 returnPosition, ref float idealRotation)
        {
            // Reel back a bit if a valid target has been located.
            returnPosition -= (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 16f;

            // And release canisters from time to time.
            int canisterShootRate = 100;

            // Open the mouth prior to firing.
            if (Time % canisterShootRate > canisterShootRate - 20)
                Projectile.frame = (int)(Main.projFrames[Projectile.type] * Utils.GetLerpValue(canisterShootRate - 20, canisterShootRate - 4, Time % canisterShootRate, true));
            Projectile.frame %= Main.projFrames[Projectile.type];

            Vector2 spawnPosition = Projectile.Center;
            float shootSpeed = MathHelper.Lerp(8f, 29f, Utils.GetLerpValue(90f, 850f, target.Distance(spawnPosition), true));
            Vector2 shootVelocity = CalamityUtils.GetProjectilePhysicsFiringVelocity(spawnPosition, target.Center, GammaCanister.Gravity, shootSpeed, Projectile.SafeDirectionTo(target.Center));

            float fireAngle = shootVelocity.ToRotation();

            // Direction affecting.
            if (target.Center.X - spawnPosition.X < 0)
                fireAngle += MathHelper.Pi;
            if (Projectile.direction == (target.Center.X < spawnPosition.X).ToDirectionInt())
                fireAngle += MathHelper.Pi;

            idealRotation = fireAngle;

            // Shoot the actual canister.
            if (Main.myPlayer == Projectile.owner && Time % canisterShootRate == canisterShootRate - 1)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPosition, shootVelocity, ModContent.ProjectileType<GammaCanister>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }

        public void MoveTowardsDestination(Vector2 returnPosition)
        {
            float distanceFromTarget = Projectile.Distance(returnPosition);
            if (distanceFromTarget > 7f)
            {
                float flySpeed = MathHelper.Lerp(2f, 17f, Utils.GetLerpValue(10f, 70f, distanceFromTarget, true));
                Projectile.velocity = (Projectile.velocity * 9f + Projectile.SafeDirectionTo(returnPosition) * flySpeed) / 10f;
            }

            int totalHeads = CalamityUtils.CountProjectiles(Projectile.type);
            int moveRate = 40 + totalHeads * 4;

            // Reset the ideal offset from time to time.
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

        public override void OnKill(int timeLeft)
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 10; i++)
                Dust.NewDustPerfect(Projectile.Center, (int)CalamityDusts.SulfurousSeaAcid, Main.rand.NextVector2CircularEdge(4f, 4f)).noGravity = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D chain = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/GammaHeadChain").Value;
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
