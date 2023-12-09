using CalamityMod.Buffs.Summon;
using CalamityMod.ExtraJumps;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Statigel
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("StatigelHood")]
    public class StatigelHeadSummon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PreHardmode";
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 4; //20
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<StatigelArmor>() && legs.type == ModContent.ItemType<StatigelGreaves>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = this.GetLocalizedValue("SetBonus") + "\n" + CalamityUtils.GetTextValueFromModItem<StatigelArmor>("CommonSetBonus");
            var modPlayer = player.Calamity();
            modPlayer.statigelSet = true;
            modPlayer.slimeGod = true;
            player.GetJumpState<StatigelJump>().Enable();
            Player.jumpHeight += 5;
            player.jumpSpeedBoost += 0.6f;
            player.GetDamage<SummonDamageClass>() += 0.18f;
            player.maxMinions++;
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_Accessory(Item);
                if (player.FindBuffIndex(ModContent.BuffType<BabySlimeGodBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<BabySlimeGodBuff>(), 3600, true);
                }

                var minionID = -1;

                // 08DEC2023: Ozzatron: Corruption and Crimson Slimes spawned with Old Fashioned active will retain their bonus damage indefinitely. Oops. Don't care.
                var baseDamage = player.ApplyArmorAccDamageBonusesTo(33);
                var minionDamage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(baseDamage);

                if (WorldGen.crimson && player.ownedProjectileCounts[ModContent.ProjectileType<CrimsonSlimeGodMinion>()] < 1)
                    minionID = Projectile.NewProjectile(source, player.Center, -Vector2.UnitY, ModContent.ProjectileType<CrimsonSlimeGodMinion>(), minionDamage, 0f, Main.myPlayer);
                else if (!WorldGen.crimson && player.ownedProjectileCounts[ModContent.ProjectileType<CorruptionSlimeGodMinion>()] < 1)
                    minionID = Projectile.NewProjectile(source, player.Center, -Vector2.UnitY, ModContent.ProjectileType<CorruptionSlimeGodMinion>(), minionDamage, 0f, Main.myPlayer);

                if (Main.projectile.IndexInRange(minionID))
                    Main.projectile[minionID].originalDamage = baseDamage;
            }
        }

        public override void UpdateEquip(Player player)
        {
            player.GetKnockback<SummonDamageClass>() += 1.5f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PurifiedGel>(5).
                AddIngredient<BlightedGel>(5).
                AddTile<StaticRefiner>().
                Register();
        }
    }
}
