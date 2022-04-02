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
    public class GammaHead : ModProjectile
    {
        public Vector2 CurrentPositionOffset;
        public Vector2 IdealPositionOffset;
        public Vector2[] OldVelocities = new Vector2[20];
        public Player Owner => Main.player[projectile.owner];
        public ref float Time => ref projectile.ai[0];
        public Vector2 DrawStartPosition
        {
            get
            {
                if (projectile.owner < 0 || projectile.owner >= Main.player.Length)
                    return Vector2.Zero;
                return Main.player[projectile.owner].Top + Vector2.UnitY * 8f;
            }
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gamma Head");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 36;
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

            if (projectile.localAI[0] == 0f)
            {
                PerformInitializationEffects();
                projectile.localAI[0] = 1f;
            }

            ApplyMinionBuffs();

            projectile.direction = projectile.spriteDirection = Owner.direction;

            float idealRotation = 0f;
            NPC potentialTarget = projectile.Center.MinionHoming(950f, Owner);
            if (potentialTarget != null)
                NoticeTarget(potentialTarget, ref returnPosition, ref idealRotation);
            else
                projectile.frame = 0;

            projectile.rotation = projectile.rotation.AngleLerp(idealRotation, 0.15f);
            projectile.rotation = projectile.rotation.AngleTowards(idealRotation, 0.15f);

            projectile.Center = new Vector2(projectile.Center.X, MathHelper.Clamp(projectile.Center.Y, 1f, returnPosition.Y - 8f));
            MoveTowardsDestination(returnPosition);
            AdjustOldVelocityArray();

            Time++;
        }

        public void PerformInitializationEffects()
        {
            int totalHeads = CalamityUtils.CountProjectiles(projectile.type);
            CurrentPositionOffset = IdealPositionOffset = new Vector2(Main.rand.NextFloat(-72f - 8f * totalHeads, 72f + 8f * totalHeads), -Main.rand.NextFloat(8f, 84f + 4f * totalHeads));
            projectile.Calamity().spawnedPlayerMinionDamageValue = Owner.MinionDamage();
            projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;

            projectile.netUpdate = true;

            if (Main.dedServ)
                return;

            // Release a circle of sulphuric dust.
            for (int i = 0; i < 18; i++)
            {
                Dust dust = Dust.NewDustPerfect(projectile.Center, (int)CalamityDusts.SulfurousSeaAcid);
                dust.velocity = new Vector2(0f, -5f).RotatedBy(i / 18f * MathHelper.TwoPi);
                dust.noGravity = true;
                dust.scale = 1.35f;
            }
        }

        public void ApplyMinionBuffs()
        {
            Owner.AddBuff(ModContent.BuffType<GammaHeadBuff>(), 3600);
            if (projectile.type == ModContent.ProjectileType<GammaHead>())
            {
                if (Owner.dead)
                    Owner.Calamity().gammaHead = false;
                if (Owner.Calamity().gammaHead)
                    projectile.timeLeft = 2;
            }
        }

        public void NoticeTarget(NPC target, ref Vector2 returnPosition, ref float idealRotation)
        {
            // Reel back a bit if a valid target has been located.
            returnPosition -= (target.Center - projectile.Center).SafeNormalize(Vector2.Zero) * 16f;

            // And release canisters from time to time.
            int canisterShootRate = 100;

            // Open the mouth prior to firing.
            if (Time % canisterShootRate > canisterShootRate - 20)
                projectile.frame = (int)(Main.projFrames[projectile.type] * Utils.InverseLerp(canisterShootRate - 20, canisterShootRate - 4, Time % canisterShootRate, true));
            projectile.frame %= Main.projFrames[projectile.type];

            Vector2 spawnPosition = projectile.Center;
            float shootSpeed = MathHelper.Lerp(8f, 29f, Utils.InverseLerp(90f, 850f, target.Distance(spawnPosition), true));
            Vector2 shootVelocity = CalamityUtils.GetProjectilePhysicsFiringVelocity(spawnPosition, target.Center, GammaCanister.Gravity, shootSpeed, projectile.SafeDirectionTo(target.Center));

            float fireAngle = shootVelocity.ToRotation();

            // Direction affecting.
            if (target.Center.X - spawnPosition.X < 0)
                fireAngle += MathHelper.Pi;
            if (projectile.direction == (target.Center.X < spawnPosition.X).ToDirectionInt())
                fireAngle += MathHelper.Pi;

            idealRotation = fireAngle;

            // Shoot the actual canister.
            if (Main.myPlayer == projectile.owner && Time % canisterShootRate == canisterShootRate - 1)
                Projectile.NewProjectile(spawnPosition, shootVelocity, ModContent.ProjectileType<GammaCanister>(), projectile.damage, projectile.knockBack, projectile.owner);
        }

        public void MoveTowardsDestination(Vector2 returnPosition)
        {
            float distanceFromTarget = projectile.Distance(returnPosition);
            if (distanceFromTarget > 7f)
            {
                float flySpeed = MathHelper.Lerp(2f, 17f, Utils.InverseLerp(10f, 70f, distanceFromTarget, true));
                projectile.velocity = (projectile.velocity * 9f + projectile.SafeDirectionTo(returnPosition) * flySpeed) / 10f;
            }

            int totalHeads = CalamityUtils.CountProjectiles(projectile.type);
            int moveRate = 40 + totalHeads * 4;

            // Reset the ideal offset from time to time.
            if (Time % moveRate == moveRate - 1f)
                IdealPositionOffset = new Vector2(Main.rand.NextFloat(-72f - 8f * totalHeads, 72f + 8f * totalHeads), -Main.rand.NextFloat(8f, 84f + 4f * totalHeads));

            if (Vector2.Distance(IdealPositionOffset, CurrentPositionOffset) > 0.2f)
                CurrentPositionOffset = Vector2.Lerp(CurrentPositionOffset, IdealPositionOffset, 0.125f);

            // Clamp the Y position.
            if (projectile.Center.Y > Owner.Top.Y - 46f)
                projectile.Center = new Vector2(projectile.Center.X, Owner.Top.Y - 46f);

            // Clamp the center so that it does not go too far from the return position.
            if (projectile.Distance(returnPosition) > 120f)
                projectile.Center = returnPosition + projectile.DirectionFrom(returnPosition) * 120f;

            // Disallow heads from getting too close to each other.
            projectile.MinionAntiClump(0.15f);
        }

        public void AdjustOldVelocityArray()
        {
            for (int i = OldVelocities.Length - 1; i > 0; i--)
                OldVelocities[i] = OldVelocities[i - 1];

            OldVelocities[0] = projectile.velocity;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 10; i++)
                Dust.NewDustPerfect(projectile.Center, (int)CalamityDusts.SulfurousSeaAcid, Main.rand.NextVector2CircularEdge(4f, 4f)).noGravity = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D chain = ModContent.GetTexture("CalamityMod/Projectiles/Summon/GammaHeadChain");
            Vector2 end = projectile.Center + (projectile.spriteDirection == 1).ToInt() * 10 * Vector2.UnitX;

            List<Vector2> controlPoints = new List<Vector2>
            {
                DrawStartPosition
            };
            for (int i = 0; i < OldVelocities.Length; i++)
            {
                // Incorporate the past movement into neck turns, giving it rubber band-like movment.
                // Become less responsive at the neck ends. Having the ends have typical movement can look strange sometimes.
                float swayResponsiveness = Utils.InverseLerp(0f, 6f, i, true) * Utils.InverseLerp(OldVelocities.Length, OldVelocities.Length - 6f, i, true);
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
