using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.TitanHeart
{
    [AutoloadEquip(EquipType.Body)]
    public class TitanHeartMantle : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Titan Heart Mantle");
            Tooltip.SetDefault("45% chance to not consume rogue items\n" +
            "5% boosted rogue knockback but 15% lowered rogue attack speed");

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
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 17;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().titanHeartMantle = true;
            player.Calamity().rogueAmmoCost *= 0.55f;
            // 15% attack speed penalty
            player.GetAttackSpeed<ThrowingDamageClass>() -= 0.15f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AstralMonolith>(20).
                AddIngredient<Materials.TitanHeart>().
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
