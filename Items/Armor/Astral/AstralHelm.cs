using System.Collections.Generic;
using System.IO;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityMod.Items.Armor.Astral
{
    [AutoloadEquip(EquipType.Head)]
    public class AstralHelm : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Hardmode";

        public static float DamageBoost = 0.05f;
        public static int CritBoost = 15;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBoost.ToPercent(), CritBoost);

        // Set Bonus
        public static int SetBonusMinionSlotBoost = 3;
        public static float SetBonusDamageBoost = 0.1f;
        public static int SetBonusCritBoost = 10; // NOTE: Tooltip shares this number with damage % as they're equal
        public static int StarRainCooldown = CalamityUtils.SecondsToFrames(1);
        public static int StarDamage = 120;

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityCyanBuyPrice;
            Item.rare = ItemRarityID.Cyan;
            Item.defense = 17; //63
        }
        #region Toggleable Omniscience effect

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

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AstralBreastplate>() && legs.type == ModContent.ItemType<AstralLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = this.GetLocalization("SetBonus").Format(SetBonusMinionSlotBoost, SetBonusDamageBoost.ToPercent(), StarRainCooldown.FramesToSeconds());
            var modPlayer = player.Calamity();
            modPlayer.astralStarRain = true;
            player.maxMinions += SetBonusMinionSlotBoost;
            player.GetDamage<GenericDamageClass>() += SetBonusDamageBoost;
            player.GetCritChance<GenericDamageClass>() += SetBonusCritBoost;
            player.Calamity().wearingRogueArmor = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBoost;
            player.GetCritChance<GenericDamageClass>() += CritBoost;
            var modPlayer = player.Calamity();
            modPlayer.omniscience = toggleEnabled;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!toggleEnabled)
                tooltips.RemoveAll(x => x.Name == "Tooltip1");
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AstralBar>(8).
                AddIngredient(ItemID.MeteoriteBar, 6).
                AddTile(TileID.LunarCraftingStation).
                SortBeforeFirstRecipesOf(ModContent.ItemType<AstralBreastplate>()).
                Register();
        }
    }
}
