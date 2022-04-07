using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Astral
{
    public class AstralSlime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Slime");
            Main.npcFrameCount[NPC.type] = 2;
        }

        public override void SetDefaults()
        {
            NPC.damage = 40;
            NPC.width = 36;
            NPC.height = 31;
            NPC.aiStyle = 1;
            NPC.defense = 8;
            NPC.lifeMax = 200;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 10, 0);
            NPC.alpha = 60;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            animationType = NPCID.BlueSlime;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<AstralSlimeBanner>();
            if (DownedBossSystem.downedAstrumAureus)
            {
                NPC.damage = 65;
                NPC.defense = 18;
                NPC.lifeMax = 310;
            }
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void FindFrame(int frameHeight)
        {
            //DO DUST
            Dust d = CalamityGlobalNPC.SpawnDustOnNPC(NPC, 44, frameHeight, ModContent.DustType<AstralOrange>(), new Rectangle(4, 4, 36, 24), Vector2.Zero, 0.15f, true);
            if (d != null)
            {
                d.customData = 0.04f;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            CalamityGlobalNPC.DoHitDust(NPC, hitDirection, ModContent.DustType<AstralOrange>(), 1f, 4, 24);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.Player))
            {
                return 0f;
            }
            else if (spawnInfo.Player.InAstral(1))
            {
                return 0.21f;
            }
            return 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120, true);
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(NPC, ModContent.ItemType<Stardust>(), Main.expertMode ? 1 : 2, 1, 3);
            int oreMin = Main.expertMode ? 11 : 8;
            int oreMax = Main.expertMode ? 16 : 12;
            DropHelper.DropItemCondition(NPC, ModContent.ItemType<AstralOre>(), DownedBossSystem.downedAstrumDeus, oreMin, oreMax);
            DropHelper.DropItemCondition(NPC, ModContent.ItemType<AbandonedSlimeStaff>(), DownedBossSystem.downedAstrumAureus, 0.05f, 1, 1);
        }
    }
}
