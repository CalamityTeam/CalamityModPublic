using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Eternity : ModItem
    {
        public const int BaseDamage = 840;
        public const int ExplosionDamage = 8400;
        public const int MaxHomers = 40;
        public const int DustID = 16;
        public static readonly Color BlueColor = new Color(34, 34, 160);
        public static readonly Color PinkColor = new Color(169, 30, 184);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eternity");
            Tooltip.SetDefault("Hexes a possible nearby enemy, trapping them in a brilliant display of destruction\n" +
                "This line is modified in ModifyTooltips");
        }

        public override void SetDefaults()
        {
            item.damage = BaseDamage;
            item.magic = true;
            item.mana = 30;
            item.width = 38;
            item.height = 40;
            item.useTime = item.useAnimation = 120;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 0f;

            item.value = CalamityGlobalItem.Rarity16BuyPrice;
            item.Calamity().customRarity = CalamityRarity.HotPink;
            item.Calamity().devItem = true;

            item.autoReuse = true;
            item.noUseGraphic = true;
            item.shoot = ModContent.ProjectileType<EternityBook>();
            item.channel = true;
            item.shootSpeed = 0f;
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var tt2 = tooltips.FirstOrDefault(x => x.Name == "Tooltip1" && x.mod == "Terraria");
            tt2.text = $"[" + DisoHex + "There's pictures of ponies in the book]";
        }
        public static string DisoHex => "c/" +
            ((int)(156 + Main.DiscoR * 99f / 255f)).ToString("X2")
            + 108.ToString("X2") + 251.ToString("X2") + ":";
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SeethingDischarge>());
            recipe.AddIngredient(ModContent.ItemType<SlitheringEels>());
            recipe.AddIngredient(ModContent.ItemType<GammaFusillade>());
            recipe.AddIngredient(ModContent.ItemType<PrimordialAncient>());
            recipe.AddIngredient(ModContent.ItemType<Heresy>());
            recipe.AddIngredient(ModContent.ItemType<DarkPlasma>(), 20);
            recipe.AddIngredient(ItemID.UnicornHorn, 5);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
