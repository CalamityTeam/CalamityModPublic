using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class StormRuler : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Ruler");
            Tooltip.SetDefault("Only a storm can fell a greatwood\n" +
                "Fires beams that generate tornadoes on death\n" +
                "Tornadoes suck enemies in");
        }

        public override void SetDefaults()
        {
            item.width = 74;
            item.damage = 80;
            item.melee = true;
            item.useAnimation = 25;
            item.useTime = 25;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 6.25f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 82;
            item.value = CalamityGlobalItem.Rarity10BuyPrice;
            item.rare = ItemRarityID.Red;
            item.shoot = ModContent.ProjectileType<StormRulerProj>();
            item.shootSpeed = 20f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<StormSaber>());
            recipe.AddIngredient(ModContent.ItemType<WindBlade>());
            recipe.AddIngredient(ModContent.ItemType<CoreofCinder>(), 3);
            recipe.AddIngredient(ItemID.FragmentSolar, 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int num250 = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, 187, (float)(player.direction * 2), 0f, 150, default, 1.3f);
                Main.dust[num250].velocity *= 0.2f;
            }
        }
    }
}
