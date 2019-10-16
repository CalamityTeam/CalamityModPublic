using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class ExcaliburShortsword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Excalibur Shortsword");
            Tooltip.SetDefault("Don't underestimate the power of shortswords");
        }

        public override void SetDefaults()
        {
            item.useStyle = 3;
            item.useTurn = false;
            item.useAnimation = 10;
            item.useTime = 10;
            item.width = 40;
            item.height = 40;
            item.damage = 55;
            item.melee = true;
            item.knockBack = 5.25f;
            item.UseSound = SoundID.Item1;
            item.useTurn = true;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HallowedBar, 7);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 57);
            }
        }
    }
}
