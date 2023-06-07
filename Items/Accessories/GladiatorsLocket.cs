using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class GladiatorsLocket : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 36;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.defense = 5;
            Item.accessory = true;
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            if (player.Calamity().gladiatorSword)
                return false;

            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            float damageIncrease = 0.2f;
            float moveSpeedIncrease = 0.2f;
            player.Calamity().gladiatorSword = true;
            player.GetDamage<GenericDamageClass>() += damageIncrease - (damageIncrease * player.statLife / player.statLifeMax2);
            player.moveSpeed += moveSpeedIncrease - (moveSpeedIncrease * player.statLife / player.statLifeMax2);
        }
    }
}
