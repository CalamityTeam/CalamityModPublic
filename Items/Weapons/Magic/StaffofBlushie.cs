using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    [LegacyName("BlushieStaff")]
    public class StaffofBlushie : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetDefaults()
        {
            Item.width = Item.height = 48;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.channel = true;
            Item.noMelee = true;
            Item.damage = 1;
            Item.knockBack = 1f;
            Item.autoReuse = false;
            Item.useTurn = false;
            Item.DamageType = DamageClass.Magic;

            Item.value = CalamityGlobalItem.Rarity16BuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;

            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<BlushieStaffProj>();
            Item.mana = 200;
            Item.shootSpeed = 0f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SapphireStaff).
                AddIngredient<Polterplasm>(10).
                AddIngredient<ShadowspecBar>(5).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
