using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AquaticScourge
{
    public class AquaticUrchin : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aquatic Urchin");
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            aiType = -1;
            npc.damage = Main.hardMode ? 50 : 25;
            npc.width = 40;
            npc.height = 40;
            npc.defense = 10;
            npc.lifeMax = Main.hardMode ? 300 : 50;
            npc.knockBackResist = 0.8f;
            npc.value = Item.buyPrice(0, 0, 0, 80);
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath15;
            npc.behindTiles = true;
            banner = npc.type;
            bannerItem = ModContent.ItemType<AquaticUrchinBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }

        public override void AI()
        {
            CalamityAI.UnicornAI(npc, mod, true, npc.wet ? 12f : 4f, npc.wet ? 7f : 3f, npc.wet ? 0.12f : 0.05f, npc.wet ? -16f : -7.5f, npc.wet ? -14f : -6.5f, npc.wet ? -13f : -6f, npc.wet ? -12f : -5f, npc.wet ? -15f : -7f);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<Irradiated>(), 120, true);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe)
            {
                return 0f;
            }
            if (spawnInfo.player.Calamity().ZoneSulphur && spawnInfo.water && NPC.CountNPCS(ModContent.NPCType<AquaticUrchin>()) < 2)
            {
                return 0.2f;
            }
            return 0f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(npc, ModContent.ItemType<UrchinStinger>(), 15, 25);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AquaticScourgeGores/AquaticUrchin"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AquaticScourgeGores/AquaticUrchin2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AquaticScourgeGores/AquaticUrchin3"), 1f);
            }
        }
    }
}
