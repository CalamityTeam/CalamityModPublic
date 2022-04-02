using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.Crags
{
    public class DespairStone : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Despair Stone");
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            aiType = -1;
            npc.damage = 40;
            npc.width = 72;
            npc.height = 72;
            npc.defense = 38;
            npc.DR_NERD(0.35f);
            npc.lifeMax = 120;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 0, 5, 0);
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.behindTiles = true;
            npc.lavaImmune = true;
            if (CalamityWorld.downedProvidence)
            {
                npc.damage = 80;
                npc.defense = 50;
                npc.lifeMax = 3000;
            }
            banner = npc.type;
            bannerItem = ModContent.ItemType<DespairStoneBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToCold = true;
            npc.Calamity().VulnerableToWater = true;
        }

        public override void AI()
        {
            CalamityAI.UnicornAI(npc, mod, true, CalamityWorld.death ? 6f : 4f, 5f, 0.2f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.player.Calamity().ZoneCalamity ? 0.25f : 0f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemCondition(npc, ModContent.ItemType<Bloodstone>(), CalamityWorld.downedProvidence, 2, 1, 1);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<EssenceofChaos>(), Main.hardMode, 3, 1, 1);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Brimstone, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 40; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Brimstone, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/DespairStoneGores/DespairStone"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/DespairStoneGores/DespairStone2"), npc.scale);
            }
        }
    }
}
