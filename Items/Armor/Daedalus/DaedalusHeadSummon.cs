using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Daedalus
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("DaedalusHeadgear")]
    public class DaedalusHeadSummon : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Daedalus Mask");
            Tooltip.SetDefault("5% increased minion damage");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 25, 0, 0);
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
            player.setBonus = "20% increased minion damage and +2 max minions\n" +
                "A daedalus crystal floats above you to protect you";
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
                    var damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(95);
                    var p = Projectile.NewProjectileDirect(source, player.Center, -Vector2.UnitY, ModContent.ProjectileType<DaedalusCrystal>(), damage, 0f, Main.myPlayer, 50f, 0f);
                    p.originalDamage = 95;
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
                AddIngredient<CryonicBar>(8).
                AddIngredient(ItemID.CrystalShard, 3).
                AddIngredient<EssenceofEleum>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
