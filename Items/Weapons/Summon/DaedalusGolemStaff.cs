using CalamityMod.Buffs.Summon;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class DaedalusGolemStaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.Frostburn2];
        }
        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.damage = 70;
            Item.mana = 10;
            Item.useAnimation = Item.useTime = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item67;
            Item.autoReuse = true;
            Item.buffType = ModContent.BuffType<DaedalusGolemBuff>();
            Item.shoot = ModContent.ProjectileType<DaedalusGolem>();
            Item.DamageType = DamageClass.Summon;

            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            Vector2 mouse = player.ClampedMouseWorld();
            Point mouseTileCoords = mouse.ToTileCoordinates();
            if (!CalamityUtils.ParanoidTileRetrieval(mouseTileCoords.X, mouseTileCoords.Y).IsTileSolidGround())
            {
                var minion = Projectile.NewProjectileDirect(source, mouse, Vector2.UnitY * 4f, type, damage, knockback, player.whoAmI);
                minion.originalDamage = Item.damage;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CryonicBar>(12).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
