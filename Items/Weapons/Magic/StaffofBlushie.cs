using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    [LegacyName("BlushieStaff")]
    public class StaffofBlushie : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Staff of Blushie");
            Tooltip.SetDefault("Hold your mouse, wait, wait, wait, and put your trust in the power of blue magic");
            SacrificeTotal = 1;
        }

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
            Item.Calamity().customRarity = CalamityRarity.HotPink;
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
                AddIngredient<Phantoplasm>(10).
                AddIngredient<ShadowspecBar>(5).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
