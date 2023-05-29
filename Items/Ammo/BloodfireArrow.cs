using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class BloodfireArrow : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Ammo";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.damage = 29;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 14;
            Item.height = 36;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.knockBack = 3.5f;
            Item.value = Item.sellPrice(copper: 24);
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.shoot = ModContent.ProjectileType<BloodfireArrowProj>();
            Item.shootSpeed = 10f;
            Item.ammo = AmmoID.Arrow;
        }

        public override void AddRecipes()
        {
            CreateRecipe(333).
                AddIngredient<BloodstoneCore>().
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
