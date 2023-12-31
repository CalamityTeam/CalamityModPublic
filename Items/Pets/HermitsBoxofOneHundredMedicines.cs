﻿using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    [LegacyName("IbarakiBox")]
    public class HermitsBoxofOneHundredMedicines : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Pets";
        public override void SetDefaults()
        {
            Item.damage = 0;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.width = 36;
            Item.height = 30;
            Item.UseSound = SoundID.Item3;
            Item.shoot = ModContent.ProjectileType<ThirdSage>();
            Item.buffType = ModContent.BuffType<ThirdSageBuff>();

            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.LightRed;
            Item.Calamity().devItem = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.itemAnimation == (int)(player.itemAnimationMax * 0.5) && Main.myPlayer == player.whoAmI)
            {
                if (player.altFunctionUse == 2)
                {
                    if (!player.Calamity().healToFull)
                    {
                        player.Calamity().healToFull = true;
                        string key = "Mods.CalamityMod.Misc.ThirdSageBlessingText";
                        Color messageColor = Color.Violet;
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else
                    {
                        player.Calamity().healToFull = false;
                        string key2 = "Mods.CalamityMod.Misc.ThirdSageBlessingText2";
                        Color messageColor2 = Color.Violet;
                        Main.NewText(Language.GetTextValue(key2), messageColor2);
                    }
                }
                else
                {
                    if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
                    {
                        player.AddBuff(Item.buffType, 3600, true);
                    }
                }
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => false;
    }
}
