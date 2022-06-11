using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
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
    public class StatigelHeadSummon : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Statigel Hood");
            Tooltip.SetDefault("Increased minion knockback");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 4; //20
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<StatigelArmor>() && legs.type == ModContent.ItemType<StatigelGreaves>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "18% increased minion damage and +1 max minion\n" +
                "Summons a mini slime god to fight for you, the type depends on what world evil you have\n" +
                "When you take over 100 damage in one hit you become immune to damage for an extended period of time\n" +
                "Grants an extra jump and increased jump height\n" +
                "12% increased jump speed";
            var modPlayer = player.Calamity();
            modPlayer.statigelSet = true;
            modPlayer.slimeGod = true;
            modPlayer.statigelJump = true;
            Player.jumpHeight += 5;
            player.jumpSpeedBoost += 0.6f;
            player.GetDamage<SummonDamageClass>() += 0.18f;
            player.maxMinions++;
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_Accessory(Item);
                if (player.FindBuffIndex(ModContent.BuffType<StatigelSummonSetBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<StatigelSummonSetBuff>(), 3600, true);
                }

                var minionID = -1;
                var baseDamage = 33;
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
                AddIngredient(ItemID.HellstoneBar, 9).
                AddTile<StaticRefiner>().
                Register();
        }
    }
}
