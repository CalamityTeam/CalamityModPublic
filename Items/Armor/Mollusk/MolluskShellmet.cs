using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityMod.Items.Potions.Alcohol;

namespace CalamityMod.Items.Armor.Mollusk
{
    [AutoloadEquip(EquipType.Head)]
    public class MolluskShellmet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Hardmode";
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 18;
        }

        public override void UpdateEquip(Player player)
        {
            player.ignoreWater = true;
            player.GetDamage<GenericDamageClass>() += 0.05f;
            player.GetCritChance<GenericDamageClass>() += 4;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<MolluskShellplate>() && legs.type == ModContent.ItemType<MolluskShelleggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = this.GetLocalizedValue("SetBonus");
            var modPlayer = player.Calamity();
            player.GetDamage<GenericDamageClass>() += 0.1f;
            modPlayer.molluskSet = true;
            player.maxMinions += 4;
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_ItemUse(Item);
                if (player.FindBuffIndex(ModContent.BuffType<ShellfishBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<ShellfishBuff>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<Shellfish>()] < 2)
                {
                    // 08DEC2023: Ozzatron: Clams spawned with Old Fashioned active will retain their bonus damage indefinitely. Oops. Don't care.
                    int baseDamage = player.ApplyArmorAccDamageBonusesTo(140);
                    // wait why does this not scale with summon damage

                    Projectile clam = Projectile.NewProjectileDirect(source, player.Center, -Vector2.UnitY, ModContent.ProjectileType<Shellfish>(), baseDamage, 0f, player.whoAmI);
                    clam.originalDamage = baseDamage;
                }
            }
            player.Calamity().wearingRogueArmor = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MolluskHusk>(6).
                AddIngredient<SeaPrism>(15).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
