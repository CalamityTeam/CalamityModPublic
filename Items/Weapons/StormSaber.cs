using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class StormSaber : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Saber");
        }

        public override void SetDefaults()
        {
            item.width = 58;
            item.damage = 42;
            item.melee = true;
            item.useAnimation = 23;
            item.useTime = 23;
            item.useTurn = true;
            item.useStyle = 1;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 68;
            item.value = 300000;
            item.rare = 5;
            item.shoot = mod.ProjectileType("StormBeam");
            item.shootSpeed = 12f;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.Next(5) == 0)
            {
                int num250 = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, 187, (float)(player.direction * 2), 0f, 150, default(Color), 1.3f);
                Main.dust[num250].velocity *= 0.2f;
            }
        }
    }
}
