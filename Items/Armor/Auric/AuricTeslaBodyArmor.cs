using System.Collections.Generic;
using CalamityMod.Items.Armor.Bloodflare;
using CalamityMod.Items.Armor.GodSlayer;
using CalamityMod.Items.Armor.Silva;
using CalamityMod.Items.Armor.Tarragon;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Auric
{
    [AutoloadEquip(EquipType.Body)]
    public class AuricTeslaBodyArmor : ModItem, ILocalizedModType
    {
        public static int setBonusTooltipNumber = 0; //Set this to zero so the first Shift pressed will increment it to 1.
        public static bool holdingShift = false;
        public static Color tooltipTarragonColor = new(194, 255, 194);
        public static Color tooltipBloodflareColor = new(255, 195, 194);
        public static Color tooltipSilvaColor = new(246, 255, 194);
        public static Color tooltipGodslayerColor = new(204, 194, 255);
        public new string LocalizationCategory => "Items.Armor.PostMoonLord";

        public static float DamageBoost = 0.16f;
        public static int CritBoost = 10;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBoost.ToPercent(), CritBoost);

        public override void Load()
        {
            // All code below runs only if we're not loading on a server
            if (!Main.dedServ)
            {
                // Add equip textures
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/Auric/AuricTeslaBodyArmor_Back", EquipType.Back, this);
            }
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 34;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.defense = 44;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBoost;
            player.GetCritChance<GenericDamageClass>() += CritBoost;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            //This just runs the ModifyTooltips for whatever helmet is equipped if a full auric set is equipped; but runs it on this item's tooltips.
            //This is a simple way to copy the tooltip edits for the set bonuses and have it adapt to helmet type.
            //If we ever add a way for auricSet to be TRUE without a helmet on, this will need to be changed.
            //If the helmet's ModifyTooltips is changed to do more than just the Set Bonus, their Set Bonus modification should be moved into it's own method, and have that called both here and in the helmet's ModifyTooltips.
            //Alternatively, their ModifyTooltips could simply return early if the item ID doesn't match the helmet type.
            if (Main.LocalPlayer.Calamity().auricSet)
            {
                Main.LocalPlayer.armor[0].ModItem?.ModifyTooltips(tooltips);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<GodSlayerChestplate>().
                AddIngredient<BloodflareBodyArmor>().
                AddIngredient<TarragonBreastplate>().
                AddIngredient<AuricBar>(18).
                AddTile<CosmicAnvil>().
                Register();

            CreateRecipe().
                AddIngredient<SilvaArmor>().
                AddIngredient<BloodflareBodyArmor>().
                AddIngredient<TarragonBreastplate>().
                AddIngredient<AuricBar>(18).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
