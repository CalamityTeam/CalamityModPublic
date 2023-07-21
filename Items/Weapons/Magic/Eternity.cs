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
    public class Eternity : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public const int BaseDamage = 840;
        public const int ExplosionDamage = 8400;
        public const int MaxHomers = 40;
        public const int DustID = 16;
        public static readonly Color BlueColor = new Color(34, 34, 160);
        public static readonly Color PinkColor = new Color(169, 30, 184);

        public override void SetDefaults()
        {
            Item.damage = BaseDamage;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 30;
            Item.width = 38;
            Item.height = 40;
            Item.useTime = Item.useAnimation = 120;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0f;

            Item.value = CalamityGlobalItem.Rarity16BuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;

            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<EternityBook>();
            Item.channel = true;
            Item.shootSpeed = 0f;
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
        public override void ModifyTooltips(List<TooltipLine> list)
        {
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip1");
            if (line != null)
                line.OverrideColor = new Color((int)MathHelper.Lerp(156f, 255f, Main.DiscoR / 256f), 108, 251);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Heresy>().
                AddIngredient<DarkPlasma>(20).
                AddIngredient(ItemID.UnicornHorn, 5).
                AddIngredient<ShadowspecBar>(5).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
