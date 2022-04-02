using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class AuricTeslaSpaceHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Auric Tesla Space Helmet");
            Tooltip.SetDefault("15% increased minion damage\n" +
                    "Not moving boosts all damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.defense = 12; //132
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AuricTeslaBodyArmor>() && legs.type == ModContent.ItemType<AuricTeslaCuisses>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Summoner Tarragon, Bloodflare and Silva armor effects\n" +
                "All projectiles spawn healing auric orbs on enemy hits\n" +
                "Max run speed and acceleration boosted by 5%\n" +
                "+6 max minions and 75% increased minion damage";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.tarraSet = true;
            modPlayer.tarraSummon = true;
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareSummon = true;
            modPlayer.silvaSet = true;
            modPlayer.silvaSummon = true;
            modPlayer.auricSet = true;
            modPlayer.WearingPostMLSummonerSet = true;
            player.thorns += 3f;
            player.lavaMax += 240;
            player.ignoreWater = true;
            player.crimsonRegen = true;
            player.minionDamage += 0.75f;
            player.maxMinions += 6;
            if (player.lavaWet)
            {
                player.statDefense += 30;
                player.lifeRegen += 10;
            }
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(ModContent.BuffType<SilvaSummonSetBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<SilvaSummonSetBuff>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<SilvaCrystal>()] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<SilvaCrystal>(), (int)(1200f * player.MinionDamage()), 0f, Main.myPlayer, -20f, 0f);
                }
            }
        }

        public override void UpdateEquip(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.auricBoost = true;
            player.minionDamage += 0.15f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SilvaHelmet>());
            recipe.AddIngredient(ModContent.ItemType<BloodflareHelmet>());
            recipe.AddIngredient(ModContent.ItemType<TarragonHornedHelm>());
            recipe.AddIngredient(ModContent.ItemType<PsychoticAmulet>());
            recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 12);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
