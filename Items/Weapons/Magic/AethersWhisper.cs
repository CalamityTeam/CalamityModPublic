using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class AethersWhisper : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetDefaults()
        {
            Item.damage = 504;
            Item.knockBack = 5.5f;
            Item.useTime = Item.useAnimation = 24;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<AetherBeam>();
            Item.mana = 30;
            Item.DamageType = DamageClass.Magic;
            Item.autoReuse = true;

            Item.width = 134;
            Item.height = 44;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = CommonCalamitySounds.LaserCannonSound;

            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PlasmaRod>().
                AddIngredient<Lazhar>().
                AddIngredient<TwistingNether>(3).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
