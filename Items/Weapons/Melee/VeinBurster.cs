using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class VeinBurster : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vein Burster");
            Tooltip.SetDefault("Fires a blood ball that sticks to tiles and explodes");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.damage = 71;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 37;
            Item.useTurn = true;
            Item.knockBack = 4.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 50;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<BloodBall>();
            Item.shootSpeed = 10f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Vertebrae, 5).
                AddIngredient(ItemID.CrimtaneBar, 5).
                AddIngredient<BloodSample>(15).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
