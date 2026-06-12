using System.Collections.Generic;
using System.Linq;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.BaseItems;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Earth : CustomUseProjItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<MiracleBlight>()];
        }
        public override void SetDefaults()
        {
            Item.width = 186;
            Item.height = 186;
            Item.damage = 4200;
            Item.DamageType = TrueMeleeDamageClass.Instance;
            Item.useAnimation = 42;
            Item.useTime = 42;
            Item.useTurn = true;
            Item.knockBack = 15f;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;

            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<EarthHoldout>();
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
        }
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/EarthGlow").Value);
        }
        public override void ModifyTooltips(List<TooltipLine> list)
        {
            List<Color> earthColors = new List<Color>()
            {
                Color.OrangeRed,
                Color.MediumTurquoise,
                Color.LimeGreen
            };

            int colorIndex = (int)(Main.GlobalTimeWrappedHourly / 2 % earthColors.Count);
            Color currentColor = earthColors[colorIndex];
            Color nextColor = earthColors[(colorIndex + 1) % earthColors.Count];
            Color earthTooltipColor = Color.Lerp(currentColor, nextColor, Main.GlobalTimeWrappedHourly % 2f > 1f ? 1f : Main.GlobalTimeWrappedHourly % 1f);

            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip3");
            if (line != null)
                line.OverrideColor = Color.Lerp(earthTooltipColor, Color.White, 0.5f);
        }
        public override bool MeleePrefix() => true;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<GrandGuardian>().
                AddIngredient<StellarStriker>().
                AddIngredient<ShadowspecBar>(5).
                AddIngredient<LifeAlloy>(5).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
