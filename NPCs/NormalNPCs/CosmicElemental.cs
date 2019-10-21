using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class CosmicElemental : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Elemental");
            Main.npcFrameCount[npc.type] = 22;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 0.5f;
            npc.aiStyle = 91;
            npc.damage = 20;
            npc.width = 20;
            npc.height = 30;
            npc.defense = 10;
            npc.lifeMax = 25;
            npc.knockBackResist = 0.5f;
            animationType = 483;
            npc.value = Item.buyPrice(0, 0, 3, 0);
            npc.HitSound = SoundID.NPCHit7;
            npc.DeathSound = SoundID.NPCDeath6;
            banner = npc.type;
            bannerItem = ModContent.ItemType<CosmicElementalBanner>();
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || spawnInfo.player.Calamity().ZoneAbyss || spawnInfo.player.Calamity().ZoneSunkenSea)
            {
                return 0f;
            }
            return SpawnCondition.Cavern.Chance * 0.01f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (CalamityWorld.revenge)
            {
                player.AddBuff(ModContent.BuffType<Horror>(), 180, true);
            }
            player.AddBuff(BuffID.Confused, 180, true);
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
            if (Main.rand.NextBool(10))
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.BoneSword);
            }
            if (Main.rand.NextBool(50))
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Starfury);
            }
            if (Main.rand.NextBool(100))
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.EnchantedSword);
            }
            if (Main.rand.NextBool(1000))
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Arkhalis);
            }
            if (CalamityWorld.defiled)
            {
                if (Main.rand.NextBool(20))
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Starfury);
                }
                if (Main.rand.NextBool(20))
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.EnchantedSword);
                }
                if (Main.rand.NextBool(20))
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Arkhalis);
                }
            }
        }
    }
}
