using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Typeless
{
    public class Aestheticus : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aestheticus");
            Tooltip.SetDefault("Fires crystals that explode and slow enemies down\n" +
                "This weapon scales with all your damage stats at once");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 58;
            Item.damage = 8;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item109;
            Item.autoReuse = true;
            Item.height = 58;

            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.Calamity().donorItem = true;

            Item.shoot = ModContent.ProjectileType<CursorProj>();
            Item.shootSpeed = 5f;
        }

        // Aestheticus scales off of all damage types simultaneously (meaning it scales 5x from universal damage boosts).
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            float formula = 5f * (player.GetDamage(DamageClass.Generic).Additive - 1f);
            formula += player.GetDamage(DamageClass.Melee).Additive - 1f;
            formula += player.GetDamage(DamageClass.Ranged).Additive - 1f;
            formula += player.GetDamage(DamageClass.Magic).Additive - 1f;
            formula += player.GetDamage(DamageClass.Summon).Additive - 1f;
            formula += player.Calamity().throwingDamage - 1f;
            damage += formula;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.HellstoneBar, 10).AddIngredient(ItemID.MeteoriteBar, 10).AddIngredient(ModContent.ItemType<AerialiteBar>(), 5).AddIngredient(ModContent.ItemType<SeaPrism>(), 10).AddIngredient(ItemID.Glass, 20).AddIngredient(ItemID.Gel, 15).AddIngredient(ItemID.FallenStar, 5).AddTile(TileID.Anvils).Register();
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Vaporfied>(), 120);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Vaporfied>(), 120);
        }
    }
}
