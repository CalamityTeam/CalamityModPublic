using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Melee;
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
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = 1;
			aiType = NPCID.DungeonSlime;
			npc.damage = 30;
            npc.width = 60;
            npc.height = 42;
            npc.defense = 8;
            npc.lifeMax = 130;
            npc.knockBackResist = 0.3f;
            animationType = NPCID.RainbowSlime;
            npc.value = Item.buyPrice(0, 0, 2, 0);
            npc.alpha = 105;
            npc.lavaImmune = false;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.buffImmune[BuffID.OnFire] = true;
            banner = npc.type;
            bannerItem = ModContent.ItemType<CrimulanBlightSlimeBanner>();
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || spawnInfo.player.Calamity().ZoneAbyss)
            {
                return 0f;
            }
            return SpawnCondition.Crimson.Chance * 0.15f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 40; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.ManaSickness, 120, true);
            player.AddBuff(BuffID.Confused, 120, true);
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(npc, ModContent.ItemType<EbonianGel>(), 15, 16);
            DropHelper.DropItem(npc, ItemID.Gel, 10, 14);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<Carnage>(), NPC.downedBoss3, 0.01f, 1, 1);
        }
    }
}
