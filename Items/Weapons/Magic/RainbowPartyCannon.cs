using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class RainbowPartyCannon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public static readonly Color[] ColorSet = new Color[]
        {
            new Color(188, 192, 193), // White
            new Color(157, 100, 183), // Purple
            new Color(249, 166, 77), // Honey-ish orange
            new Color(255, 105, 234), // Pink
            new Color(67, 204, 219), // Sky blue
            new Color(249, 245, 99), // Bright yellow
            new Color(236, 168, 247), // Purplish pink
        };
        public override void SetDefaults()
        {
            Item.damage = 225;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 25;
            Item.width = 52;
            Item.height = 30;
            Item.crit += 4;
            Item.useTime = Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<RainbowPartyCannonProjectile>();
            Item.channel = true;
            Item.shootSpeed = 20f;
            Item.Calamity().devItem = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip0");
            if (line != null)
                line.OverrideColor = new Color((int)MathHelper.Lerp(156f, 255f, Main.DiscoR / 256f), 108, 251);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ConfettiCannon).
                AddIngredient<CosmicRainbow>().
                AddIngredient(ItemID.Celeb2).
                AddIngredient(ItemID.FlaskofParty, 5).
                AddIngredient(ItemID.SoulofLight, 25).
                AddIngredient(ItemID.Confetti, 50).
                AddIngredient<ShadowspecBar>(5).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
