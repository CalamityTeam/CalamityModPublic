using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ShadowboltStaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 56;
            Item.damage = 285;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 72;
            Item.useTime = 8;
            Item.useAnimation = 24;
            Item.reuseDelay = 30;
            Item.useLimitPerAnimation = 3;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Shadowbolt>();
            Item.shootSpeed = 5f;
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ShadowbeamStaff).
                AddIngredient<RuinousSoul>(2).
                AddIngredient<ArmoredShell>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
