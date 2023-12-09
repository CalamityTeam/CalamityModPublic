using CalamityMod.Buffs.Summon;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Items.Potions.Alcohol;

namespace CalamityMod.Items.Armor.Plaguebringer
{
    [AutoloadEquip(EquipType.Head)]
    public class PlaguebringerVisor : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Hardmode";
        public const int PlagueDashIFrames = 12;

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
            player.setBonus = this.GetLocalizedValue("SetBonus");

            player.Calamity().plaguebringerPatronSet = true;
            player.Calamity().DashID = PlaguebringerArmorDash.ID;
            player.dashType = 0;
            player.maxMinions += 3;
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_ItemUse(Item);
                if (player.FindBuffIndex(ModContent.BuffType<LilPlaguebringerBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<LilPlaguebringerBuff>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<PlaguebringerSummon>()] < 1)
                {
                    // 08DEC2023: Ozzatron: Plaguebringer armor dudes spawned with Old Fashioned active will retain their bonus damage indefinitely. Oops. Don't care.
                    int baseDamage = player.ApplyArmorAccDamageBonusesTo(25);
                    var damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(baseDamage);

                    var p = Projectile.NewProjectile(source, player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<PlaguebringerSummon>(), damage, 0f, player.whoAmI, 0f, 0f);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = baseDamage;
                }
            }

            // Constantly emit dim green light
            Lighting.AddLight(player.Center, 0f, 0.39f, 0.24f);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BeeHeadgear).
                AddIngredient<PlagueCellCanister>(4).
                AddIngredient<InfectedArmorPlating>(4).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
