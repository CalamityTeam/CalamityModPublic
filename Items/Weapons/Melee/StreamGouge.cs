using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Spears;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class StreamGouge : ModItem
    {
        public const int SpinTime = 45;

        public const int SpearFireTime = 24;

        public const int PortalLifetime = 30;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stream Gouge");
            Tooltip.SetDefault("Summons a portal that the spear crosses through\n" +
                "Shortly after going through the portal, portals appear near the mouse that release copies of the spear's cutting edge\n" +
                "Enemies hit by the copies create lacerations in space, revealing a cosmic background");
        }

        public override void SetDefaults()
        {
            item.width = 100;
            item.height = 100;
            item.damage = 300;
            item.melee = true;
            item.noMelee = true;
            item.useTurn = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.useAnimation = 19;
            item.useTime = 19;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 9.75f;
            item.UseSound = SoundID.Item20;
            item.autoReuse = true;
            item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            item.rare = ItemRarityID.Red;
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
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 12);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
