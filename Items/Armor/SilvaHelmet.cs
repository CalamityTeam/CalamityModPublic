using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class SilvaHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silva Helmet");
            Tooltip.SetDefault("13% increased summon damage and +5 max minions");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 9000000;
            item.defense = 13; //110
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(108, 45, 199);
                }
            }
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("SilvaArmor") && legs.type == mod.ItemType("SilvaLeggings");
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.silvaSet = true;
            modPlayer.silvaSummon = true;
            player.setBonus = "You are immune to almost all debuffs\n" +
                "Reduces all damage taken by 5%, this is calculated separately from damage reduction\n" +
                "All projectiles spawn healing leaf orbs on enemy hits\n" +
                "Max run speed and acceleration boosted by 5%\n" +
                "If you are reduced to 0 HP you will not die from any further damage for 10 seconds\n" +
                "If you get reduced to 0 HP again while this effect is active you will lose 100 max life\n" +
                "This effect only triggers once per life\n" +
                "Your max life will return to normal if you die\n" +
                "Summons an ancient leaf prism to blast your enemies with life energy\n" +
                "After the silva invulnerability time your minions will deal 15% more damage and you will gain +2 max minions";
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(mod.BuffType("SilvaCrystal")) == -1)
                {
                    player.AddBuff(mod.BuffType("SilvaCrystal"), 3600, true);
                }
                if (player.ownedProjectileCounts[mod.ProjectileType("SilvaCrystal")] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, mod.ProjectileType("SilvaCrystal"), (int)((double)1500 * player.minionDamage), 0f, Main.myPlayer, 0f, 0f);
                }
            }
        }

        public override void UpdateEquip(Player player)
        {
            player.minionDamage += 0.13f;
            player.maxMinions += 5;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "DarksunFragment", 5);
            recipe.AddIngredient(null, "EffulgentFeather", 5);
            recipe.AddIngredient(null, "CosmiliteBar", 5);
            recipe.AddIngredient(null, "NightmareFuel", 14);
            recipe.AddIngredient(null, "EndothermicEnergy", 14);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}