using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class BlushieStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Staff of Blushie");
            Tooltip.SetDefault("Hold your mouse, wait, wait, wait, and put your trust in the power of blue magic");
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
            CreateRecipe(1).AddIngredient(ItemID.SapphireStaff).AddIngredient(ModContent.ItemType<Phantoplasm>(), 10).AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5).AddTile(ModContent.TileType<DraedonsForge>()).Register();
        }
    }
}
