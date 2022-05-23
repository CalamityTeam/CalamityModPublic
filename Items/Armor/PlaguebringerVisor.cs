using CalamityMod.Buffs.Summon;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer.Dashes;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class PlaguebringerVisor : ModItem
    {
        public const int PlagueDashIFrames = 12;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Plaguebringer Visor");
            Tooltip.SetDefault("15% increased minion damage\n" +
            "+20 max life");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 7; // 32 total
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.Calamity().donorItem = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<SummonDamageClass>() += 0.15f;
            player.statLifeMax2 += 20;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<PlaguebringerCarapace>() && legs.type == ModContent.ItemType<PlaguebringerPistons>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Grants a plague dash to ram enemies and afflict them with the plague\n" +
            "Summons a lil' plaguebringer to protect you and empower nearby minions\n" +
            "+3 max minions";

            player.Calamity().plaguebringerPatronSet = true;
            player.Calamity().DashID = PlaguebringerArmorDash.ID;
            player.dash = 0;
            player.maxMinions += 3;
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_ItemUse(Item);
                if (player.FindBuffIndex(ModContent.BuffType<PlaguebringerSummonBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<PlaguebringerSummonBuff>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<PlaguebringerSummon>()] < 1)
                {
                    int damage = (int)player.GetDamage<SummonDamageClass>().ApplyTo(80);
                    int p = Projectile.NewProjectile(source, player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<PlaguebringerSummon>(), damage, 0f, player.whoAmI, 0f, 0f);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = 80;
                }
            }

            // Constantly emit dim green light
            Lighting.AddLight(player.Center, 0f, 0.39f, 0.24f);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BeeHeadgear).
                AddIngredient<PlagueCellCluster>(4).
                AddIngredient<InfectedArmorPlating>(4).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
