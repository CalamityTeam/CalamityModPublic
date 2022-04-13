using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class GodSlayerHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("God Slayer Helmet");
            Tooltip.SetDefault("14% increased ranged damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 75, 0, 0);
            Item.defense = 35; //96
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<GodSlayerChestplate>() && legs.type == ModContent.ItemType<GodSlayerLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.godSlayer = true;
            modPlayer.godSlayerRanged = true;
            string hotkey = CalamityKeybinds.GodSlayerDashHotKey.TooltipHotkeyString();
            player.setBonus = "Allows you to dash for an immense distance in 8 directions\n" +
                "Press " + hotkey + " while holding down the movement keys in the direction you want to dash\n" +
                "Enemies you dash through take massive damage\n" +
                "During the dash you are immune to most debuffs\n" +
                "The dash has a 35 second cooldown\n" +
                "You fire a god killer shrapnel round while firing ranged weapons every 2.5 seconds";

            if (modPlayer.godSlayerDashHotKeyPressed)
            {
                modPlayer.dashMod = 9;
                player.dash = 0;
            }
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Ranged) += 0.14f;
            player.GetCritChance(DamageClass.Ranged) += 14;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CosmiliteBar>(14).
                AddIngredient<AscendantSpiritEssence>(2).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
