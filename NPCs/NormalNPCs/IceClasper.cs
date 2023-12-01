using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.World;
using CalamityMod.Projectiles.Enemy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using System.IO;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class IceClasper : ModNPC
    {
        public Player player => Main.player[NPC.target];
        
        public bool expert = Main.expertMode;
        public bool revenge = CalamityWorld.revenge;
        public bool death = CalamityWorld.death;

        public enum IceClasperAIState
        {
            Shooting,
            Dashing
        }
        public IceClasperAIState CurrentState
        {
            get => (IceClasperAIState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float RotationIncrease => ref NPC.ai[1];
        public bool checkedRotationDir = false;
        public int rotationDir;

        public ref float TimerForShooting => ref NPC.ai[2];

        public ref float AITimer => ref NPC.ai[3];

        public bool isDashing => (CurrentState == IceClasperAIState.Dashing && AITimer > TimeBeforeDash && AITimer <= TimeBeforeDash + TimeDashing);

        #region Other stats

        public float MaxVelocity = 10f;
        public float DistanceFromPlayer = 500f;
        
        // Although it is weird that Death Mode less projectiles, the AI also changes, making it a shotgun spread of 3 projectiles, so it'd be 2*3.
        public float AmountOfProjectiles = (CalamityWorld.death) ? 2f : (CalamityWorld.revenge) ? 4f : (Main.expertMode) ? 3f : 3f;
        public float TimeBetweenProjectiles = (CalamityWorld.death) ? 50f : (CalamityWorld.revenge) ? 35f : (Main.expertMode) ? 40f : 45f;
        public float TimeBetweenBurst = (CalamityWorld.death) ? 240f : 180f;
        public float ProjectileSpeed = 8f;

        public float TimeBeforeDash = (CalamityWorld.revenge) ? 100f : 120f;
        public float TimeDashing = 100f;
        public float DashSpeed = 6f;

        #endregion

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0);
            value.Rotation = MathHelper.ToRadians(135);
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.npcSlots = 3f;
            NPC.noGravity = true;
            NPC.damage = 32;
            NPC.width = 50;
            NPC.height = 50;
            NPC.defense = 12;
            NPC.lifeMax = 500;
            NPC.knockBackResist = 0.25f;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.value = Item.buyPrice(0, 0, 25, 0);
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath7;
            NPC.rarity = 2;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<IceClasperBanner>();
            NPC.coldDamage = true;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = false;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundSnow,
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.IceClasper")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(rotationDir);
            writer.Write(checkedRotationDir);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            rotationDir = reader.ReadInt32();
            checkedRotationDir = reader.ReadBoolean();
        }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || player.dead || !player.active)
                NPC.TargetClosest(true);

            AIMovement(player);

            float distToTarget = NPC.Distance(player.Center) + .1f;
            // When it's not dashing, it'll just look at the player normally.
            // When it is dashing, so it's not unfair, it'll be slower when it's closer to the player.
            NPC.rotation = NPC.rotation.AngleTowards(NPC.AngleTo(player.Center), (isDashing) ? ((death) ? .0005f : (revenge) ? .0003f : (expert) ? .0002f : .0001f) * distToTarget : .3f);

            Lighting.AddLight(NPC.Center, Color.Cyan.ToVector3());

            switch (CurrentState)
            {
                case IceClasperAIState.Shooting:
                    State_Shooting(player);
                    break;
                case IceClasperAIState.Dashing:
                    State_Dashing(player);
                    break;
            }
        }

        public void AIMovement(Player player)
        {
            // Randomly chooses to go clockwise or anti-clockwise around the player.
            if (!checkedRotationDir)
            {
                rotationDir = (Main.rand.NextBool()).ToDirectionInt();
                checkedRotationDir = true;
                NPC.netUpdate = true;
            }

            Vector2 shootingPos = player.Center + new Vector2(MathF.Cos(RotationIncrease) * rotationDir, MathF.Sin(RotationIncrease) * rotationDir) * DistanceFromPlayer;
            RotationIncrease += (CurrentState == IceClasperAIState.Shooting) ? .02f : .008f;

            NPC.velocity = Vector2.Lerp(NPC.velocity, (shootingPos - NPC.Center).SafeNormalize(Vector2.Zero) * 6f, .1f);
            NPC.velocity = Vector2.Clamp(NPC.velocity, new Vector2(-MaxVelocity, -MaxVelocity), new Vector2(MaxVelocity, MaxVelocity));

            NPC.netUpdate = true;
        }

        public void State_Shooting(Player player)
        {                        
            // Minimun distance so the minion is able to shoot.
            if (NPC.Distance(player.Center) > 800f)
                return;
            
            AITimer++;

            if (AITimer >= TimeBetweenBurst)
            {
                if (TimerForShooting % TimeBetweenProjectiles == 0)
                {
                    Vector2 vecToPlayer = NPC.SafeDirectionTo(player.Center);
                    Vector2 projVelocity = vecToPlayer * ProjectileSpeed;
                    int type = ModContent.ProjectileType<IceClasperEnemyProjectile>();

                    // If Death Mode on, the enemy will shoot out a spead of projectiles, instead of a burst.
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (death)
                        {
                            for (int i = -16; i < 8; i += 8)
                            {
                                Vector2 spreadVelocity = projVelocity.RotatedBy(MathHelper.ToRadians(i));
                                int projectile = Projectile.NewProjectile(NPC.GetSource_FromAI(),
                                    NPC.Center + projVelocity.SafeNormalize(Vector2.Zero) * 10f,
                                    spreadVelocity,
                                    type,
                                    24,
                                    0f,
                                    Main.myPlayer);
                                Main.projectile[projectile].timeLeft = 300;
                            }
                            NPC.netUpdate = true;
                        }
                        else
                        {
                            int projectile = Projectile.NewProjectile(NPC.GetSource_FromAI(),
                                NPC.Center + projVelocity.SafeNormalize(Vector2.Zero) * 10f,
                                projVelocity,
                                type,
                                24,
                                0f,
                                Main.myPlayer);
                            Main.projectile[projectile].timeLeft = 300;
                            NPC.netUpdate = true;
                        }
                    }

                    // Recoil effect when shooting.
                    NPC.velocity -= vecToPlayer * 3f;

                    SoundEngine.PlaySound(SoundID.Item28, NPC.Center);
                    NPC.netUpdate = true;
                }

                TimerForShooting++;

                // When the enemy stops it's burst, reset every timer and go to the dash state.
                if (TimerForShooting >= TimeBetweenProjectiles * AmountOfProjectiles)
                {
                    TimerForShooting = 0f;
                    AITimer = 0f;
                    CurrentState = IceClasperAIState.Dashing;
                    NPC.netUpdate = true;
                }
            }
            // When it's about to shoot, make a dust telegraph.
            else if (AITimer >= TimeBetweenBurst / 2f && AITimer < TimeBetweenBurst)
            {
                Vector2 randPos = Main.rand.NextVector2CircularEdge(100f, 100f);
                Dust telegraphDust = Dust.NewDustPerfect(NPC.Center + randPos, 172, NPC.DirectionFrom(NPC.Center + NPC.velocity + randPos) * Main.rand.NextFloat(5f, 7f), 0, default, 1.5f);
                telegraphDust.noGravity = true;
                NPC.netUpdate = true;
            }
        }   

        public void State_Dashing(Player player)
        {
            float distToTarget = NPC.Distance(player.Center) + .1f;
            AITimer++;
            if (AITimer <= TimeBeforeDash) // Before dashing.
            {
                // When it's preparing to dash, it stands back a bit. Flavor movement.
                NPC.velocity = Vector2.Lerp(NPC.velocity, -NPC.rotation.ToRotationVector2() * 2f, .1f);
                NPC.netUpdate = true;
            }
            else if (AITimer > TimeBeforeDash && AITimer <= TimeBeforeDash + TimeDashing) // While dashing.
            {
                // The enemy will charge at player.
                // And it's velocity will increase inversely proportional to the distance from the player.
                NPC.velocity = NPC.rotation.ToRotationVector2() * (DashSpeed + (2f / (distToTarget * .1f)));
                NPC.netUpdate = true;
            }
            else // Done dashing.
            {
                AITimer = 0f;
                checkedRotationDir = false; // Doing this makes the minion randomly choosing to rotate clockwise or anti-clockwise in the shooting state.
                CurrentState = IceClasperAIState.Shooting;
                NPC.netUpdate = true;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += (isDashing) ? 0.4f : 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.ZoneSnow &&
                !spawnInfo.Player.PillarZone() &&
                !spawnInfo.Player.ZoneDungeon &&
                !spawnInfo.Player.InSunkenSea() &&
                Main.hardMode && !spawnInfo.PlayerInTown && !spawnInfo.Player.ZoneOldOneArmy && !Main.snowMoon && !Main.pumpkinMoon ? 0.02f : 0f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
            {
                target.AddBuff(BuffID.Frostburn, 180, true);
                target.AddBuff(BuffID.Chilled, 120, true);
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 92, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 92, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, Mod.Find<ModGore>("IceClasper").Type);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, Mod.Find<ModGore>("IceClasper2").Type);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, Mod.Find<ModGore>("IceClasper3").Type);
                }
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => (isDashing) ? true : false;

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<EssenceofEleum>());
            npcLoot.Add(ModContent.ItemType<FrostBarrier>(), 5);
            npcLoot.Add(ModContent.ItemType<AncientIceChunk>(), 3);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Vector2 position = NPC.Center - screenPos;
            Vector2 origin = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2);
            position -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
            position += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY);

            // If the enemy is preparing to dash, it'll fade in afterimages.
            // And when it is dashing, the afterimages fade out.
            float interpolant = (AITimer > TimeBeforeDash && AITimer <= TimeBeforeDash + TimeDashing) ? 1f - ((AITimer - TimeBeforeDash) / TimeDashing) :
                (MathHelper.Clamp(AITimer, 0f, TimeBeforeDash) / TimeBeforeDash);
            float AfterimageFade = MathHelper.Lerp(0f, 1f, interpolant);

            if (CurrentState == IceClasperAIState.Dashing && CalamityConfig.Instance.Afterimages)
            {
                for (int i = 0; i < NPC.oldPos.Length; i++)
                {
                    Color afterimageDrawColor = new Color(0.79f, 0.94f, 0.98f) with { A = 125 } * NPC.Opacity * (1f - i / (float)NPC.oldPos.Length) * AfterimageFade;
                    Vector2 afterimageDrawPosition = NPC.oldPos[i] + NPC.Size * 0.5f - screenPos;
                    spriteBatch.Draw(texture, afterimageDrawPosition, NPC.frame, afterimageDrawColor, NPC.rotation - MathHelper.PiOver2, origin, NPC.scale, SpriteEffects.None, 0f);
                }
            }

            spriteBatch.Draw(texture, position, NPC.frame, drawColor, NPC.rotation - MathHelper.PiOver2, origin, NPC.scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
