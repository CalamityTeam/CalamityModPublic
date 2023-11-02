using CalamityMod.Items.Materials;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class AuroraSpirit : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                SpriteDirection = -1,
                PortraitPositionYOverride = -20f
            };
            value.Position.X += 4f;
            value.Position.Y -= 4f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = NPCAIStyleID.AncientVision;
            NPC.damage = 40;
            NPC.width = 40;
            NPC.height = 24;
            NPC.defense = 8;
            NPC.alpha = 100;
            NPC.lifeMax = 50;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath15;
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
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.AuroraSpirit")
            });
        }

        public override void FindFrame(int frameHeight)
        {
            int currentFrame = 1;
            if (!Main.dedServ)
            {
                if (TextureAssets.Npc[NPC.type].Value == null)
                    return;
                currentFrame = TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type];
            }

            if (!NPC.IsABestiaryIconDummy)
            {
                if (NPC.velocity.X < 0f)
                    NPC.direction = -1;
                else
                    NPC.direction = 1;
                if (NPC.direction == 1)
                    NPC.spriteDirection = 1;
                if (NPC.direction == -1)
                    NPC.spriteDirection = -1;
                NPC.rotation = (float)Math.Atan2((double)NPC.velocity.Y * (double)NPC.direction, (double)NPC.velocity.X * (double)NPC.direction);
            }

            NPC.frameCounter++;
            if (NPC.frameCounter > 4)
            {
              NPC.frame.Y += currentFrame;
              NPC.frameCounter = 0;
            }
            if (NPC.frame.Y / currentFrame >= Main.npcFrameCount[NPC.type])
              NPC.frame.Y = 0;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.ZoneSnow &&
                spawnInfo.Player.ZoneOverworldHeight &&
                !spawnInfo.Player.PillarZone() &&
                !spawnInfo.Player.ZoneDungeon &&
                !spawnInfo.Player.InSunkenSea() &&
                Main.hardMode && !spawnInfo.PlayerInTown && !spawnInfo.Player.ZoneOldOneArmy && !Main.snowMoon && !Main.pumpkinMoon ? 0.03f : 0f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
            {
                target.AddBuff(BuffID.Frostburn, 90, true);
                target.AddBuff(BuffID.Chilled, 60, true);
            }
        }

        public override void AI()
        {
            Lighting.AddLight((int)((NPC.position.X + (float)(NPC.width / 2)) / 16f), (int)((NPC.position.Y + (float)(NPC.height / 2)) / 16f), 0.02f, 0.7f, 0.7f);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 67, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 67, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("CryoSpirit").Type, 1f);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(ModContent.ItemType<EssenceofEleum>());
    }
}
