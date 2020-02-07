using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.Crags
{
    public class CalamityEye : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Calamity Eye");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.lavaImmune = true;
            npc.aiStyle = 2;
			aiType = 133;
            npc.damage = 40;
            npc.width = 30;
            npc.height = 30;
            npc.defense = 12;
            npc.lifeMax = 140;
            npc.knockBackResist = 0f;
            animationType = 2;
            npc.value = Item.buyPrice(0, 0, 5, 0);
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            if (CalamityWorld.downedProvidence)
            {
                npc.damage = 227;
                npc.defense = 101;
                npc.lifeMax = 5000;
                npc.value = Item.buyPrice(0, 0, 50, 0);
            }
            banner = npc.type;
            bannerItem = ModContent.ItemType<CalamityEyeBanner>();
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.player.Calamity().ZoneCalamity ? 0.25f : 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Weak, 120, true);
            player.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120, true);
            if (CalamityWorld.revenge)
            {
                player.AddBuff(ModContent.BuffType<Horror>(), 180, true);
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemCondition(npc, ModContent.ItemType<Bloodstone>(), CalamityWorld.downedProvidence, 2, 1, 1);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<EssenceofChaos>(), Main.hardMode, 3, 1, 1);
            DropHelper.DropItemChance(npc, ModContent.ItemType<BlightedLens>(), 2);
            DropHelper.DropItemChance(npc, ItemID.Lens, 2);
        }
    }
}
