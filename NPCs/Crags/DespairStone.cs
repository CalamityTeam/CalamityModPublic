using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Utilities;
using System;
using CalamityMod.Items.Placeables;

namespace CalamityMod.NPCs.Crags
{
    public class DespairStone : ModNPC
    {
        public SlotId ChainsawSoundSlot;

        public static readonly SoundStyle ChainsawStartSound = new("CalamityMod/Sounds/Custom/ChainsawStart") { Volume = 0.15f };

        public static readonly SoundStyle ChainsawEndSound = new("CalamityMod/Sounds/Custom/ChainsawEnd") { Volume = 0.15f };

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 40;
            NPC.width = 72;
            NPC.height = 72;
            NPC.defense = 38;
            NPC.DR_NERD(0.35f);
            NPC.lifeMax = 120;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.behindTiles = true;
            NPC.lavaImmune = true;
            if (DownedBossSystem.downedProvidence)
            {
                NPC.damage = 80;
                NPC.defense = 50;
                NPC.lifeMax = 3000;
            }
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<DespairStoneBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToWater = true;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<BrimstoneCragsBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.DespairStone")
            });
        }

        public override void AI()
        {
            float buzzsawStartTime = 480f;
            NPC.ai[1]++;
            if (NPC.ai[2] != 0f) BuzzsawMode();
            if (NPC.ai[1] > buzzsawStartTime) //time needed for buzzsaw mode to be able to be started
            {
                if (NPC.ai[2] == 0f)
                {
                    NPC.ai[1] = buzzsawStartTime + 1f; //freeze buzzsaw timer until buzzsaw mode is initiated.
                    NPC.rotation += NPC.velocity.X * 0.01f;
                    NPC.spriteDirection = -NPC.direction;
                    if (NPC.velocity.Y == 0f || NPC.lavaWet) BuzzsawMode(); //initiate Buzzsaw mode once the npc hits the ground
                }
            }
            else //run regular ai if buzzsaw mode isn't available
            {
                NPC.ai[2] = 0f;
                CalamityAI.UnicornAI(NPC, Mod, true, CalamityWorld.death ? 8f : CalamityWorld.revenge ? 6f : 4f, 5f, 0.2f);
            }
            if (NPC.lavaWet) //float on lava 
                NPC.velocity.Y += -0.8f;
        }

        public void BuzzsawMode()
        {
            float distance;
            float speedCap = 10f;
            //Choose Direction
            if (NPC.ai[2] == 0f)
            {
                ChainsawSoundSlot = SoundEngine.PlaySound(ChainsawStartSound, NPC.Center);
                if (NPC.velocity.X < 0f) //left
                {
                    NPC.ai[2] = -1f;
                }   
                else if (NPC.velocity.X > 0f) //right
                {
                    NPC.ai[2] = 1f;
                }
                else //if npc is stationary when selecting direction, go towards target.
                {
                    distance = Main.player[NPC.target].Center.X - NPC.Center.X;
                    if (distance != 0f)
                    {
                        NPC.ai[2] = distance / Math.Abs(distance);
                    }
                    else NPC.ai[2] = 1f;
                }
            }
            if (SoundEngine.TryGetActiveSound(ChainsawSoundSlot, out var chainsawSound) && chainsawSound.IsPlaying)
            {
                chainsawSound.Position = NPC.Center;
                chainsawSound.Update();
            }
            if (NPC.velocity.X == 0f) //go up if against wall
            {
                if (NPC.velocity.Y > -speedCap) //Accelerate to top speed
                    NPC.velocity.Y += -1.66f;
                if (NPC.velocity.Y < -speedCap) //Cap top speed
                    NPC.velocity.Y = -speedCap;
            }

            if (NPC.velocity.X == 0f || NPC.velocity.Y == 0f) //check if on solid ground
                SpawnSparks();
            else if (Main.player[NPC.target].Center.Y - NPC.Center.Y < 0f && NPC.velocity.Y < 0f && !NPC.lavaWet) //if not on solid ground and target player is above the NPC, reduce gravity
                NPC.velocity.Y += -0.03f;

            if (Math.Abs(NPC.velocity.X) < speedCap) //Accelerate to top speed
                NPC.velocity.X += 1.66f * NPC.ai[2];
            if (Math.Abs(NPC.velocity.X) > speedCap) //Cap top speed
                NPC.velocity.X = speedCap * NPC.ai[2];

            NPC.rotation += speedCap * 0.03f * NPC.ai[2]; //rotate sprite in chosen direction
            NPC.spriteDirection = -NPC.direction;
            //stop buzzsaw mode
            if (NPC.ai[1] > Main.rand.Next(670, 740))
            {
                NPC.ai[1] = 0f;
                NPC.velocity.Y += -3f;
                chainsawSound?.Stop();
                SoundEngine.PlaySound(ChainsawEndSound, NPC.Center);
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.Calamity().ZoneCalamity ? 0.25f : 0f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<BrimstoneSlag>(), 5, 10, 30);
            LeadingConditionRule hardmode = npcLoot.DefineConditionalDropSet(DropHelper.Hardmode());
            LeadingConditionRule postProv = npcLoot.DefineConditionalDropSet(DropHelper.PostProv());
            hardmode.Add(ModContent.ItemType<EssenceofHavoc>(), 2);
            postProv.Add(ModContent.ItemType<Bloodstone>(), 4);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120, true);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 40; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("DespairStone").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("DespairStone2").Type, NPC.scale);
                }
            }
        }

        public override bool PreKill()
        {
            if (SoundEngine.TryGetActiveSound(ChainsawSoundSlot, out var chainsawSound) && chainsawSound.IsPlaying)
                chainsawSound?.Stop();
            return true;
        }
        public void SpawnSparks()
        {
            Vector2 particleSpawnDisplacement;
            Vector2 splatterDirection;
            if (NPC.velocity.X == 0f)
            {
                particleSpawnDisplacement = new Vector2 (24f * NPC.ai[2], 20f);
                splatterDirection = new Vector2(0f, 1f);
            }
            else
            {
                particleSpawnDisplacement = new Vector2(20f * -NPC.ai[2], 24f);
                splatterDirection = new Vector2(-NPC.ai[2], 0f);
            }
            
            //Color impactColor = Color.Lerp(Color.Silver, Color.Gold, Main.rand.NextFloat(0.5f));
            Vector2 bloodSpawnPosition = NPC.Center + particleSpawnDisplacement;

            if (NPC.ai[1] % 4 == 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    int sparkLifetime = Main.rand.Next(14, 21);
                    float sparkScale = Main.rand.NextFloat(0.8f, 1f) + 1f * 0.05f;
                    Color sparkColor = Color.Lerp(Color.DarkGray, Color.DarkRed, Main.rand.NextFloat(0.7f));
                    sparkColor = Color.Lerp(sparkColor, Color.OrangeRed, Main.rand.NextFloat());

                    if (Main.rand.NextBool(10))
                        sparkScale *= 1.4f;

                    Vector2 sparkVelocity = splatterDirection.RotatedByRandom(0.45f) * Main.rand.NextFloat(6f, 13f);
                    sparkVelocity.Y -= 6f;
                    SparkParticle spark = new SparkParticle(bloodSpawnPosition, sparkVelocity, true, sparkLifetime, sparkScale, sparkColor);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
        }
    }
}
