using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Summon;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class KingofConstellationsTenryu : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";

        public override void SetStaticDefaults()
        {
            ItemID.Sets.StaffMinionSlotsRequired[Type] = 4f;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<Shadowflame>(), BuffID.Frostburn2];
        }

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 62;
            Item.mana = 10;
            Item.damage = 187;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.buffType = ModContent.BuffType<KingofConstellationsBuff>();
            Item.shoot = ModContent.ProjectileType<BlackDragonHead>();
            Item.UseSound = Flare.FlareSound;
            Item.useAnimation = Item.useTime = 24;

            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.Calamity().donorItem = true;

            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.DamageType = DamageClass.Summon;
            Item.autoReuse = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-20, 0);

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0 && player.maxMinions >= 4;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 spawnPos = new Vector2(player.position.X - 200, player.position.Y - 200);
            Vector2 spawnPos2 = new Vector2(player.position.X + 200, player.position.Y - 200);
            player.AddBuff(Item.buffType, 2);
            SpawnDragon(ModContent.ProjectileType<WhiteDragonHead>(), ModContent.ProjectileType<WhiteDragonBody>(), ModContent.ProjectileType<WhiteDragonTail>(), spawnPos2, player, source, damage, Item.damage, knockback);
            SpawnDragon(ModContent.ProjectileType<BlackDragonHead>(), ModContent.ProjectileType<BlackDragonBody>(), ModContent.ProjectileType<BlackDragonTail>(), spawnPos, player, source, damage, Item.damage, knockback);
            return false;
        }

        public static void SpawnDragon(int headType, int bodyType, int tailType, Vector2 spawnPos, Player player, EntitySource_ItemUse_WithAmmo source, int damage, int originalDamage, float knockback)
        {
            // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
            var head = Projectile.NewProjectileDirect(source, spawnPos, player.DirectionTo(Main.MouseWorld) * 3, headType, damage, knockback, player.whoAmI);
            head.originalDamage = originalDamage;
            var tail = Projectile.NewProjectileDirect(source, spawnPos, Vector2.Zero, tailType, damage, knockback, player.whoAmI);
            tail.originalDamage = originalDamage;
            for (var i = 0; i < 20; i++)
            {
                var body = Projectile.NewProjectileDirect(source, spawnPos, Vector2.Zero, bodyType, damage, knockback, player.whoAmI);
                body.originalDamage = originalDamage;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.StardustDragonStaff).
                AddIngredient(ItemID.DarkShard).
                AddIngredient(ItemID.LightShard).
                AddIngredient<TwistingNether>(3).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}

