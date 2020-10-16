using CalamityMod.Buffs.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace CalamityMod.Projectiles.Summon
{
    public class DaedalusGolem : ModProjectile
    {
        public int AttackTimer;
        public bool UsingChargedLaserAttack;
        public const int ChargedPelletAttackTime = 30;
        public const int ChargedLaserAttackTime = 120;
        public const float Gravity = 0.35f;
        public bool Stuck => StuckWalkThroughWallsTimer >= 40f || Collision.SolidCollision(projectile.Center, 2, 2);
        public Player Owner => Main.player[projectile.owner];
        public ref float StuckWalkThroughWallsTimer => ref projectile.ai[0];
        public ref float StuckJumpSpeed => ref projectile.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daedalus Golem");
            Main.projFrames[projectile.type] = 18;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 58;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.minionSlots = 1;
            projectile.timeLeft = 90000;
            projectile.penetrate = -1;
            projectile.minion = true;
            projectile.tileCollide = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(AttackTimer);
            writer.Write(UsingChargedLaserAttack);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
		{
            AttackTimer = reader.ReadInt32();
            UsingChargedLaserAttack = reader.ReadBoolean();
        }

        #region AI
        public override void AI()
        {
            Main.projFrames[projectile.type] = 16;
            bool isCorrectProjectile = projectile.type == ModContent.ProjectileType<DaedalusGolem>();
            Owner.AddBuff(ModContent.BuffType<DaedalusGolemBuff>(), 3600);
            if (isCorrectProjectile)
            {
                if (Owner.dead)
                    Owner.Calamity().daedalusGolem = false;

                if (Owner.Calamity().daedalusGolem)
                    projectile.timeLeft = 2;
            }

            AdjustMinionDamage();

            // Fall down.
            if (projectile.velocity.Y < 15f)
                projectile.velocity.Y += Gravity;

            Vector2 destination;
            NPC pontentialTarget = projectile.Center.MinionHoming(900f, Owner);
            if (pontentialTarget is null)
                destination = Owner.Center + Vector2.UnitX * (80f + (projectile.identity * 28f) % 560f);
			else
                destination = pontentialTarget.Center + Vector2.UnitX * (130f + (projectile.identity * 28f) % 560f);

            // Go upwards, and check down again to discover any height differences before deciding where to move.
            Vector2 upwardCheck = destination - Vector2.UnitY * 2400f;
            upwardCheck.Y = Utils.Clamp(upwardCheck.Y, 0f, Main.maxTilesY * 16f - 100f);
            WorldUtils.Find(upwardCheck.ToTileCoordinates(), Searches.Chain(new Searches.Down(200), new Conditions.IsSolid()), out Point loweredPoint);
            destination = loweredPoint.ToWorldCoordinates();

            StuckWalkThroughWallsTimer = Utils.Clamp(StuckWalkThroughWallsTimer, 0, 160);

            if (projectile.Distance(Owner.Center) > 2500f)
			{
                projectile.Center = Owner.Center;
                StuckWalkThroughWallsTimer = 0;
                projectile.netImportant = true;
            }
            if ((MoveToDestination(destination) || AttackTimer > 0) && pontentialTarget != null)
			{
                AttackTimer++;
                if (AttackTimer == 1)
				{
                    UsingChargedLaserAttack = Main.rand.NextBool(3);
                    projectile.netUpdate = true;
                }

                if (Main.myPlayer == projectile.owner)
				{
                    if (UsingChargedLaserAttack && AttackTimer == ChargedLaserAttackTime / 2)
                    {

                    }
                    else if (UsingChargedLaserAttack && AttackTimer == ChargedPelletAttackTime / 2)
                    {

					}
                }
            }
        }

        public void AdjustMinionDamage()
        {
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = Owner.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] += 1f;
            }
            if (Owner.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int newDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    Owner.MinionDamage());
                projectile.damage = newDamage;
            }
        }

        public bool MoveToDestination(Vector2 destination)
        {
            Tile tileBelow = CalamityUtils.ParanoidTileRetrieval((int)(projectile.Bottom.X / 16), (int)(projectile.Bottom.Y / 16) + 1);

            // Float to the destination if stuck.
            if (Stuck)
            {
                StuckJumpSpeed = 0f;
                projectile.tileCollide = false;

                if (projectile.DistanceSQ(destination - Vector2.UnitY * 16f) > 10f * 10f)
                    projectile.velocity = projectile.DirectionTo(destination - Vector2.UnitY * 16f) * 6f;
                else
                    StuckWalkThroughWallsTimer = 0;

                StuckWalkThroughWallsTimer -= 4;
                return false;
            }

            projectile.tileCollide = true;
            // Don't bother moving any more if super close to the destination. 
            // Just slow down.
            if (Math.Abs(projectile.Center.X - destination.X) < 15 + Math.Abs(projectile.velocity.X))
            {
                StuckJumpSpeed = 0f;
                projectile.velocity.X *= 0.8f;
                return true;
            }

            int currentWalkDirection = Math.Sign(projectile.velocity.X);
            int tilesSearchedAhead = 0;

            Tile tileBelowAhead;

            do
            {
                tileBelowAhead = CalamityUtils.ParanoidTileRetrieval((int)(projectile.Bottom.X / 16) + currentWalkDirection, (int)(projectile.Bottom.Y / 16) + 1);

                if (tileBelowAhead.IsTileSolidGround())
                    break;

                tilesSearchedAhead++;
            }
            while (tilesSearchedAhead < 4);

            int directionToWalk = Math.Sign(destination.X - projectile.Center.X);
            float idealWalkSpeed = 10f * directionToWalk;
            float walkAcceleration = directionToWalk != currentWalkDirection ? 0.325f : 0.2f;
            projectile.velocity.X = MathHelper.Lerp(projectile.velocity.X, idealWalkSpeed, walkAcceleration);

            // Jump if there's a gap or a wall.
            if (tileBelow.IsTileSolidGround() || Collision.SolidCollision(projectile.Center, 10, 10))
            {
                if (Math.Abs(projectile.oldPosition.X - projectile.position.X) < 2f || 
                    Collision.SolidCollision(projectile.Center, 2, 2) || 
                    !Collision.CanHitLine(projectile.position, projectile.width, projectile.height, Owner.position, Owner.width, Owner.height))
                {
                    projectile.velocity.Y = -12f - StuckJumpSpeed;
                    projectile.netSpam -= 10;
                    StuckJumpSpeed += 3.5f;
                    StuckJumpSpeed = Utils.Clamp(StuckJumpSpeed, 0f, 14f);

                    StuckWalkThroughWallsTimer += 10f;

                    projectile.netUpdate = true;
                }
                else if (tilesSearchedAhead > 0)
                {
                    projectile.velocity.X = 7f;

                    projectile.velocity.Y = -(5f + tilesSearchedAhead * 2f);
                    projectile.netSpam -= 10;
                    projectile.netUpdate = true;
                }
                else
                {
                    StuckJumpSpeed = 0f;
                    StuckWalkThroughWallsTimer -= 5f;
                }
            }

            projectile.spriteDirection = (projectile.velocity.X < 0).ToDirectionInt();
            return false;
        }
        #endregion

        // Prevent on-tile collision death.
        public override bool OnTileCollide(Vector2 oldVelocity) => false;

		public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
		{
            int startingWalkFrame = 6;
            int endingWalkFrame = 10;

            int startingPelletFrame = 1;
            int endingPelletFrame = 4;

            int startingBeamFrame = 11;
            int endingBeamFrame = 15;

            // Walking frames. Moves more quickly the faster the walk.
            projectile.frameCounter++;
            Tile tileBelow = CalamityUtils.ParanoidTileRetrieval((int)(projectile.Bottom.X / 16), (int)(projectile.Bottom.Y / 16) + 1);

            if (Stuck)
            {
                projectile.frame = 5;
            }
            else if (AttackTimer > 0)
            {
                if (UsingChargedLaserAttack)
                    projectile.frame = (int)MathHelper.Lerp(startingBeamFrame, endingBeamFrame + 1, Utils.InverseLerp(0f, ChargedLaserAttackTime, AttackTimer, true));
				else
                    projectile.frame = (int)MathHelper.Lerp(startingPelletFrame, endingPelletFrame + 1, Utils.InverseLerp(0f, ChargedPelletAttackTime, AttackTimer, true));
            }
            else if (Math.Abs(projectile.velocity.X) > 1f && tileBelow.IsTileSolidGround())
			{
                if (projectile.frameCounter >= Utils.Clamp(2, 6, (int)Math.Abs(projectile.velocity.X * 0.8)))
				{
                    projectile.frameCounter = 0;
                    projectile.frame++;
                    if (projectile.frame >= endingWalkFrame)
                        projectile.frame = startingWalkFrame;
                }
                projectile.frame = Utils.Clamp(projectile.frame, startingWalkFrame, endingWalkFrame);
			}
            else if (Math.Abs(projectile.velocity.X) <= 1f)
            {
                projectile.frame = 0;
            }
		}

		public override bool CanDamage() => false;
    }
}
