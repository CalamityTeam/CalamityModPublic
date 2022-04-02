using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
namespace CalamityMod.Items.Fishing.FishingRods
{
    public class ChaoticSpreadRod : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rift Reeler");
            Tooltip.SetDefault("Fires three to five lines at once. Can fish in lava.\n" +
                "The battlefield is a scene of constant chaos.\n" + //Napoleon Bonaparte quote reference
                "The winner will be the one who controls that chaos, both the pole and the fish.");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 28;
            item.useAnimation = 8;
            item.useTime = 8;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.fishingPole = 45;
            item.shootSpeed = 17f;
            item.shoot = ModContent.ProjectileType<ChaoticSpreadBobber>();
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int index = 0; index < Main.rand.Next(3,6); ++index) //3 to 5 bobbers
            {
                float SpeedX = speedX + Main.rand.NextFloat(-3.75f, 3.75f);
                float SpeedY = speedY + Main.rand.NextFloat(-3.75f, 3.75f);
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, 0, 0f, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HotlineFishingHook);
            recipe.AddIngredient(ModContent.ItemType<CruptixBar>(), 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
