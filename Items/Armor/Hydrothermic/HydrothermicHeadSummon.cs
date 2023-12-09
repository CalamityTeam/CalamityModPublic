using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Hydrothermic
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("AtaxiaHelmet")]
    public class HydrothermicHeadSummon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Hardmode";
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
            player.setBonus = this.GetLocalizedValue("SetBonus") + "\n" + CalamityUtils.GetTextValueFromModItem<HydrothermicArmor>("CommonSetBonus");
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
                    // 08DEC2023: Ozzatron: Hydrothermic Vents spawned with Old Fashioned active will retain their bonus damage indefinitely. Oops. Don't care.
                    int baseDamage = player.ApplyArmorAccDamageBonusesTo(190);
                    var damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(baseDamage);

                    var p = Projectile.NewProjectile(source, player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<HydrothermicVent>(), damage, 0f, Main.myPlayer, 38f, 0f);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = baseDamage;
                }
            }
            player.GetDamage<SummonDamageClass>() += 0.4f;
            player.maxMinions += 2;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<SummonDamageClass>() += 0.05f;
            player.GetKnockback<SummonDamageClass>() += 1.5f;
            player.lavaImmune = true;
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
