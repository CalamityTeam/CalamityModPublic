using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class ReaverHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaver Helmet");
            Tooltip.SetDefault("5% increased minion damage, +2 max minions, and increased minion knockback\n" +
                "10% increased movement speed and can move freely through liquids");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
			item.value = Item.buyPrice(0, 30, 0, 0);
			item.rare = 7;
            item.defense = 3; //36
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("ReaverScaleMail") && legs.type == mod.ItemType("ReaverCuisses");
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "16% increased minion damage\n" +
                "Summons a reaver orb that emits spore gas when enemies are near";
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.reaverOrb = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(mod.BuffType("ReaverOrb")) == -1)
                {
                    player.AddBuff(mod.BuffType("ReaverOrb"), 3600, true);
                }
                if (player.ownedProjectileCounts[mod.ProjectileType("ReaverOrb")] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, mod.ProjectileType("ReaverOrb"), (int)(80f * player.minionDamage), 0f, Main.myPlayer, 0f, 0f);
                }
            }
            player.minionDamage += 0.16f;
        }

        public override void UpdateEquip(Player player)
        {
            player.ignoreWater = true;
            player.minionDamage += 0.05f;
            player.minionKB += 1f;
            player.maxMinions += 2;
            player.moveSpeed += 0.1f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "DraedonBar", 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
