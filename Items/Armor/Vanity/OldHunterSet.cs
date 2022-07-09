using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Armor.DesertProwler;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class OldHunterHat : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Old Hunter Hat");
            Tooltip.SetDefault("Piece of attire fashioned after hunters from a far away place..");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DesertProwlerHat>().
                AddIngredient(ItemID.BlackInk).
                AddTile(TileID.DyeVat).
                Register();
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class OldHunterShirt : ModItem, IBulkyArmor
    {
        public string BulkTexture => "CalamityMod/Items/Armor/Vanity/OldHunterShirt_Bulk";

        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/Vanity/OldHunterShirt_Back", EquipType.Back, this);
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Old Hunter Shirt");
            Tooltip.SetDefault("Piece of attire fashioned after hunters from a far away place...");

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
            Item.vanity = true;
        }


        public override void EquipFrameEffects(Player player, EquipType type)
        {
            player.back = (sbyte)EquipLoader.GetEquipSlot(Mod, Name, EquipType.Back);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DesertProwlerShirt>().
                AddIngredient(ItemID.BlackInk).
                AddTile(TileID.DyeVat).
                Register();
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class OldHunterPants : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Old Hunter Pants");
            Tooltip.SetDefault("Piece of attire fashioned after hunters from a far away place...");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DesertProwlerPants>().
                AddIngredient(ItemID.BlackInk).
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
