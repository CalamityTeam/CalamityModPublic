using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Demonshade
{
    [AutoloadEquip(EquipType.Body)]
    public class DemonshadeBreastplate : ModItem, IDrawArmOverShoulderpad
    {
        public string FrontArmTexture => "CalamityMod/Items/Armor/Demonshade/DemonshadeBreastplate_Arms";


        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Demonshade Breastplate");
            Tooltip.SetDefault("20% increased melee speed, 15% increased damage and critical strike chance\n" +
                "Enemies take ungodly damage when they touch you\n" +
                "Increased max life and mana by 200\n" +
                "Standing still lets you absorb the shadows and boost your life regen");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 50;
            Item.value = CalamityGlobalItem.Rarity16BuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;
        }

        public override void UpdateEquip(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.shadeRegen = true;
            player.thorns += 100f;
            player.statLifeMax2 += 200;
            player.statManaMax2 += 200;
            player.GetDamage<GenericDamageClass>() += 0.15f;
            player.GetCritChance<GenericDamageClass>() += 15;
            player.GetAttackSpeed<MeleeDamageClass>() += 0.2f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ShadowspecBar>(18).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
