using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class BloodfireBullet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodfire Bullet");
            Tooltip.SetDefault("Accelerates your life regeneration on hit\n" + "Deals bonus damage based on your current life regeneration");
        }

        public override void SetDefaults()
        {
            Item.damage = 26;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 14;
            Item.height = 30;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.knockBack = 4.5f;
            Item.value = Item.sellPrice(copper: 24);
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.shoot = ModContent.ProjectileType<BloodfireBulletProj>();
            Item.shootSpeed = 4.8f;
            Item.ammo = ItemID.MusketBall;
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
