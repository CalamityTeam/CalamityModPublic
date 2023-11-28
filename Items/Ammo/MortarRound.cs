using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class MortarRound : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
                   }

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 20;
            Item.height = 14;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.knockBack = 7.5f;
            Item.value = Item.sellPrice(copper: 20);
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.ammo = AmmoID.Bullet;
            Item.shoot = ModContent.ProjectileType<MortarRoundProj>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(100).
                AddIngredient(ItemID.RocketIV, 100).
                AddIngredient<UelibloomBar>().
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
