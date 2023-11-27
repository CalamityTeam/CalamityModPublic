using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Projectiles.Melee.Yoyos;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TheGodsGambit : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetStaticDefaults()
        {
                       ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 38;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.damage = 29;
            Item.knockBack = 3.5f;
            Item.useTime = 21;
            Item.useAnimation = 21;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<GodsGambitYoyo>();
            Item.shootSpeed = 10f;

            Item.rare = ItemRarityID.LightRed;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PurifiedGel>(18).
                AddIngredient<BlightedGel>(18).
                AddTile<StaticRefiner>().
                Register();
        }
    }
}
