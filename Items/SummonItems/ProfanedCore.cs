using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Providence;
using CalamityMod.Projectiles.Boss;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    [LegacyName("ProfanedCoreUnlimited")]
    public class ProfanedCore : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.SummonItems";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 19; // Celestial Sigil
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.rare = ItemRarityID.Purple;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossItem;
        }

        public override bool CanUseItem(Player player)
        {
            var Prov = CalamityGlobalNPC.holyBoss;
            bool canPissOffProvi = Prov != -1 && Main.npc[Prov].life >= (Main.npc[Prov].lifeMax * 0.95f) && Main.npc[Prov].Calamity().newAI[3] >= 180f && !Main.npc[Prov].Calamity().CurrentlyEnraged;
            return ((!NPC.AnyNPCs(ModContent.NPCType<Providence>()) && (player.ZoneHallow || player.ZoneUnderworldHeight)) || canPissOffProvi) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            var Prov = CalamityGlobalNPC.holyBoss;
            bool usingToMakeProviPissedOff = Prov != -1 && Main.npc[Prov].life >= (Main.npc[Prov].lifeMax * 0.95f) && Main.npc[Prov].Calamity().newAI[3] >= 180f && !Main.npc[Prov].Calamity().CurrentlyEnraged;
            if (usingToMakeProviPissedOff)
            {
                (Main.npc[Prov].ModNPC as Providence).hasBeenGivenFullPower = true;
                Projectile.NewProjectile(Item.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<HolyProfanedCore>(), 0, 0);
            }
            else
            {
                int posX = (int)player.position.X;
                int posY = (int)(player.position.Y - 100f);
                int bossToSpawn = ModContent.NPCType<Providence>();
                CalamityUtils.SpawnBossOnPosUsingItem(player, bossToSpawn, posX, posY, Providence.SpawnSound);
            }
            return true;
        }
    }
}
