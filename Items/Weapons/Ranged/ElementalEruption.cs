using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class ElementalEruption : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elemental Eruption");
            Tooltip.SetDefault("90% chance to not consume gel\n" +
                "Fires a spread of rainbow flames");
        }

        public override void SetDefaults()
        {
            item.damage = 77;
            item.ranged = true;
            item.width = 64;
            item.height = 34;
            item.useTime = 14;
            item.useAnimation = 14;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3.5f;
            item.UseSound = SoundID.Item34;
            item.value = CalamityGlobalItem.Rarity11BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<TerraFireGreen2>();
            item.shootSpeed = 10f;
            item.useAmmo = AmmoID.Gel;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int numFirestreams = Main.rand.Next(3, 5);
            for (int index = 0; index < numFirestreams; ++index)
            {
                float SpeedX = speedX + Main.rand.Next(-20, 21) * 0.05f;
                float SpeedY = speedY + Main.rand.Next(-20, 21) * 0.05f;
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override bool ConsumeAmmo(Player player) => Main.rand.NextFloat() > 0.9f;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5);
            recipe.AddIngredient(ModContent.ItemType<TerraFlameburster>());
            recipe.AddIngredient(ModContent.ItemType<Meowthrower>());
            recipe.AddIngredient(ModContent.ItemType<MepheticSprayer>());
            recipe.AddIngredient(ModContent.ItemType<BrimstoneFlamesprayer>());
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
