using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class CrimulanBlightSlime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crimulan Blight Slime");
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = 1;
            AIType = NPCID.DungeonSlime;
            NPC.damage = 30;
            NPC.width = 60;
            NPC.height = 42;
            NPC.defense = 8;
            NPC.lifeMax = 130;
            NPC.knockBackResist = 0.3f;
            animationType = NPCID.RainbowSlime;
            NPC.value = Item.buyPrice(0, 0, 2, 0);
            NPC.alpha = 105;
            NPC.lavaImmune = false;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<CrimulanBlightSlimeBanner>();
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || spawnInfo.Player.Calamity().ZoneAbyss)
            {
                return 0f;
            }
            return SpawnCondition.Crimson.Chance * 0.15f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 40; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Cursed, 60, true);
        }

        public override void NPCLoot()
        {
            int item = Item.NewItem(NPC.Center, NPC.Size, ModContent.ItemType<EbonianGel>(), Main.rand.Next(15, 21), false, 0, false, false);
            Main.item[item].notAmmo = true;
            NetMessage.SendData(MessageID.ItemTweaker, -1, -1, null, item, 1f, 0f, 0f, 0, 0, 0);

            DropHelper.DropItem(NPC, ItemID.Gel, 10, 14);
        }
    }
}
