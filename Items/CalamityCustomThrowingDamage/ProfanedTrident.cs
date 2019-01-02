using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
    public class ProfanedTrident : CalamityDamageItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Infernal Spear");
        }

        public override void SafeSetDefaults()
        {
            item.width = 72;
            item.damage = 1700;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 13;
            item.useStyle = 1;
            item.useTime = 13;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 72;
            item.value = 10000000;
            item.shoot = mod.ProjectileType("ProfanedTrident");
            item.shootSpeed = 28f;
        }
    }
}
