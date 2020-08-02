using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class PlaguedFlyingFox : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Melter");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 0.5f;
            npc.aiStyle = 14;
            aiType = NPCID.GiantFlyingFox;
            animationType = NPCID.GiantFlyingFox;
            npc.damage = 55;
            npc.width = 38;
            npc.height = 34;
            npc.defense = 15;
            npc.lifeMax = 500;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 0, 10, 0);
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath4;
            npc.buffImmune[BuffID.ShadowFlame] = true;
            npc.buffImmune[BuffID.Venom] = true;
            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[ModContent.BuffType<BrimstoneFlames>()] = true;
            npc.buffImmune[ModContent.BuffType<Plague>()] = true;
            banner = npc.type;
            bannerItem = ModContent.ItemType<MelterBanner>();
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
            player.AddBuff(ModContent.BuffType<Plague>(), 300, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Plague, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Plague, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void NPCLoot()
        {
			DropHelper.DropItem(npc, ModContent.ItemType<PlagueCellCluster>(), 1, 2);
        }
    }
}
