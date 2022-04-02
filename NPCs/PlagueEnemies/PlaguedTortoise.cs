using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.PlagueEnemies
{
    public class PlaguedTortoise : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plagueshell");
            Main.npcFrameCount[npc.type] = 8;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 2f;
            npc.damage = 80;
            npc.aiStyle = 39;
            npc.width = 46;
            npc.height = 32;
            npc.defense = 32;
            npc.lifeMax = 800;
            npc.knockBackResist = 0.2f;
            animationType = NPCID.GiantTortoise;
            npc.value = Item.buyPrice(0, 0, 20, 0);
            npc.HitSound = SoundID.NPCHit24;
            npc.noGravity = false;
            banner = npc.type;
            bannerItem = ModContent.ItemType<PlagueshellBanner>();
            npc.Calamity().VulnerableToHeat = true;
            npc.Calamity().VulnerableToCold = true;
            npc.Calamity().VulnerableToSickness = false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Plague, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/PlagueSounds/PlagueBoom" + Main.rand.Next(1, 5)), npc.Center);
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Plague, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !NPC.downedGolemBoss || spawnInfo.player.Calamity().ZoneSunkenSea)
            {
                return 0f;
            }
            return SpawnCondition.HardmodeJungle.Chance * 0.09f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<Plague>(), 180, true);
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(npc, ModContent.ItemType<PlagueCellCluster>(), 3, 4);
        }
    }
}
