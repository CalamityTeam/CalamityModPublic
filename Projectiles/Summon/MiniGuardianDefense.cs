using System;
using System.IO;
using CalamityMod.NPCs.Providence;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class MiniGuardianDefense : ModProjectile, ILocalizedModType
    {
        public enum MiniDefenderAIState
        {
            ShieldActive,
            ShieldInactive,
            Vanity
        }
        public new string LocalizationCategory => "Projectiles.Summon";
        public Player Owner => Main.player[Projectile.owner];

        public bool SpawnedFromPSC => Projectile.ai[0] == 1f;
        public bool ForcedVanity => SpawnedFromPSC && !Owner.Calamity().profanedCrystalBuffs;
        public bool shieldActive => !ForcedVanity && Owner.Calamity().pSoulShieldDurability > 0;

        public bool shieldActiveBefore = false;
        

        public MiniDefenderAIState AIState => ForcedVanity ? MiniDefenderAIState.Vanity : (shieldActive ? MiniDefenderAIState.ShieldActive : MiniDefenderAIState.ShieldInactive);

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.width = 62;
            Projectile.height = 80;
            Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
        }


        private void HandleRocks(bool spawnRocks = false, bool yeetRocks = false)
        {
            if (spawnRocks)
            {
                //spawn rocks
                bool psc = Owner.Calamity().profanedCrystalBuffs;
                int rockCount = psc ? 10 : 5;
                int[] validRockTypes = psc ? new int[]{ 1, 3, 4, 5, 6 } : new int[]{ 3, 5, 6 }; //1 and 4 are chonkier and psc pushes shield so it looks less weirdge
                float angleVariance = MathHelper.TwoPi / rockCount;
                float angle = 0f;
                for (int i = 0; i < rockCount; i++)
                {
                    int rockType = validRockTypes[Main.rand.Next(0, validRockTypes.Length)];
                    var rockyRoad = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.position,
                        angle.ToRotationVector2() * 8f, ModContent.ProjectileType<MiniGuardianRock>(), 1, 2f, Owner.whoAmI, 0f, angle, rockType);
                    rockyRoad.originalDamage = Projectile.originalDamage;
                    angle += angleVariance;
                }
            }
            else if (yeetRocks)
            {
                //flag the rocks for yeetage
                int rock = ModContent.ProjectileType<MiniGuardianRock>();
                foreach (var proj in Main.projectile)
                {
                    if (proj.active && proj.owner == Owner.whoAmI && proj.type == rock)
                    {
                        proj.ai[0] = 1f;
                    }
                }
            }
        }

        public override void AI()
        {
            // Despawn properly
            if (Owner.Calamity().pSoulGuardians)
                Projectile.timeLeft = 2;
            if (!Owner.Calamity().pSoulArtifact || Owner.dead || !Owner.active)
            {
                Owner.Calamity().pSoulGuardians = false;
                Projectile.active = false;
                return;
            }
            //dust and framing
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 6 % Main.projFrames[Projectile.type];

            var psc = Owner.Calamity().profanedCrystal;
            if (psc && !SpawnedFromPSC || !psc && SpawnedFromPSC)
            {
                int rock = ModContent.ProjectileType<MiniGuardianRock>();
                foreach (var proj in Main.projectile)
                {
                    if (proj.active && proj.owner == Owner.whoAmI && proj.type == rock)
                    {
                        proj.active = false;
                    }
                }
                Projectile.active = false;
            }

            var shieldIsActive = shieldActive; //avoid checkiing cooldowns multiple times per frame
            
            bool shouldSpawnRocks = !shieldActiveBefore && shieldIsActive;
            bool shouldYeetRocks = shieldActiveBefore && !shieldIsActive;
            HandleRocks(shouldSpawnRocks, shouldYeetRocks); 
            
            bool shouldDust = shouldSpawnRocks || shouldYeetRocks; 
            
            if (shouldDust)
            {
                for (int i = 0; i < 20; i++)
                {
                    Vector2 dustPos = new Vector2(Owner.Center.X + Main.rand.NextFloat(-10, 10), Owner.Center.Y + Main.rand.NextFloat(-10, 10));
                    Vector2 velocity = (Owner.Center - dustPos).SafeNormalize(Vector2.Zero);
                    velocity *= (Main.dayTime || !SpawnedFromPSC) ? 3f : 6.9f;
                    var dust = Dust.NewDustPerfect(Owner.Center, ProvUtils.GetDustID((float)((Main.dayTime || !SpawnedFromPSC) ? Providence.BossMode.Day : Providence.BossMode.Night)), velocity, 0, default(Color), 2f);
                    if (!Main.dayTime && SpawnedFromPSC)
                        dust.noGravity = true;
                }
            }
            
            
            // Doesn't deal damage directly, damage used for rocks
            NPC potentialTarget = Projectile.Center.MinionHoming(1500f, Owner);
            Vector2 playerDestination = Owner.Center - Projectile.Center;
            switch (AIState)
            {
                case MiniDefenderAIState.ShieldActive:
                case MiniDefenderAIState.ShieldInactive:
                    if (AIState == MiniDefenderAIState.ShieldInactive) //dust only while inactive
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            if (!Main.rand.NextBool(3))
                                continue;

                            Dust dust = Dust.NewDustDirect(Owner.position, Owner.width, Owner.height, ProvUtils.GetDustID((float)((Main.dayTime || !SpawnedFromPSC) ? Providence.BossMode.Day : Providence.BossMode.Night)));
                            dust.velocity = Main.rand.NextVector2Circular(3.5f, 3.5f);
                            dust.velocity.Y -= Main.rand.NextFloat(1f, 3f);
                            dust.scale = Main.rand.NextFloat(1.15f, 1.45f);
                            dust.noGravity = true;
                        }
                    }

                    if (potentialTarget != null)
                    {
                        Vector2 angle = Owner.Center + Owner.SafeDirectionTo(potentialTarget.Center) * (shieldIsActive ? (Owner.Calamity().profanedCrystalBuffs ? 125f : 75f) : -50f);
                        playerDestination = angle;
                        playerDestination.X += Main.rand.NextFloat(-5f, 5f);
                        playerDestination.Y += Main.rand.NextFloat(-5f, 5f);
                    }
                    else
                    {
                        playerDestination.X += Main.rand.NextFloat(-10f, 10f) + (75f * (shieldIsActive ? Owner.direction : -Owner.direction));
                        playerDestination.Y += Main.rand.NextFloat(-10f, 10f);  
                    }
                    break;
                case MiniDefenderAIState.Vanity:
                    playerDestination.X += Main.rand.NextFloat(-10f, 20f) - (60f * Owner.direction);
                    playerDestination.Y += Main.rand.NextFloat(-10f, 20f) - 60f;
                    break;
            }

            if (potentialTarget != null && AIState != MiniDefenderAIState.Vanity)
            {
                float dist = Projectile.Center.Distance(playerDestination);
                float num543 = playerDestination.X;
                float num544 = playerDestination.Y;
                float num550 = 40f;
                Vector2 vector43 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                float num551 = num543 - vector43.X;
                float num552 = num544 - vector43.Y;
                float num553 = (float)Math.Sqrt((double)(num551 * num551 + num552 * num552));
                if (num553 < 100f)
                {
                    num550 = 28f; //14
                }
                num553 = num550 / num553;
                num551 *= num553;
                num552 *= num553;
                Projectile.velocity.X = (Projectile.velocity.X * 14f + num551) / 13.5f;
                Projectile.velocity.Y = (Projectile.velocity.Y * 14f + num552) / 13.5f;

                Projectile.velocity *= dist > 10 ? 0.9f : 0.3f;
                Projectile.spriteDirection = Projectile.DirectionTo(potentialTarget.Center).X > 0 ? 1 : -1;
            }
            else 
            {
                float playerDist = playerDestination.Length();
                float acceleration = 0.5f;
                float returnSpeed = 28f;

                // Teleport if too far
                if (playerDist > 2000f)
                {
                    Projectile.position = Owner.position;
                    Projectile.netUpdate = true;
                }
                // Slow down a lot when close
                else if (playerDist < 50f)
                {
                    acceleration = 0.01f;
                    if (Math.Abs(Projectile.velocity.X) > 2f || Math.Abs(Projectile.velocity.Y) > 2f)
                        Projectile.velocity *= 0.9f;
                }
                else
                {
                    if (playerDist < 100f)
                        acceleration = 0.1f;
                    
                    if (playerDist > 300f)
                        acceleration = 1f;

                    playerDist = returnSpeed / playerDist;
                    playerDestination *= playerDist;
                    // Turning (wtf is this) (idk ask phup lmao)
                    if (Projectile.velocity.X < playerDestination.X)
                    {
                        Projectile.velocity.X += acceleration;
                        if (acceleration > 0.05f && Projectile.velocity.X < 0f)
                            Projectile.velocity.X += acceleration;
                    }

                    if (Projectile.velocity.X > playerDestination.X)
                    {
                        Projectile.velocity.X -= acceleration;
                        if (acceleration > 0.05f && Projectile.velocity.X > 0f)
                            Projectile.velocity.X -= acceleration;
                    }

                    if (Projectile.velocity.Y < playerDestination.Y)
                    {
                        Projectile.velocity.Y += acceleration;
                        if (acceleration > 0.05f && Projectile.velocity.Y < 0f)
                            Projectile.velocity.Y += acceleration * 2f;
                    }

                    if (Projectile.velocity.Y > playerDestination.Y)
                    {
                        Projectile.velocity.Y -= acceleration;
                        if (acceleration > 0.05f && Projectile.velocity.Y > 0f)
                            Projectile.velocity.Y -= acceleration * 2f;
                    }
                }
                
                // Direction
                if (Math.Abs(Projectile.velocity.X) > 0.2f)
                    Projectile.direction = Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            }
            
            Projectile.netUpdate = Projectile.netUpdate || (shieldIsActive != shieldActiveBefore);
            shieldActiveBefore = shieldIsActive;
        }
        
        public override bool? CanDamage() => false;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(shieldActiveBefore);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            shieldActiveBefore = reader.ReadBoolean();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Has afterimages if maximum empowerment
            if (SpawnedFromPSC && !ForcedVanity)
            {
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
                return false;
            }
            return true;
        }
    }
}
