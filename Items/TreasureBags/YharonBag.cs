using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class YharonBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<Yharon>();

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

            // Materials

            // Weapons
            DropHelper.DropItemChance(player, ModContent.ItemType<DragonRage>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<TheBurningSky>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<DragonsBreath>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<ChickenCannon>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<PhoenixFlameBarrage>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<AngryChickenStaff>(), 3); // Yharon Kindle Staff
            DropHelper.DropItemChance(player, ModContent.ItemType<ProfanedTrident>(), 3); // Infernal Spear

            // Equipment
            DropHelper.DropItem(player, ModContent.ItemType<YharimsGift>());

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<YharonMask>(), 7);
            DropHelper.DropItemChance(player, ModContent.ItemType<ForgottenDragonEgg>(), 10);
            DropHelper.DropItemCondition(player, ModContent.ItemType<FoxDrive>(), CalamityWorld.revenge);
        }
    }
}
