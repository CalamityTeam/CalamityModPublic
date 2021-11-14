using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class AerialHamaxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aerial Hamaxe");
        }

        public override void SetDefaults()
        {
            item.damage = 20;
            item.knockBack = 7f;
            item.useTime = 16;
            item.useAnimation = 26;
            item.hammer = 70;
            item.axe = 125 / 5;
            item.tileBoost += 1;

            item.melee = true;
            item.width = 44;
            item.height = 44;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 6);
            recipe.AddIngredient(ItemID.SunplateBlock, 5);
            recipe.AddTile(TileID.SkyMill);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 59);
            }
        }
    }
}
