using CalamityMod.Buffs.Summon;
using CalamityMod.Items.Materials;
using CalamityMod.Systems.Collections;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using CalamityMod.Buffs.DamageOverTime;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class DragonbloodDisgorger : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";

        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<BurningBlood>(), ModContent.BuffType<Laceration>()];
            ItemID.Sets.StaffMinionSlotsRequired[Type] = 6f;
        }

        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 62;
            Item.damage = 215;
            Item.mana = 10;
            Item.useAnimation = Item.useTime = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.UseSound = SoundID.DD2_SkeletonDeath;
            Item.buffType = ModContent.BuffType<SkeletalDragonsBuff>();
            Item.shoot = ModContent.ProjectileType<SkeletalDragonMother>();
            Item.DamageType = DamageClass.Summon;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            var minion = Projectile.NewProjectileDirect(source, player.ClampedMouseWorld(), Vector2.Zero, type, damage, knockback, player.whoAmI);
            minion.originalDamage = Item.damage;
            return false;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0 && player.maxMinions >= 5;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BloodstoneCore>(12).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
