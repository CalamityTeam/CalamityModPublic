using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Sounds;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class ElementalBlaster : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.damage = 67;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 104;
            Item.height = 42;
            Item.useTime = 2;
            Item.useAnimation = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.75f;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = CommonCalamitySounds.PlasmaBoltSound;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<RainbowBlast>();
            Item.shootSpeed = 18f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SpeedBlaster>().
                AddIngredient(ItemID.LunarBar, 5).
                AddIngredient<LifeAlloy>(5).
                AddIngredient<GalacticaSingularity>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
