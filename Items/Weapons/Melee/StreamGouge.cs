using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Spears;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class StreamGouge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stream Gouge");
            Tooltip.SetDefault("Fires an essence spear clone\n" +
                "Ignores immunity frames");
        }

        public override void SetDefaults()
        {
            item.width = 100;
            item.damage = 350;
            item.melee = true;
            item.noMelee = true;
            item.useTurn = true;
            item.noUseGraphic = true;
            item.useAnimation = 19;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 19;
            item.knockBack = 9.75f;
            item.UseSound = SoundID.Item20;
            item.autoReuse = true;
            item.height = 100;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<StreamGougeProj>();
            item.shootSpeed = 15f;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.GetTexture("CalamityMod/Items/Weapons/Melee/StreamGougeGlow"));
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 14);
            recipe.AddRecipeGroup("NForEE", 7);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
