using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Critters;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class Piggy : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Piggy");
            Main.npcFrameCount[npc.type] = 5;
            Main.npcCatchable[npc.type] = true;
        }

        public override void SetDefaults()
        {
            npc.chaseable = false;
            npc.damage = 0;
            npc.width = 26;
            npc.height = 26;
            npc.lifeMax = 2000;
            npc.aiStyle = 7;
            aiType = NPCID.Squirrel;
            npc.knockBackResist = 0.99f;
            npc.value = Item.buyPrice(0, 2, 0, 0);
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            banner = npc.type;
            bannerItem = ModContent.ItemType<PiggyBanner>();
            npc.catchItem = (short)ModContent.ItemType<PiggyItem>();
            npc.Calamity().VulnerableToHeat = true;
            npc.Calamity().VulnerableToCold = true;
            npc.Calamity().VulnerableToSickness = true;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.Calamity().ZoneSulphur || spawnInfo.player.Calamity().ZoneSunkenSea)
            {
                return 0f;
            }
            return SpawnCondition.TownCritter.Chance * 0.005f;
        }

        public override void FindFrame(int frameHeight)
        {
            if (npc.velocity.Y == 0f)
            {
                if (npc.direction == 1)
                {
                    npc.spriteDirection = -1;
                }
                if (npc.direction == -1)
                {
                    npc.spriteDirection = 1;
                }
                if (npc.velocity.X == 0f)
                {
                    npc.frame.Y = 0;
                    npc.frameCounter = 0.0;
                    return;
                }
                npc.frameCounter += (double)(Math.Abs(npc.velocity.X) * 0.25f);
                npc.frameCounter += 1.0;
                if (npc.frameCounter > 12.0)
                {
                    npc.frame.Y = npc.frame.Y + frameHeight;
                    npc.frameCounter = 0.0;
                }
                if (npc.frame.Y / frameHeight >= Main.npcFrameCount[npc.type] - 1)
                {
                    npc.frame.Y = frameHeight;
                }
            }
            else
            {
                npc.frameCounter = 0.0;
                npc.frame.Y = frameHeight * 2;
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(npc, ItemID.Bacon, 1, 1);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void OnCatchNPC(Player player, Item item)
        {
            try
            {
            } catch
            {
                return;
            }
        }
    }
}
