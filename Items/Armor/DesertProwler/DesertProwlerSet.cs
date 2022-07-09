using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.DesertProwler
{
    [AutoloadEquip(EquipType.Head)]
    public class DesertProwlerHat : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Desert Prowler Hat");
            Tooltip.SetDefault("4% increased ranged critical strike chance and 20% chance to not consume ammo");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 1; //6
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<DesertProwlerShirt>() && legs.type == ModContent.ItemType<DesertProwlerPants>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Ranged attacks deal an extra 1 flat damage\n" +
            "Ranged crits can rarely whip up a sandstorm";
            player.Calamity().desertProwler = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<RangedDamageClass>() += 4;
            player.ammoCost80 = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DesertFeather>(2).
                AddIngredient(ItemID.Silk, 8).
                AddTile(TileID.Loom).
                Register();
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class DesertProwlerShirt : ModItem, IBulkyArmor
    {
        public string BulkTexture => "CalamityMod/Items/Armor/DesertProwler/DesertProwlerShirt_Bulk";

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Desert Prowler Shirt");
            Tooltip.SetDefault("5% increased ranged critical strike chance");

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
            Item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<RangedDamageClass>() += 5;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DesertFeather>(3).
                AddIngredient(ItemID.Silk, 10).
                AddTile(TileID.Loom).
                Register();
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class DesertProwlerPants : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Desert Prowler Pants");
            Tooltip.SetDefault("10% increased movement speed and immunity to the Mighty Wind debuff");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 2;
        }

        public override void UpdateEquip(Player player)
        {
            player.buffImmune[BuffID.WindPushed] = true;
            player.moveSpeed += 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DesertFeather>().
                AddIngredient(ItemID.Silk, 5).
                AddTile(TileID.Loom).
                Register();
        }
    }

}
