using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Summon;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class CosmicViperEngine : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<GodSlayerInferno>()];
        }
        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 28;
            Item.damage = 255;
            Item.mana = 10;
            Item.useAnimation = Item.useTime = 24;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item15; //phaseblade sound effect
            Item.autoReuse = true;
            Item.buffType = ModContent.BuffType<CosmicViperEngineBuff>();
            Item.shoot = ModContent.ProjectileType<CosmicViperSummon>();
            Item.shootSpeed = 10f; // Affects bullet speed
            Item.DamageType = DamageClass.Summon;

            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = ModContent.RarityType<CosmicPurple>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);

            float speed = Item.shootSpeed;
            Vector2 spawnPos = player.RotatedRelativePoint(player.MountedCenter, true);
            float xPos = Main.screenPosition.X + Main.mouseX - spawnPos.X;
            float yPos = Main.screenPosition.Y + (player.gravDir == -1 ? Main.screenHeight - Main.mouseY : Main.mouseY) - spawnPos.Y;
            Vector2 vel = new Vector2(xPos, yPos).SafeNormalize(Vector2.UnitX * player.direction) * speed;

            spawnPos = player.ClampedMouseWorld();
            vel = vel.RotatedBy(MathHelper.PiOver2);
            var minion = Projectile.NewProjectileDirect(source, spawnPos + vel, vel, type, damage, knockback, player.whoAmI, 0f, 1f);
            minion.originalDamage = Item.damage;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<TacticalPlagueEngine>().
                AddIngredient<CosmiliteBar>(10).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
