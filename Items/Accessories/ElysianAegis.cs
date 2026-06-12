using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class ElysianAegis : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public bool HasFlavorTooltip => true;
        public Color? TooltipExtensionColor => new(195, 223, 255);

        public const int ShieldSlamDamage = 500;
        public const float ShieldSlamKnockback = 12f;
        public const int ShieldSlamIFrames = 12;

        public const int RamExplosionDamage = 100;
        public const float RamExplosionKnockback = 15f;
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<HolyFlames>()];
        }

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 42;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.defense = 5;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.DashID = ElysianAegisDash.ID;
            player.dashType = 0;
            player.noKnockback = true;
        }
    }
}
