using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class MortarRound : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 99;
            DisplayName.SetDefault("Mortar Round");
            Tooltip.SetDefault("Large blast radius. Will destroy tiles\n" +
                "Used by normal guns");
        }

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 20;
            Item.height = 14;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.knockBack = 7.5f;
            Item.value = Item.sellPrice(copper: 20);
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
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
