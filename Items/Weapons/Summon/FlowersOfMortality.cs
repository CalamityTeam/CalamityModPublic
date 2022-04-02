using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class FlowersOfMortality : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flowers of Mortality");
            Tooltip.SetDefault("Summons five rainbow flowers over your head\n" +
                               "The combined flowers consume two and a half minion slots");
        }

        public override void SetDefaults()
        {
            item.damage = 140;
            item.summon = true;
            item.mana = 10;
            item.width = 36;
            item.height = 36;
            item.useTime = item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = CalamityGlobalItem.Rarity11BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.UseSound = SoundID.Item44;
            item.shoot = ModContent.ProjectileType<FlowersOfMortalityPetal>();
            item.shootSpeed = 10f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] < 5; //If you already have all 5, no need to resummon

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            CalamityUtils.KillShootProjectiles(false, type, player);
            for (int i = 0; i < 5; i++)
            {
                Projectile blossom = Projectile.NewProjectileDirect(player.Center, Vector2.Zero, type, damage, knockBack, player.whoAmI, 0f, 0f);
                blossom.ai[0] = MathHelper.TwoPi * i / 5f;
                blossom.rotation = blossom.ai[0];
            }
            return false;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<WitherBlossomsStaff>());
            recipe.AddIngredient(ModContent.ItemType<ViralSprout>());
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5);
            recipe.AddIngredient(ModContent.ItemType<BarofLife>(), 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
