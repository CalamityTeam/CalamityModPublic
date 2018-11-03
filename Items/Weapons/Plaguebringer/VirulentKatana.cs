using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Plaguebringer
{
    public class VirulentKatana : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Virulence");
        }

        public override void SetDefaults()
        {
            item.width = 42;
            item.damage = 96;
            item.melee = true;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.useTime = 15;
            item.useTurn = true;
            item.knockBack = 5.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 52;
            item.maxStack = 1;
            item.value = 755000;
            item.rare = 8;
            item.shoot = mod.ProjectileType("PlagueDust");
            item.shootSpeed = 9f;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(mod.BuffType("Plague"), 240);
        }
    }
}
