using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Rarities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.LunicCorps
{
    [AutoloadEquip(EquipType.Head)]
    public class LunicCorpsHelmet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Hardmode";

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.defense = 14;
            Item.rare = ItemRarityID.Cyan;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<LunicCorpsVest>() && legs.type == ModContent.ItemType<LunicCorpsBoots>();
        }

        public override void UpdateArmorSet(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.lunicCorpsSet = true;
            player.setBonus = this.GetLocalizedValue("SetBonus");
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<RangedDamageClass>() += 0.12f;
            player.nightVision = true;
            player.detectCreature = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ChlorophyteBar, 6).
                AddIngredient<AstralBar>(6).
                AddIngredient(ItemID.Glass, 20).
                AddIngredient(ItemID.NightVisionHelmet).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
