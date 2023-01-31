using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Hydrothermic
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("AtaxiaHelmet")]
    public class HydrothermicHeadSummon : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Hydrothermic Helmet");
            Tooltip.SetDefault("5% increased minion damage and increased minion knockback\n" +
                "Temporary immunity to lava and immunity to fire damage");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 6; //40
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<HydrothermicArmor>() && legs.type == ModContent.ItemType<HydrothermicSubligar>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
            player.Calamity().hydrothermalSmoke = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "40% increased minion damage and +2 max minions\n" +
                "Inferno effect when below 50% life\n" +
                "Summons a hydrothermic vent to protect you\n" +
                "You emit a blazing explosion when you are hit";
            var modPlayer = player.Calamity();
            modPlayer.ataxiaBlaze = true;
            modPlayer.chaosSpirit = true;
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_ItemUse(Item);
                if (player.FindBuffIndex(ModContent.BuffType<HydrothermicVentBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<HydrothermicVentBuff>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<HydrothermicVent>()] < 1)
                {
                    var damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(190);
                    var p = Projectile.NewProjectile(source, player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<HydrothermicVent>(), damage, 0f, Main.myPlayer, 38f, 0f);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = 190;
                }
            }
            player.GetDamage<SummonDamageClass>() += 0.4f;
            player.maxMinions += 2;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<SummonDamageClass>() += 0.05f;
            player.GetKnockback<SummonDamageClass>() += 1.5f;
            player.lavaMax += 240;
            player.buffImmune[BuffID.OnFire] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ScoriaBar>(7).
                AddIngredient<CoreofHavoc>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
