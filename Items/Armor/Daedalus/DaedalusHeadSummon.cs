using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Daedalus
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("DaedalusHeadgear")]
    public class DaedalusHeadSummon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Hardmode";
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 3; //33
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<DaedalusBreastplate>() && legs.type == ModContent.ItemType<DaedalusLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = this.GetLocalizedValue("SetBonus");
            var modPlayer = player.Calamity();
            modPlayer.daedalusCrystal = true;
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_ItemUse(Item);
                if (player.FindBuffIndex(ModContent.BuffType<DaedalusCrystalBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<DaedalusCrystalBuff>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<DaedalusCrystal>()] < 1)
                {
                    // 08DEC2023: Ozzatron: Daedalus Crystals spawned with Old Fashioned active will retain their bonus damage indefinitely. Oops. Don't care.
                    int baseDamage = player.ApplyArmorAccDamageBonusesTo(95);
                    var damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(baseDamage);

                    var p = Projectile.NewProjectileDirect(source, player.Center, -Vector2.UnitY, ModContent.ProjectileType<DaedalusCrystal>(), damage, 0f, Main.myPlayer, 50f, 0f);
                    p.originalDamage = baseDamage;
                }
            }
            player.GetDamage<SummonDamageClass>() += 0.2f;
            player.maxMinions += 2;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<SummonDamageClass>() += 0.05f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CryonicBar>(7).
                AddIngredient<EssenceofEleum>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
