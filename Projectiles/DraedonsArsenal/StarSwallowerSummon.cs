using CalamityMod.Buffs.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.Audio;
namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class StarSwallowerSummon : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public bool FlyUntilNearPlayer;
        public float HopCooldown = 0f;
        public float AcidShootTimer = 0f;
        public float AcidShootCooldown = 0f;
        public float FallThroughYPoint = 0f;

        public bool ReleasingAcid
        {
            get => Projectile.ai[0] != 0f;
            set => Projectile.ai[0] = value.ToInt();
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 12;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 30;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.minion = true;
            Projectile.timeLeft *= 5;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Summon;
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
            Player player = Main.player[Projectile.owner];

            bool isCorrectProjectile = Projectile.type == ModContent.ProjectileType<StarSwallowerSummon>();
            player.AddBuff(ModContent.BuffType<StarSwallowerBuff>(), 3600);
            if (isCorrectProjectile)
            {
                if (player.dead)
                {
                    player.Calamity().starSwallowerPetFroge = false;
                }
                if (player.Calamity().starSwallowerPetFroge)
                {
                    Projectile.timeLeft = 2;
                }
            }

            int minimumFrame = 0;
            int maximumFrame = 11;

            if (Projectile.velocity == Vector2.Zero)
            {
                minimumFrame = maximumFrame = 0;
            }

            if (HopCooldown > 0)
            {
                if (Projectile.velocity.Length() > 8f)
                {
                    minimumFrame = maximumFrame = 1;
                    if (Math.Abs(Projectile.oldPosition.Y - Projectile.position.Y) < 5.5f)
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

            // Gravity effect.
            Projectile.velocity.Y = MathHelper.Clamp(Projectile.velocity.Y + 0.4f, -11f, 11f);

            Projectile.tileCollide = !FlyUntilNearPlayer;

            ReleasingAcid = false;

            // Remember, && doesn't evaluate future statements if the statement it just evaluated is true.
            // This means that null problems should never happen, since it wouldn't pass the potentialTarget != null check.
            NPC potentialTarget = Projectile.Center.MinionHoming(800f, player);
            if (potentialTarget != null &&
                Math.Abs(potentialTarget.Center.X - Projectile.Center.X) < 320f &&
                Math.Sign(potentialTarget.Center.Y - Projectile.Center.Y) == -1 &&
                Math.Abs(potentialTarget.Center.Y - Projectile.Center.Y) < 160f &&
                Math.Sign(potentialTarget.Center.Y - Projectile.Center.Y) == -1 &&
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

            Projectile.MinionAntiClump();
            ManipulateFrames(minimumFrame, maximumFrame);
        }

        public void HandleHop(Vector2 targetPosition)
        {
            if (HopCooldown <= 0f && Projectile.oldPosition.Y == Projectile.position.Y)
            {
                float hopXMult = Utils.GetLerpValue(50f, 500f, targetPosition.X - Projectile.Center.X, true);
                float hopYMult = Utils.GetLerpValue(40f, 300f, targetPosition.Y - Projectile.Center.Y, true);
                float hopSpeedX = MathHelper.Lerp(4f, 17f, hopXMult) * Math.Sign(targetPosition.X - Projectile.Center.X);
                float hopSpeedY = -1 * MathHelper.Lerp(8f, 23f, hopYMult);

                Vector2 velocity = new Vector2(hopSpeedX, hopSpeedY);

                Projectile.velocity = velocity;
                Projectile.spriteDirection = (Projectile.velocity.X > 0).ToDirectionInt();
                HopCooldown = 25f;
            }
            else
            {
                Projectile.velocity.X *= 0.935f;
                HopCooldown--;
            }
        }

        public void PlayerSeparationAnxietyAI(Player player, ref int minimumFrame, ref int maximumFrame)
        {
            // If the player is extremely far away, don't bother homing. Just go to the player's location.
            if (Projectile.Distance(player.Center) > 1900f)
            {
                Projectile.Center = player.Center;
                Projectile.netUpdate = true;
                return;
            }
            Vector2 returnPosition = Projectile.Center;
            returnPosition.Y -= 200 * 16;
            returnPosition.Y = MathHelper.Clamp(returnPosition.Y, 50, Main.maxTilesX * 16 - 50);

            bool noGround = !WorldUtils.Find(returnPosition.ToTileCoordinates(), Searches.Chain(new Searches.Down(400), new GenCondition[]
            {
                    new Conditions.IsSolid()
            }), out _);

            if (Projectile.Distance(player.Center) > 1200f)
            {
                FlyUntilNearPlayer = true;
                Projectile.netUpdate = true;
            }
            if (Projectile.Distance(player.Center) < 120f &&
                FlyUntilNearPlayer &&
                !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                FlyUntilNearPlayer = false;
                Projectile.netUpdate = true;
            }

            bool flying = noGround || FlyUntilNearPlayer;

            // Fly to player.
            if (flying)
            {
                minimumFrame = 7;
                maximumFrame = 11;
                float inertia = 30f;
                Projectile.velocity = (Projectile.velocity * (inertia - 1) + Projectile.SafeDirectionTo(player.Center) * 18f) / inertia;
            }
            // Otherwise hop around when on the ground.
            else if (Projectile.oldPosition.Y == Projectile.position.Y)
            {
                HandleHop(player.Center);
            }
        }

        public void NPCTargetingAI(NPC potentialTarget)
        {
            HandleHop(potentialTarget.Center);
            if (Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, potentialTarget.position, potentialTarget.width, Projectile.height) && AcidShootCooldown <= 0f)
            {
                AcidShootTimer++;
                ReleasingAcid = AcidShootTimer >= 16 && AcidShootTimer <= 44;

                if (AcidShootTimer >= 20 &&
                    AcidShootTimer <= 44 &&
                    AcidShootTimer % 6 == 0 &&
                    Main.myPlayer == Projectile.owner)
                {
                    Vector2 spawnPosition = Projectile.Center + Vector2.UnitX * 8f * Projectile.spriteDirection;
                    Projectile.spriteDirection = (potentialTarget.Center.X - Projectile.Center.X > 0).ToDirectionInt();
                    Vector2 fireVelocity = CalamityUtils.GetProjectilePhysicsFiringVelocity(spawnPosition, potentialTarget.Center, StarSwallowerAcid.Gravity, 14f);

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPosition,
                                             fireVelocity,
                                             ModContent.ProjectileType<StarSwallowerAcid>(),
                                             Projectile.damage,
                                             Projectile.knockBack,
                                             Projectile.owner);
                    SoundEngine.PlaySound(SoundID.Item13, spawnPosition);
                }
                if (AcidShootTimer > 48)
                {
                    AcidShootTimer = 0;
                    AcidShootCooldown = 10f;
                    Projectile.netUpdate = true;
                }
            }
            else if (AcidShootCooldown > 0f)
                AcidShootCooldown--;
            Projectile.velocity.X *= 0.8f;
        }

        public void ManipulateFrames(int minimumFrame, int maximumFrame)
        {
            if (Projectile.frame < minimumFrame)
            {
                Projectile.frame = minimumFrame;
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= maximumFrame)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > maximumFrame)
            {
                Projectile.frame = minimumFrame;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = texture.Frame(2, Main.projFrames[Projectile.type], ReleasingAcid.ToInt(), Projectile.frame);
            Main.EntitySpriteDraw(texture,
                             Projectile.Center - Main.screenPosition,
                             frame,
                             lightColor,
                             Projectile.rotation,
                             Projectile.Size * 0.5f,
                             Projectile.scale,
                             Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                             0);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = Projectile.Bottom.Y < FallThroughYPoint - 120f;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool? CanDamage() => false;
    }
}
