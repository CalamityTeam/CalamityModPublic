using System.Collections.Generic;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Starfleet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.width = 76;
            Item.height = 36;
            Item.damage = 3170;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 70;
            Item.useAnimation = 70;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 15f;
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<StarfleetStar>();
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.FallenStar;
        }
        // Holdout projectile is spawned when holding the item, so using the item does nothing
        public override bool CanUseItem(Player player) => false;
        public override void ModifyTooltips(List<TooltipLine> list)
        {
            Player Owner = Main.LocalPlayer;
            if (Owner is null)
                return;
            float rate = (Main.GlobalTimeWrappedHourly * 3);
            List<Color> eColors = new List<Color>()
            {
                new Color(146, 255, 211),
                new Color(222, 225, 146),
                new Color(255, 233, 146)
            };
            int colorIndex = (int)(rate / 2 % eColors.Count);
            Color currentColor = eColors[colorIndex];
            Color nextColor = eColors[(colorIndex + 1) % eColors.Count];
            Color eTooltipColor = Color.Lerp(currentColor, nextColor, rate % 2f >= 1f ? 1f : rate % 1f);

            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip7");
            if (line != null)
                line.OverrideColor = Color.Lerp(eTooltipColor, Color.White, 0.2f);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SuperStarCannon).
                AddIngredient<RuinousSoul>(4).
                AddIngredient(ItemID.FallenStar, 15).
                AddIngredient(ItemID.FragmentStardust, 5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
