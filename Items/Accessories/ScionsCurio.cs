using System.Collections.Generic;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("RustyMedallion")]

    public class ScionsCurio : ModItem, ILocalizedModType
    {
        public static int postHitDamage = 20;
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 38;
            Item.rare = ItemRarityID.Blue;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.scionsCurio = true;
            modPlayer.scionsCurioVisuals = !hideVisual;

            if (player.ownedProjectileCounts[ProjectileType<ScionsCurioMini>()] < 1 && !player.dead)
            {
                Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, Vector2.Zero, ProjectileType<ScionsCurioMini>(), 0, 0f, player.whoAmI);
            }
        }
        public override void ModifyTooltips(List<TooltipLine> list)
        {
            Player player = Main.LocalPlayer;
            if (Main.LocalPlayer != null)
                list.FindAndReplace("[DAMAGE]", ((int)(player.Calamity().scionsCurioDebuffDamage / 2)).ToString() + " DPS");
        }
    }
}
