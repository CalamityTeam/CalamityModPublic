using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class DemonshadeHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demonshade Helm");
            Tooltip.SetDefault("30% increased damage and 15% increased critical strike chance, +10 max minions");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(5, 0, 0, 0);
            item.defense = 50; //15
            item.Calamity().customRarity = CalamityRarity.ItemSpecific;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<DemonshadeBreastplate>() && legs.type == ModContent.ItemType<DemonshadeGreaves>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            string hotkey = CalamityMod.TarraHotKey.TooltipHotkeyString();
            player.setBonus = "100% increased minion damage\n" +
                "All attacks inflict the demon flame debuff\n" +
                "Shadowbeams and demon scythes will fire down when you are hit\n" +
                "A friendly red devil follows you around\n" +
                "Press " + hotkey + " to enrage nearby enemies with a dark magic spell for 10 seconds\n" +
                "This makes them do 25% more damage but they also take 125% more damage";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.dsSetBonus = true;
            modPlayer.wearingRogueArmor = true;
            if (player.whoAmI == Main.myPlayer && !modPlayer.chibii)
            {
                modPlayer.redDevil = true;
                if (player.FindBuffIndex(ModContent.BuffType<DemonshadeSetDevilBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<DemonshadeSetDevilBuff>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<DemonshadeRedDevil>()] < 1)
                {
					int damage = (int)(10000 * player.AverageDamage());
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<DemonshadeRedDevil>(), damage, 0f, Main.myPlayer, 0f, 0f);
                }
            }
            player.minionDamage += 1f;
        }

        public override void UpdateEquip(Player player)
        {
            player.maxMinions += 10;
            player.allDamage += 0.3f;
            player.Calamity().AllCritBoost(15);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 8);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
