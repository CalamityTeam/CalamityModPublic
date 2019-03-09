using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class MolluskShellmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mollusk Shellmet");
            Tooltip.SetDefault("5% increased damage and 4% increased critical strike chance\n" +
							   "7% decreased movement speed\n" +
							   "You can move freely through liquids");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 22;
			item.value = Item.buyPrice(0, 25, 0, 0);
			item.rare = 5;
            item.defense = 18;
        }

        public override void UpdateEquip(Player player)
        {
			player.ignoreWater = true;
            const float damageUp = 0.05f;
            const int critUp = 4;
            player.meleeDamage += damageUp;
            player.rangedDamage += damageUp;
            player.magicDamage += damageUp;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += damageUp;
            player.minionDamage += damageUp;
            player.meleeCrit += critUp;
            player.rangedCrit += critUp;
            player.magicCrit += critUp;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += critUp;
            player.moveSpeed -= 0.07f;
        }
		
		public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("MolluskShellplate") && legs.type == mod.ItemType("MolluskShelleggings");
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Two shellfishes aid you in combat\n" +
							  "10% increased damage";
			const float damageUp = 0.10f;
            player.meleeDamage += damageUp;
            player.rangedDamage += damageUp;
            player.magicDamage += damageUp;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += damageUp;
            player.minionDamage += damageUp;
            player.GetModPlayer<CalamityPlayer>().molluskSet = true;
			player.maxMinions += 4;
			if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(mod.BuffType("Shellfish")) == -1)
                {
                    player.AddBuff(mod.BuffType("Shellfish"), 3600, true);
                }
                if (player.ownedProjectileCounts[mod.ProjectileType("Shellfish")] < 2)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, mod.ProjectileType("Shellfish"), (int)((double)1500 * player.minionDamage), 0f, Main.myPlayer, 0f, 0f);
                }
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType("SeaPrism"), 15);
            recipe.AddIngredient(mod.ItemType("MolluskHusk"), 6);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
