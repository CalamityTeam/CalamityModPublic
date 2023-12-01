using CalamityMod.BiomeManagers;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using CalamityMod.Items.Weapons.Magic;
using Terraria.GameContent.ItemDropRules;

namespace CalamityMod.NPCs.Abyss
{
    public class BoxJellyfish : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0);
            value.Position.Y += 10;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.noGravity = true;
            NPC.damage = 44;
            NPC.width = 30;
            NPC.height = 33;
            NPC.defense = 5;
            NPC.lifeMax = 90;
            NPC.alpha = 20;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.value = Item.buyPrice(0, 0, 0, 80);
            NPC.HitSound = SoundID.NPCHit25;
            NPC.DeathSound = SoundID.NPCDeath28;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<BoxJellyfishBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<AbyssLayer1Biome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.BoxJellyfish")
            });
        }

        public override void AI()
        {
            if (NPC.direction == 0)
            {
                NPC.TargetClosest(true);
            }
            if (!NPC.wet)
            {
                NPC.rotation += NPC.velocity.X * 0.1f;
                if (NPC.velocity.Y == 0f)
                {
                    NPC.velocity.X = NPC.velocity.X * 0.98f;
                    if (NPC.velocity.X > -0.01 && NPC.velocity.X < 0.01)
                    {
                        NPC.velocity.X = 0f;
                    }
                }
                NPC.velocity.Y = NPC.velocity.Y + 0.2f;
                if (NPC.velocity.Y > 10f)
                {
                    NPC.velocity.Y = 10f;
                }
                NPC.ai[0] = 1f;
                return;
            }
            if (NPC.collideX)
            {
                NPC.velocity.X = NPC.velocity.X * -1f;
                NPC.direction *= -1;
            }
            if (NPC.collideY)
            {
                if (NPC.velocity.Y > 0f)
                {
                    NPC.velocity.Y = Math.Abs(NPC.velocity.Y) * -1f;
                    NPC.directionY = -1;
                    NPC.ai[0] = -1f;
                }
                else if (NPC.velocity.Y < 0f)
                {
                    NPC.velocity.Y = Math.Abs(NPC.velocity.Y);
                    NPC.directionY = 1;
                    NPC.ai[0] = 1f;
                }
            }
            bool canAttack = false;
            NPC.TargetClosest(false);
            if (Main.player[NPC.target].wet && !Main.player[NPC.target].dead && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
            {
                canAttack = true;
            }
            if (canAttack)
            {
                NPC.localAI[2] = 1f;
                NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;
                NPC.velocity *= 0.975f;
                float lungeThreshold = 0.8f;
                if (NPC.velocity.X > -lungeThreshold && NPC.velocity.X < lungeThreshold && NPC.velocity.Y > -lungeThreshold && NPC.velocity.Y < lungeThreshold)
                {
                    NPC.TargetClosest(true);
                    float lungeSpeed = CalamityWorld.death ? 12f : CalamityWorld.revenge ? 10f : 8f;
                    Vector2 npcPosition = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
                    float targetX = Main.player[NPC.target].position.X + (Main.player[NPC.target].width / 2) - npcPosition.X;
                    float targetY = Main.player[NPC.target].position.Y + (Main.player[NPC.target].height / 2) - npcPosition.Y;
                    float targetDistance = (float)Math.Sqrt(targetX * targetX + targetY * targetY);
                    targetDistance = lungeSpeed / targetDistance;
                    targetX *= targetDistance;
                    targetY *= targetDistance;
                    NPC.velocity.X = targetX;
                    NPC.velocity.Y = targetY;
                }
            }
            else
            {
                NPC.localAI[2] = 0f;
                NPC.velocity.X = NPC.velocity.X + NPC.direction * 0.02f;
                NPC.rotation = NPC.velocity.X * 0.4f;
                if (NPC.velocity.X < -1f || NPC.velocity.X > 1f)
                {
                    NPC.velocity.X = NPC.velocity.X * 0.95f;
                }
                if (NPC.ai[0] == -1f)
                {
                    NPC.velocity.Y = NPC.velocity.Y - 0.01f;
                    if (NPC.velocity.Y < -1f)
                    {
                        NPC.ai[0] = 1f;
                    }
                }
                else
                {
                    NPC.velocity.Y = NPC.velocity.Y + 0.01f;
                    if (NPC.velocity.Y > 1f)
                    {
                        NPC.ai[0] = -1f;
                    }
                }
                int npcTileX = (int)(NPC.position.X + (NPC.width / 2)) / 16;
                int npcTileY = (int)(NPC.position.Y + (NPC.height / 2)) / 16;
                if (Main.tile[npcTileX, npcTileY - 1].LiquidAmount > 128)
                {
                    if (Main.tile[npcTileX, npcTileY + 1].HasTile)
                    {
                        NPC.ai[0] = -1f;
                    }
                    else if (Main.tile[npcTileX, npcTileY + 2].HasTile)
                    {
                        NPC.ai[0] = -1f;
                    }
                }
                else
                {
                    NPC.ai[0] = 1f;
                }
                if (NPC.velocity.Y > 1.2 || NPC.velocity.Y < -1.2)
                {
                    NPC.velocity.Y = NPC.velocity.Y * 0.99f;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || spawnInfo.Player.Calamity().ZoneSulphur)
            {
                return 0f;
            }
            if (spawnInfo.Player.Calamity().ZoneAbyssLayer1 && spawnInfo.Water)
            {
                return SpawnCondition.CaveJellyfish.Chance * 2.5f;
            }
            return SpawnCondition.OceanMonster.Chance * 0.1f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(BuffID.Venom, 120, true);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemID.JellyfishNecklace, 100);
            var postSkeletron = npcLoot.DefineConditionalDropSet(() => NPC.downedBoss3);
            postSkeletron.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<AbyssShocker>(), 50, 40));
        }
    }
}
