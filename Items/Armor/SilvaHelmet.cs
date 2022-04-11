using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class SilvaHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silva Horned Hood");
            Tooltip.SetDefault("10% increased minion damage");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 24;
            Item.value = Item.buyPrice(0, 90, 0, 0);
            Item.defense = 13; //110
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SilvaArmor>() && legs.type == ModContent.ItemType<SilvaLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.silvaSet = true;
            modPlayer.silvaSummon = true;
            modPlayer.WearingPostMLSummonerSet = true;
            player.setBonus = "65% increased minion damage and +5 max minions\n" +
                "All projectiles spawn healing leaf orbs on enemy hits\n" +
                "Max run speed and acceleration boosted by 5%\n" +
                "If you are reduced to 1 HP you will not die from any further damage for 8 seconds\n" +
                "This effect has a 5 minute cooldown. The cooldown does not decrement if any bosses or events are active.\n" +
                "Summons an ancient leaf prism to blast your enemies with life energy";
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetProjectileSource_Item(Item);
                if (player.FindBuffIndex(ModContent.BuffType<SilvaSummonSetBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<SilvaSummonSetBuff>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<SilvaCrystal>()] < 1)
                {
                    int p = Projectile.NewProjectile(source, player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<SilvaCrystal>(), (int)(600f * player.MinionDamage()), 0f, Main.myPlayer, -20f, 0f);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = 600;
                }
            }
            player.GetDamage(DamageClass.Summon) += 0.65f;
            player.maxMinions += 5;
        }

        public override void UpdateEquip(Player player) => player.GetDamage(DamageClass.Summon) += 0.1f;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<EffulgentFeather>(5).
                AddRecipeGroup("AnyGoldBar", 5).
                AddIngredient<Tenebris>(6).
                AddIngredient<AscendantSpiritEssence>(2).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
