using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class SparkSpreader : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spark Spreader");
            Tooltip.SetDefault("70% chance to not consume gel");
        }

        public override void SetDefaults()
        {
            item.damage = 7;
            item.knockBack = 1f;
            item.ranged = true;
            item.autoReuse = true;
            item.useTime = 10;
            item.useAnimation = 30;
            item.useAmmo = AmmoID.Gel;
            item.shootSpeed = 5f;
            item.shoot = ModContent.ProjectileType<SparkSpreaderFire>();

            item.width = 52;
            item.height = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.UseSound = SoundID.Item34;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Blue;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-4, 0);

        public override bool ConsumeAmmo(Player player) => Main.rand.Next(100) >= 70;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FlareGun);
            recipe.AddIngredient(ItemID.Ruby);
            recipe.AddIngredient(ItemID.Gel, 12);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
