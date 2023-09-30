using CalamityMod.ExtraJumps;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Statigel
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("StatigelCap")]
    public class StatigelHeadMagic : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PreHardmode";
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 5; //22
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<StatigelArmor>() && legs.type == ModContent.ItemType<StatigelGreaves>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = CalamityUtils.GetTextValueFromModItem<StatigelArmor>("CommonSetBonus");
            var modPlayer = player.Calamity();
            modPlayer.statigelSet = true;
            player.GetJumpState<StatigelJump>().Enable();
            Player.jumpHeight += 5;
            player.jumpSpeedBoost += 0.6f;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<MagicDamageClass>() += 0.1f;
            player.GetCritChance<MagicDamageClass>() += 7;
            player.manaCost *= 0.9f;
            player.statManaMax2 += 30;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PurifiedGel>(5).
                AddIngredient<BlightedGel>(5).
                AddTile<StaticRefiner>().
                Register();
        }
    }
}
