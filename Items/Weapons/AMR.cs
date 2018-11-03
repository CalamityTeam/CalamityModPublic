using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons
{
    public class AMR : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Anti-materiel Rifle");
            Tooltip.SetDefault("Fires a .50 caliber sniper round that rips apart enemy defense and DR\n" +
                "If you crit the target a second swarm of bullets will fire near the target");
        }

        public override void SetDefaults()
        {
            item.damage = 4000;
            item.ranged = true;
            item.width = 76;
            item.crit += 20;
            item.height = 30;
            item.useTime = 60;
            item.useAnimation = 60;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 9.5f;
            item.value = 10000000;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaBlast");
            item.autoReuse = true;
            item.shoot = 10;
            item.shootSpeed = 12f;
            item.useAmmo = 97;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(43, 96, 222);
                }
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "Shroomer");
            recipe.AddIngredient(null, "CosmiliteBar", 10);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("AMR"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            return false;
        }
    }
}