using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class BeastialPickaxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beastial Pickaxe");
        }

        public override void SetDefaults()
        {
            item.damage = 35;
            item.knockBack = 4.5f;
            item.useTime = 5;
            item.useAnimation = 15;
            item.pick = 200;

            item.melee = true;
            item.width = 48;
            item.height = 46;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DraedonBar>(), 7);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 61);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Venom, 300);
        }
    }
}
