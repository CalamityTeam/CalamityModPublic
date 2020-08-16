using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Yharon;
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

            // Weapons
			DropHelper.DropWeaponSet(player, 3,
				ModContent.ItemType<DragonRage>(),
				ModContent.ItemType<TheBurningSky>(),
				ModContent.ItemType<DragonsBreath>(),
				ModContent.ItemType<ChickenCannon>(),
				ModContent.ItemType<PhoenixFlameBarrage>(),
				ModContent.ItemType<AngryChickenStaff>(), // Yharon Kindle Staff
				ModContent.ItemType<ProfanedTrident>(), // Infernal Spear
				ModContent.ItemType<FinalDawn>());

            // Equipment
            DropHelper.DropItem(player, ModContent.ItemType<YharimsGift>());

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<YharonMask>(), 7);
            DropHelper.DropItemChance(player, ModContent.ItemType<ForgottenDragonEgg>(), 10);
            DropHelper.DropItemCondition(player, ModContent.ItemType<FoxDrive>(), CalamityWorld.revenge);
        }
    }
}
