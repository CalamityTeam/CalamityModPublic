using CalamityMod.Buffs.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;
namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class StarSwallowerSummon : ModProjectile
    {
        public bool FlyUntilNearPlayer;
        public float HopCooldown = 0f;
        public float AcidShootTimer = 0f;
        public float AcidShootCooldown = 0f;
        public float FallThroughYPoint = 0f;

        public bool ReleasingAcid
        {
            get => projectile.ai[0] != 0f;
            set => projectile.ai[0] = value.ToInt();
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pet Froge");
            Main.projFrames[projectile.type] = 12;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 30;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.minion = true;
            projectile.timeLeft *= 5;
            projectile.penetrate = -1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(HopCooldown);
            writer.Write(FlyUntilNearPlayer);
            writer.Write(AcidShootTimer);
            writer.Write(AcidShootCooldown);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            HopCooldown = reader.ReadSingle();
            FlyUntilNearPlayer = reader.ReadBoolean();
            AcidShootTimer = reader.ReadSingle();
            AcidShootCooldown = reader.ReadSingle();
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            bool isCorrectProjectile = projectile.type == ModContent.ProjectileType<StarSwallowerSummon>();
            player.AddBuff(ModContent.BuffType<StarSwallowerBuff>(), 3600);
            if (isCorrectProjectile)
            {
                if (player.dead)
                {
                    player.Calamity().starSwallowerPetFroge = false;
                }
                if (player.Calamity().starSwallowerPetFroge)
                {
                    projectile.timeLeft = 2;
                }
            }

            int minimumFrame = 0;
            int maximumFrame = 11;

            if (projectile.velocity == Vector2.Zero)
            {
                minimumFrame = maximumFrame = 0;
            }

            if (HopCooldown > 0)
            {
                if (projectile.velocity.Length() > 8f)
                {
                    minimumFrame = maximumFrame = 1;
                    if (Math.Abs(projectile.oldPosition.Y - projectile.position.Y) < 5.5f)
                        minimumFrame = maximumFrame = 0;
                }
                else if (HopCooldown < 6)
                {
                    minimumFrame = maximumFrame = 2;
                }
                else if (HopCooldown < 12)
                {
                    minimumFrame = maximumFrame = 3;
                }
                else if (HopCooldown < 21)
                {
                    minimumFrame = maximumFrame = 4;
                }
                else
                {
                    minimumFrame = maximumFrame = 5;
                }
            }

            AdjustDamage(player);

            // Gravity effect.
            projectile.velocity.Y = MathHelper.Clamp(projectile.velocity.Y + 0.4f, -11f, 11f);

            projectile.tileCollide = !FlyUntilNearPlayer;

            ReleasingAcid = false;

            // Remember, && doesn't evaluate future statements if the statement it just evaluated is true.
            // This means that null problems should never happen, since it wouldn't pass the potentialTarget != null check.
            NPC potentialTarget = projectile.Center.MinionHoming(800f, player);
            if (potentialTarget != null &&
                Math.Abs(potentialTarget.Center.X - projectile.Center.X) < 320f &&
                Math.Sign(potentialTarget.Center.Y - projectile.Center.Y) == -1 &&
                Math.Abs(potentialTarget.Center.Y - projectile.Center.Y) < 160f &&
                Math.Sign(potentialTarget.Center.Y - projectile.Center.Y) == -1 &&
                !FlyUntilNearPlayer)
            {
                FallThroughYPoint = potentialTarget.Top.Y;
                NPCTargetingAI(potentialTarget);
            }
            else
            {
                PlayerSeparationAnxietyAI(player, ref minimumFrame, ref maximumFrame);
                FallThroughYPoint = player.Top.Y;
            }

            projectile.MinionAntiClump();
            ManipulateFrames(minimumFrame, maximumFrame);
        }

        public void AdjustDamage(Player player)
        {
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] += 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int newDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = newDamage;
            }
        }

        public void HandleHop(Vector2 targetPosition)
        {
            if (HopCooldown <= 0f && projectile.oldPosition.Y == projectile.position.Y)
            {
                float hopXMult = Utils.InverseLerp(50f, 500f, targetPosition.X - projectile.Center.X, true);
                float hopYMult = Utils.InverseLerp(40f, 300f, targetPosition.Y - projectile.Center.Y, true);
                float hopSpeedX = MathHelper.Lerp(4f, 17f, hopXMult) * Math.Sign(targetPosition.X - projectile.Center.X);
                float hopSpeedY = -1 * MathHelper.Lerp(8f, 23f, hopYMult);

                Vector2 velocity = new Vector2(hopSpeedX, hopSpeedY);

                projectile.velocity = velocity;
                projectile.spriteDirection = (projectile.velocity.X > 0).ToDirectionInt();
                HopCooldown = 25f;
            }
            else
            {
                projectile.velocity.X *= 0.935f;
                HopCooldown--;
            }
        }

        public void PlayerSeparationAnxietyAI(Player player, ref int minimumFrame, ref int maximumFrame)
        {
            // If the player is extremely far away, don't bother homing. Just go to the player's location.
            if (projectile.Distance(player.Center) > 1900f)
            {
                projectile.Center = player.Center;
                projectile.netUpdate = true;
                return;
            }
            Vector2 returnPosition = projectile.Center;
            returnPosition.Y -= 200 * 16;
            returnPosition.Y = MathHelper.Clamp(returnPosition.Y, 50, Main.maxTilesX * 16 - 50);

            bool noGround = !WorldUtils.Find(returnPosition.ToTileCoordinates(), Searches.Chain(new Searches.Down(400), new GenCondition[]
            {
                    new Conditions.IsSolid()
            }), out _);

            if (projectile.Distance(player.Center) > 1200f)
            {
                FlyUntilNearPlayer = true;
                projectile.netUpdate = true;
            }
            if (projectile.Distance(player.Center) < 120f &&
                FlyUntilNearPlayer &&
                !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                FlyUntilNearPlayer = false;
                projectile.netUpdate = true;
            }

            bool flying = noGround || FlyUntilNearPlayer;

            // Fly to player.
            if (flying)
            {
                minimumFrame = 7;
                maximumFrame = 11;
                float inertia = 30f;
                projectile.velocity = (projectile.velocity * (inertia - 1) + projectile.SafeDirectionTo(player.Center) * 18f) / inertia;
            }
            // Otherwise hop around when on the ground.
            else if (projectile.oldPosition.Y == projectile.position.Y)
            {
                HandleHop(player.Center);
            }
        }

        public void NPCTargetingAI(NPC potentialTarget)
        {
            HandleHop(potentialTarget.Center);
            if (Collision.CanHit(projectile.position, projectile.width, projectile.height, potentialTarget.position, potentialTarget.width, projectile.height) && AcidShootCooldown <= 0f)
            {
                AcidShootTimer++;
                ReleasingAcid = AcidShootTimer >= 16 && AcidShootTimer <= 44;

                if (AcidShootTimer >= 20 &&
                    AcidShootTimer <= 44 &&
                    AcidShootTimer % 6 == 0 &&
                    Main.myPlayer == projectile.owner)
                {
                    Vector2 spawnPosition = projectile.Center + Vector2.UnitX * 8f * projectile.spriteDirection;
                    projectile.spriteDirection = (potentialTarget.Center.X - projectile.Center.X > 0).ToDirectionInt();
                    Vector2 fireVelocity = CalamityUtils.GetProjectilePhysicsFiringVelocity(spawnPosition, potentialTarget.Center, StarSwallowerAcid.Gravity, 14f);

                    Projectile.NewProjectile(spawnPosition,
                                             fireVelocity,
                                             ModContent.ProjectileType<StarSwallowerAcid>(),
                                             projectile.damage,
                                             projectile.knockBack,
                                             projectile.owner);
                    Main.PlaySound(SoundID.Item13, spawnPosition);
                }
                if (AcidShootTimer > 48)
                {
                    AcidShootTimer = 0;
                    AcidShootCooldown = 10f;
                    projectile.netUpdate = true;
                }
            }
            else if (AcidShootCooldown > 0f)
                AcidShootCooldown--;
            projectile.velocity.X *= 0.8f;
        }

        public void ManipulateFrames(int minimumFrame, int maximumFrame)
        {
            if (projectile.frame < minimumFrame)
            {
                projectile.frame = minimumFrame;
            }
            projectile.frameCounter++;
            if (projectile.frameCounter >= maximumFrame)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > maximumFrame)
            {
                projectile.frame = minimumFrame;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture(Texture);
            Rectangle frame = texture.Frame(2, Main.projFrames[projectile.type], ReleasingAcid.ToInt(), projectile.frame);
            spriteBatch.Draw(texture,
                             projectile.Center - Main.screenPosition,
                             frame,
                             lightColor,
                             projectile.rotation,
                             projectile.Size * 0.5f,
                             projectile.scale,
                             projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                             0f);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = projectile.Bottom.Y < FallThroughYPoint - 120f;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough);
        }

        public override bool CanDamage() => false;
    }
}
