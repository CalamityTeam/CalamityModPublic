using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Plaguebringer
{
    [AutoloadEquip(EquipType.Head)]
    public class PlaguebringerVisor : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Hardmode";

        public static int MinionSlotBoost = 1;
        public static float SummonDamageBoost = 0.20f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MinionSlotBoost, SummonDamageBoost.ToPercent());

        // Set Bonus
        public static int PlagueDashDamage = 50;
        public static float PlagueDashKnockback = 3f;
        public static int PlagueDashIFrames = 12;
        public static int BeeMinionDamage = 25;
        public static int BeePlagueDuration = CalamityUtils.SecondsToFrames(5);

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 9; // 32 total
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.Calamity().donorItem = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.maxMinions += MinionSlotBoost;
            player.GetDamage<SummonDamageClass>() += SummonDamageBoost;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<PlaguebringerCarapace>() && legs.type == ModContent.ItemType<PlaguebringerPistons>();

        public override void ArmorSetShadows(Player player) => player.armorEffectDrawShadow = true;

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = this.GetLocalizedValue("SetBonus");

            player.Calamity().plaguebringerPatronSet = true;
            player.Calamity().DashID = PlaguebringerArmorDash.ID;
            player.dashType = 0;
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_ItemUse(Item);
                if (player.FindBuffIndex(ModContent.BuffType<LilPlaguebringerBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<LilPlaguebringerBuff>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<PlaguebringerSummon>()] < 1)
                {
                    var damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(BeeMinionDamage);

                    var p = Projectile.NewProjectile(source, player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<PlaguebringerSummon>(), damage, 0f, player.whoAmI, 0f, 0f);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = BeeMinionDamage;
                }
            }

            // Constantly emit dim green light
            Lighting.AddLight(player.Center, 0f, 0.39f, 0.24f);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BeeHeadgear).
                AddIngredient<InfectedArmorPlating>(4).
                AddIngredient<PlagueCellCanister>(4).
                AddTile(TileID.MythrilAnvil).
                SortBeforeFirstRecipesOf(ModContent.ItemType<PlaguebringerCarapace>()).
                Register();
        }
    }
}
