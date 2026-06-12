using System.Collections.Generic;
using System.IO;
using CalamityMod.Items.Armor.Bloodflare;
using CalamityMod.Items.Armor.GodSlayer;
using CalamityMod.Items.Armor.Silva;
using CalamityMod.Items.Armor.Tarragon;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityMod.Items.Armor.Auric
{
    [AutoloadEquip(EquipType.Legs)]
    public class AuricTeslaCuisses : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PostMoonLord";

        public static float DamageBoost = 0.12f;
        public static int CritBoost = 10;
        public static float MoveSpeedBoost = 0.1f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBoost.ToPercent(), CritBoost, MoveSpeedBoost.ToPercent());

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.defense = 42;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
        }
        #region Toggleable Magic Carpet

        bool toggleEnabled = true;

        public override bool CanRightClick() => Main.keyState.PressingShift();
        public override void RightClick(Player player)
        {
            toggleEnabled = !toggleEnabled;
            Item.NetStateChanged();
        }
        public override bool ConsumeItem(Player player) => false;
        public override void SaveData(TagCompound tag)
        {
            tag.Add("toggleEffect", toggleEnabled);
        }

        public override void LoadData(TagCompound tag)
        {
            toggleEnabled = tag.GetBool("toggleEffect");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(toggleEnabled);
        }

        public override void NetReceive(BinaryReader reader)
        {
            toggleEnabled = reader.ReadBoolean();
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            CalamityUtils.DrawInventoryDot(spriteBatch, position, new Vector2(16, 16) * Main.inventoryScale, toggleEnabled);
        }
#endregion

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBoost;
            player.GetCritChance<GenericDamageClass>() += CritBoost;
            player.moveSpeed += MoveSpeedBoost;
            player.carpet = toggleEnabled;
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
            
            if (!toggleEnabled)
                tooltips.RemoveAll(x => x.Name == "Tooltip2");
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<GodSlayerLeggings>().
                AddIngredient<BloodflareCuisses>().
                AddIngredient<TarragonLeggings>().
                AddIngredient(ItemID.FlyingCarpet).
                AddIngredient<AuricBar>(15).
                AddTile<CosmicAnvil>().
                Register();

            CreateRecipe().
                AddIngredient<SilvaLeggings>().
                AddIngredient<BloodflareCuisses>().
                AddIngredient<TarragonLeggings>().
                AddIngredient(ItemID.FlyingCarpet).
                AddIngredient<AuricBar>(15).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
