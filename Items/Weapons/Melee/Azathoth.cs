using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Yoyos;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Azathoth : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Azathoth");
            Tooltip.SetDefault("Fires cosmic orbs that blast nearby enemies with lasers\n" +
            "A very agile yoyo\n" +
            "Destroy the universe in the blink of an eye");
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 54;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.damage = 80;
            Item.knockBack = 6f;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<AzathothYoyo>();
            Item.shootSpeed = 16f;

            Item.autoReuse = true;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;
            Item.value = CalamityGlobalItem.Rarity16BuyPrice;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Terrarian).
                AddIngredient<CoreofCalamity>(3).
                AddIngredient<ShadowspecBar>(5).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
