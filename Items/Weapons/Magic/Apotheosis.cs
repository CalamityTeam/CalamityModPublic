using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Apotheosis : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Apotheosis");
            Tooltip.SetDefault("Eat worms\n" +
                "Unleashes interdimensional projection magic");
        }

        public override void SetDefaults()
        {
            item.damage = 77;
            item.magic = true;
            item.mana = 42;
            item.width = 30;
            item.height = 34;
            item.useTime = item.useAnimation = 177;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTurn = false;
            item.noMelee = true;
            item.knockBack = 6.9f;

            item.value = CalamityGlobalItem.Rarity16BuyPrice;
            item.Calamity().customRarity = CalamityRarity.HotPink;
            item.Calamity().devItem = true;

            item.UseSound = SoundID.Item92;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<ApotheosisWorm>();
            item.shootSpeed = 42.0f;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.GetTexture("CalamityMod/Items/Weapons/Magic/ApotheosisGlow"));
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SpellTome);
            recipe.AddIngredient(ModContent.ItemType<CosmicDischarge>());
            recipe.AddIngredient(ModContent.ItemType<StaffoftheMechworm>(), 2);
            recipe.AddIngredient(ModContent.ItemType<Excelsus>(), 2);
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 11);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 33);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
