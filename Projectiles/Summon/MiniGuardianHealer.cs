using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using CalamityMod.Buffs.Summon.Whips;
using CalamityMod.Items.Accessories;
using CalamityMod.NPCs.Providence;
using static CalamityMod.Items.Accessories.ProfanedSoulCrystal;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class MiniGuardianHealer : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        internal const int starTimer = 180;
        internal const int laserTimer = 1200;
        private bool hasSetTimers;
        
        public Player Owner => Main.player[Projectile.owner];

        public bool SpawnedFromPSC => Projectile.ai[0] == 1f;
        public bool ForcedVanity => SpawnedFromPSC && !Owner.Calamity().profanedCrystalBuffs;
        
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public bool isEmpowered => Main.player[Projectile.owner].Calamity().pscState == (int)ProfanedSoulCrystal.ProfanedSoulCrystalState.Empowered;

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.width = 68;
            Projectile.height = 82;
            Projectile.friendly = true;
            Projectile.minionSlots = 0f;
            Projectile.minion = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18000;
            Projectile.timeLeft *= 5;
        }

        public override bool? CanCutTiles()
        {
            CalamityPlayer modPlayer = Main.player[Projectile.owner].Calamity();
            bool psa = modPlayer.pSoulArtifact && !modPlayer.profanedCrystal;
            if (!psa && !modPlayer.profanedCrystalBuffs)
                return false;
            return null;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            
            if (modPlayer.pSoulGuardians)
            {
                Projectile.timeLeft = 2;
            }
            if (!modPlayer.pSoulArtifact || player.dead || !player.active)
            {
                modPlayer.pSoulGuardians = false;
                Projectile.active = false;
                return;
            }

            if (!hasSetTimers)
            {
                Projectile.ai[1] = starTimer;
                Projectile.ai[2] = laserTimer;
                hasSetTimers = true;
                Projectile.netUpdate = true;
            }
            
            Projectile.MinionAntiClump();
            Player owner = Owner;
            
            var psc = owner.Calamity().profanedCrystal;
            if (psc && !SpawnedFromPSC || !psc && SpawnedFromPSC)
            {
                Projectile.active = false;
            }
            
            Vector2 playerDestination = owner.Center - Projectile.Center;
            bool empowered = isEmpowered; 
            

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 5)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }

            NPC target = null;
            //this player whoami check is necessary, otherwise a laser occurs for every play with psc, which is funny but unbalanced
            if (empowered && player.whoAmI == Main.myPlayer)
            {
                target = CalamityUtils.MinionHoming(Projectile.Center, 2000f, player);
                if (target != null)
                {
                    Projectile.spriteDirection = Projectile.DirectionTo(owner.Center).X > 0 ? 1 : -1;
                    if (Projectile.ai[1] <= 0)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.Center);

                        int totalFlameProjectiles = 30;
                        int totalRings = 3;
                        double radians = MathHelper.TwoPi / totalFlameProjectiles;
                        double angleA = radians * 0.5;
                        double angleB = MathHelper.ToRadians(90f) - angleA;
                        for (int i = 0; i < totalRings; i++)
                        {
                            bool firstRing = i % 2 == 0;
                            float starVelocity = i + 2f;
                            float velocityX = (float)(starVelocity * Math.Sin(angleA) / Math.Sin(angleB));
                            Vector2 spinningPoint = firstRing ? new Vector2(-velocityX, -starVelocity) : new Vector2(0f, -starVelocity);
                            for (int j = 0; j < totalFlameProjectiles; j++)
                            {
                                Vector2 vector2 = spinningPoint.RotatedBy(radians * j);
                                int type = ModContent.ProjectileType<MiniGuardianStars>();
                                int dmgAmt = Projectile.originalDamage;
                                var star = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, vector2 * 2.5f, type, dmgAmt, 0f, Main.myPlayer);
                                star.originalDamage = Projectile.originalDamage;

                                Color dustColor = ProfanedSoulCrystal.GetColorForPsc(modPlayer.pscState, Main.dayTime);
                                dustColor.A = 255;
                                int maxDust = 3;
                                for (int k = 0; k < maxDust; k++)
                                {
                                    int dust = Dust.NewDust(Projectile.Center, 0, 0, 267, 0f, 0f, 0, dustColor, 1f);
                                    Main.dust[dust].position = Projectile.Center;
                                    Main.dust[dust].velocity = vector2 * starVelocity * (k * 0.5f + 1f);
                                    Main.dust[dust].noGravity = true;
                                    Main.dust[dust].scale = 1f + k;
                                    Main.dust[dust].fadeIn = Main.rand.NextFloat() * 2f;
                                    Dust dust2 = Dust.CloneDust(dust);
                                    Dust dust3 = dust2;
                                    dust3.scale /= 2f;
                                    dust3 = dust2;
                                    dust3.fadeIn /= 2f;
                                    dust2.color = new Color(255, 255, 255, 255);
                                }
                            }
                        }
                        Projectile.ai[1] = starTimer;
                    }

                    bool canDoLaser = player.HasBuff<ProfanedCrystalWhipBuff>();
                    
                    if (canDoLaser && Projectile.ai[2] <= 0)
                    {
                        SoundEngine.PlaySound(Providence.HolyRaySound, player.Center);
                        float rotation = 435f;
                        Vector2 velocity = target.Center - player.Center;
                        velocity.Normalize();

                        float beamDirection = -1f;
                        if (velocity.X < 0f)
                            beamDirection = 1f;

                        int holyLaserDamage = Projectile.originalDamage * 5;
                        // 60 degrees offset
                        velocity = velocity.RotatedBy(-(double)beamDirection * MathHelper.TwoPi / 4f);
                        int projectile = Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center.X, player.Center.Y, velocity.X, velocity.Y, ModContent.ProjectileType<MiniGuardianHolyRay>(), holyLaserDamage, 0f, Main.myPlayer, beamDirection * MathHelper.TwoPi / rotation, player.whoAmI);

                        if (Main.projectile.IndexInRange(projectile))
                            Main.projectile[projectile].originalDamage = holyLaserDamage;
                        
                        // -60 degrees offset
                        projectile = Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center.X, player.Center.Y, -velocity.X, -velocity.Y, ModContent.ProjectileType<MiniGuardianHolyRay>(), holyLaserDamage, 0f, Main.myPlayer, -beamDirection * MathHelper.TwoPi / rotation, player.whoAmI);
                        
                        if (Main.projectile.IndexInRange(projectile))
                            Main.projectile[projectile].originalDamage = holyLaserDamage;
                        
                        Projectile.ai[2] = laserTimer;
                    }


                    Projectile.ai[1]--;
                    if (canDoLaser) 
                        Projectile.ai[2]--;
                }
            }
            
            if (target == null)
            {
                playerDestination.X += Main.rand.NextFloat(-5f, 5f);
                playerDestination.Y += Main.rand.NextFloat(-10f, 10f);  
                
                float playerDist = playerDestination.Length();
                float acceleration = 0.5f;
                float returnSpeed = 28f;

                // Teleport if too far
                if (playerDist > 2000f)
                {
                    Projectile.position = owner.position;
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
            else
            {
                if (Projectile.ai[2] <= 120f)
                {
                    playerDestination = player.Center;
                    if (Projectile.Hitbox.Intersects(player.Hitbox))
                        Projectile.ai[2] = 0f;
                }
                else
                {
                    playerDestination = target.Center;
                    playerDestination.X += Main.rand.NextFloat(-5f, 5f);
                    playerDestination.Y += Main.rand.NextFloat(-155f, -160f);  
                }
                
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
                
                if (Projectile.ai[2] <= 120f)
                    Projectile.velocity *= dist > 10 ? MathHelper.SmoothStep(0.65f, 0.95f, 120f - Utils.GetLerpValue(120, Projectile.ai[2], 60)) : 0.1f;
                else 
                    Projectile.velocity *= dist > 10 ? 0.9f : 0.3f;
            }

            if (Projectile.ai[2] <= 120f && owner.HasBuff<ProfanedCrystalWhipBuff>())
            {
                int dustCount = (int)Math.Round(MathHelper.SmoothStep(1f, 5f, Projectile.ai[2] / 120));
                float outwardness = MathHelper.SmoothStep(40f, 60f, Projectile.ai[2] / 120);
                float dustScale = MathHelper.Lerp(1.15f, 1.425f, Projectile.ai[2] / 120);
                for (int i = 0; i < dustCount; i++)
                {
                    Vector2 spawnPosition = player.Center + Main.rand.NextVector2Unit() * outwardness * Main.rand.NextFloat(0.75f, 1.1f);
                    Vector2 dustVelocity = (player.Center - spawnPosition) * 0.085f + owner.velocity;
                    
                    int pscState = (int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night);
                    
                    Dust dust = Dust.NewDustPerfect(spawnPosition, ProvUtils.GetDustID(pscState));
                    dust.velocity = dustVelocity;
                    dust.scale = dustScale * Main.rand.NextFloat(0.75f, 1.15f);
                    dust.color = Color.Lerp(Color.LightCoral, Color.White, Projectile.ai[2] / 120 * Main.rand.NextFloat(0.65f, 1f));
                    dust.noGravity = true;
                    dust.noLight = true;
                }
            }
        }
        
        public override bool PreDraw(ref Color lightColor)
        {
            // Has afterimages if maximum empowerment
            if (SpawnedFromPSC && !ForcedVanity && Owner.Calamity().pscState == (int)ProfanedSoulCrystalState.Empowered)
            {
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
                return false;
            }
            return true;
        }

        public override bool? CanDamage() => false;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(hasSetTimers);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            hasSetTimers = reader.ReadBoolean();
        }
    }
}
