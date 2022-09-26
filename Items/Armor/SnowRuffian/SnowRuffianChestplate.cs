using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.SnowRuffian
{
    [AutoloadEquip(EquipType.Body)]
    public class SnowRuffianChestplate : ModItem
    {
        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/SnowRuffian/SnowRuffianChestplate_Back", EquipType.Back, this);
            EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/SnowRuffian/SnowRuffianChestplate_Neck", EquipType.Neck, this);
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Snow Ruffian Chestplate");
            Tooltip.SetDefault("3% increased rogue critical strike chance");

            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);

            ArmorIDs.Body.Sets.HidesTopSkin[equipSlot] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 2; //4
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<ThrowingDamageClass>() += 3;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnySnowBlock", 30).
                AddRecipeGroup("AnyIceBlock", 15).
                AddIngredient(ItemID.BorealWood, 45).
                AddIngredient(ItemID.FlinxFur, 2).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
