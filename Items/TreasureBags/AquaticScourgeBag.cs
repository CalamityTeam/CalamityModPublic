
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.AquaticScourge;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class AquaticScourgeBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<AquaticScourgeHead>();

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Treasure Bag");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 24;
            item.height = 24;
            item.rare = 9;
            item.expert = true;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void OpenBossBag(Player player)
        {
            player.TryGettingDevArmor();

            // Materials (Souls of Sight are Hardmode only)
            DropHelper.DropItemCondition(player, ItemID.SoulofSight, Main.hardMode, 25, 40);
            DropHelper.DropItem(player, ModContent.ItemType<VictoryShard>(), 15, 25);
            DropHelper.DropItem(player, ItemID.Coral, 7, 11);
            DropHelper.DropItem(player, ItemID.Seashell, 7, 11);
            DropHelper.DropItem(player, ItemID.Starfish, 7, 11);

            // Weapons (Hardmode only)
            DropHelper.DropItemCondition(player, ModContent.ItemType<SubmarineShocker>(), Main.hardMode, 3, 1, 1);
            DropHelper.DropItemCondition(player, ModContent.ItemType<Barinautical>(), Main.hardMode, 3, 1, 1);
            DropHelper.DropItemCondition(player, ModContent.ItemType<Downpour>(), Main.hardMode, 3, 1, 1);
            DropHelper.DropItemCondition(player, ModContent.ItemType<DeepseaStaff>(), Main.hardMode, 3, 1, 1);
            DropHelper.DropItemCondition(player, ModContent.ItemType<ScourgeoftheSeas>(), Main.hardMode, 3, 1, 1);

            // Equipment (Emblem is Hardmode only)
            DropHelper.DropItemCondition(player, ModContent.ItemType<AquaticEmblem>(), Main.hardMode);
            DropHelper.DropItemChance(player, ModContent.ItemType<AeroStone>(), 8);

            // Vanity
            // there is no Aquatic Scourge mask yet

            // Fishing
            DropHelper.DropItemChance(player, ItemID.HighTestFishingLine, 10);
            DropHelper.DropItemChance(player, ItemID.AnglerTackleBag, 10);
            DropHelper.DropItemChance(player, ItemID.TackleBox, 10);
            DropHelper.DropItemChance(player, ItemID.AnglerEarring, 8);
            DropHelper.DropItemChance(player, ItemID.FishermansGuide, 8);
            DropHelper.DropItemChance(player, ItemID.WeatherRadio, 8);
            DropHelper.DropItemChance(player, ItemID.Sextant, 8);
            DropHelper.DropItemChance(player, ItemID.AnglerHat, 3);
            DropHelper.DropItemChance(player, ItemID.AnglerVest, 3);
            DropHelper.DropItemChance(player, ItemID.AnglerPants, 3);
            DropHelper.DropItemChance(player, ItemID.FishingPotion, 3, 2, 3);
            DropHelper.DropItemChance(player, ItemID.SonarPotion, 3, 2, 3);
            DropHelper.DropItemChance(player, ItemID.CratePotion, 3, 2, 3);
            DropHelper.DropItemCondition(player, ItemID.GoldenBugNet, NPC.downedBoss3, 12, 1, 1);
        }
    }
}
