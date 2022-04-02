using CalamityMod.Items.Accessories;
using CalamityMod.Items.Placeables.Banners;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class CrawlerAmethyst : ModNPC
    {
        private bool detected = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Amethyst Crawler");
            Main.npcFrameCount[npc.type] = 5;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 0.3f;
            npc.aiStyle = -1;
            npc.damage = 15;
            npc.width = 44;
            npc.height = 34;
            npc.defense = 4;
            npc.lifeMax = 60;
            npc.knockBackResist = 0.75f;
            aiType = -1;
            npc.value = Item.buyPrice(0, 0, 0, 30);
            npc.HitSound = SoundID.NPCHit33;
            npc.DeathSound = SoundID.NPCDeath36;
            banner = npc.type;
            bannerItem = ModContent.ItemType<AmethystCrawlerBanner>();
            npc.Calamity().VulnerableToHeat = true;
            npc.Calamity().VulnerableToCold = true;
            npc.Calamity().VulnerableToSickness = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(detected);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            detected = reader.ReadBoolean();
        }

        public override void FindFrame(int frameHeight)
        {
            if (!detected)
            {
                npc.frame.Y = frameHeight * 4;
                npc.frameCounter = 0.0;
                return;
            }
            npc.spriteDirection = -npc.direction;
            npc.frameCounter += (double)(npc.velocity.Length() / 8f);
            if (npc.frameCounter > 2.0)
            {
                npc.frame.Y = npc.frame.Y + frameHeight;
                npc.frameCounter = 0.0;
            }
            if (npc.frame.Y >= frameHeight * 3)
            {
                npc.frame.Y = 0;
            }
        }

        public override void AI()
        {
            if (!detected)
                npc.TargetClosest();
            if (((Main.player[npc.target].Center - npc.Center).Length() < 100f && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position,
                    Main.player[npc.target].width, Main.player[npc.target].height)) || npc.justHit)
                detected = true;
            if (!detected)
                return;
            CalamityAI.GemCrawlerAI(npc, mod, 4.5f, 0.045f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || spawnInfo.player.Calamity().ZoneAbyss || spawnInfo.player.Calamity().ZoneSunkenSea)
            {
                return 0f;
            }
            return SpawnCondition.Underground.Chance * 0.04f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 70, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 70, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(npc, ItemID.Amethyst, 2, 4);
            DropHelper.DropItemChance(npc, ModContent.ItemType<ScuttlersJewel>(), 10);
        }
    }
}
