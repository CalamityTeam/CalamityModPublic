using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Aerospec
{
    [AutoloadEquip(EquipType.Head)]
    public class AerospecHelmet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PreHardmode";
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.defense = 2; //13
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AerospecBreastplate>() && legs.type == ModContent.ItemType<AerospecLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = this.GetLocalizedValue("SetBonus") + "\n" + CalamityUtils.GetTextValueFromModItem<AerospecBreastplate>("CommonSetBonus");
            var modPlayer = player.Calamity();
            modPlayer.valkyrie = true;
            modPlayer.aeroSet = true;
            player.noFallDmg = true;
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_ItemUse(Item);
                if (player.FindBuffIndex(ModContent.BuffType<ValkyrieBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<ValkyrieBuff>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<Valkyrie>()] < 1)
                {
                    // 08DEC2023: Ozzatron: Aerospec Valkyries spawned with Old Fashioned active will retain their bonus damage indefinitely. Oops. Don't care.
                    int baseDamage = player.ApplyArmorAccDamageBonusesTo(20);
                    var damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(baseDamage);

                    var p = Projectile.NewProjectile(source, player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<Valkyrie>(), damage, 0f, Main.myPlayer, 0f, 0f);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = baseDamage;
                }
            }
            player.GetDamage<SummonDamageClass>() += 0.11f;
            player.maxMinions++;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.05f;
            player.GetDamage<SummonDamageClass>() += 0.05f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AerialiteBar>(5).
                AddIngredient(ItemID.SunplateBlock, 3).
                AddIngredient(ItemID.Feather).
                AddTile(TileID.SkyMill).
                Register();
        }
    }
}
