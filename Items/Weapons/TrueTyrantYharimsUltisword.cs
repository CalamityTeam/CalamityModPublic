using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class TrueTyrantYharimsUltisword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("True Tyrant's Ultisword");
            Tooltip.SetDefault("Contains the essence of a forgotten age\n50% chance to give the player the tyrant's fury buff on enemy hits\nThis buff increases melee damage, speed, and crit chance by 30%");
        }

        public override void SetDefaults()
        {
            item.width = 86;
            item.damage = 165;
            item.melee = true;
            item.useAnimation = 18;
            item.useStyle = 1;
            item.useTime = 18;
            item.useTurn = true;
            item.knockBack = 7.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 86;
            item.value = 5250000;
            item.shoot = mod.ProjectileType("BlazingPhantomBlade");
            item.shootSpeed = 12f;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(0, 255, 200);
                }
            }
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            switch (Main.rand.Next(3))
            {
                case 0: type = mod.ProjectileType("BlazingPhantomBlade"); break;
                case 1: type = mod.ProjectileType("HyperBlade"); break;
                case 2: type = mod.ProjectileType("SunlightBlade"); break;
                default: break;
            }
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, Main.myPlayer);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "TyrantYharimsUltisword");
            recipe.AddIngredient(null, "CoreofCalamity");
            recipe.AddIngredient(null, "UeliaceBar", 15);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.Next(5) == 0)
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 106);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.Next(2) == 0)
            {
                player.AddBuff(mod.BuffType("TyrantsFury"), 300);
            }
            target.AddBuff(BuffID.OnFire, 300);
            target.AddBuff(BuffID.Venom, 240);
            target.AddBuff(BuffID.CursedInferno, 180);
            target.AddBuff(mod.BuffType("BrimstoneFlames"), 300);
            target.AddBuff(mod.BuffType("HolyLight"), 300);
        }
    }
}
