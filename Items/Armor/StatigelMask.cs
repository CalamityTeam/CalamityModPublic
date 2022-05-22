using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class StatigelMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Statigel Mask");
            Tooltip.SetDefault("10% increased rogue damage and 34% chance to not consume rogue items\n" +
                "7% increased rogue critical strike chance and 5% increased movement speed");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 6; //23
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<StatigelArmor>() && legs.type == ModContent.ItemType<StatigelGreaves>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "When you take over 100 damage in one hit you become immune to damage for an extended period of time\n" +
                    "Grants an extra jump and increased jump height\n" +
                    "12% increased jump speed\n" +
                    "Rogue stealth builds while not attacking and slower while moving, up to a max of 100\n" +
                    "Once you have built max stealth, you will be able to perform a Stealth Strike\n" +
                    "Rogue stealth only reduces when you attack, it does not reduce while moving\n" +
                    "The higher your rogue stealth the higher your rogue damage, crit, and movement speed";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.statigelSet = true;
            modPlayer.statigelJump = true;
            modPlayer.rogueStealthMax += 1f;
            modPlayer.wearingRogueArmor = true;
            Player.jumpHeight += 5;
            player.jumpSpeedBoost += 0.6f;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().rogueAmmoCost *= 0.66f;
            player.Calamity().throwingDamage += 0.1f;
            player.Calamity().throwingCrit += 7;
            player.moveSpeed += 0.05f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PurifiedGel>(5).
                AddIngredient(ItemID.HellstoneBar, 9).
                AddTile<StaticRefiner>().
                Register();
        }
    }
}
