using System.Collections.Generic;
using System.IO;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityMod.Items.Accessories
{
    public class DeificAmulet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static readonly int MaxBonusIFrames = 30;
        public static int StarDamage => CalamityUtils.ScaleWithDifficulty(130);

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.rare = ItemRarityID.Cyan;
            Item.value = CalamityGlobalItem.RarityCyanBuyPrice;
            Item.accessory = true;
        }

        #region Toggleable Panic Necklace

        bool panicNecklaceEnabled = true;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.FindAndReplace("[TOGGLE]", panicNecklaceEnabled ? this.GetLocalizedValue("ToggleEffect") : "");
        }
        public override bool CanRightClick() => Main.keyState.PressingShift();
        public override void RightClick(Player player)
        {
            panicNecklaceEnabled = !panicNecklaceEnabled;
            Item.NetStateChanged();
        }
        public override bool ConsumeItem(Player player) => false;
        public override void SaveData(TagCompound tag)
        {
            tag.Add("panic", panicNecklaceEnabled);
        }

        public override void LoadData(TagCompound tag)
        {
            panicNecklaceEnabled = tag.GetBool("panic");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(panicNecklaceEnabled);
        }

        public override void NetReceive(BinaryReader reader)
        {
            panicNecklaceEnabled = reader.ReadBoolean();
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            CalamityUtils.DrawInventoryDot(spriteBatch, position, new Vector2(16, 16) * Main.inventoryScale, panicNecklaceEnabled);
        }
        #endregion

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.longInvince = true;
            modPlayer.dAmulet = true;
            player.panic = panicNecklaceEnabled;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.StarVeil).
                AddIngredient(ItemID.SweetheartNecklace).
                AddIngredient<AstralBar>(10).
                AddTile(TileID.TinkerersWorkbench).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.BeeCloak).
                AddIngredient(ItemID.CrossNecklace).
                AddIngredient(ItemID.PanicNecklace).
                AddIngredient<AstralBar>(10).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
