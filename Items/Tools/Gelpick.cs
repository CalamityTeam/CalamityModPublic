using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class Gelpick : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gelpick");
        }

        public override void SetDefaults()
        {
            item.damage = 12;
            item.melee = true;
            item.width = 46;
            item.height = 46;
            item.useTime = 16;
            item.useAnimation = 20;
            item.pick = 100;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 2.5f;
            item.value = Item.buyPrice(0, 12, 0, 0);
            item.rare = ItemRarityID.LightRed;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.useTurn = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<PurifiedGel>(), 15);
            recipe.AddIngredient(ItemID.Gel, 30);
            recipe.AddIngredient(ItemID.HellstoneBar, 5);
            recipe.AddTile(ModContent.TileType<StaticRefiner>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 20);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Slimed, 180);
        }
    }
}
