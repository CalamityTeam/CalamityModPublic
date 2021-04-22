using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
	public class SoulPiercer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Piercer");
            Tooltip.SetDefault("Casts a powerful ray that summons extra rays on enemy hits");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 128;
            item.magic = true;
            item.mana = 19;
            item.width = 64;
            item.height = 64;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 8f;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = ItemRarityID.Red;
            item.UseSound = SoundID.Item73;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SoulPiercerBeam>();
            item.shootSpeed = 6f;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        /*public override Vector2? HoldoutOrigin()
        {
            return new Vector2(15, 15);
        }*/

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
			item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.GetTexture("CalamityMod/Items/Weapons/Magic/SoulPiercerGlow"));
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 12);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
