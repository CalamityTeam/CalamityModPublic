using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace CalamityMod.Items.Weapons.Summon
{
    public class WulfrumController : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";

        public override void SetDefaults()
        {
            Item.damage = 16;
            Item.mana = 10;
            Item.width = 28;
            Item.height = 20;
            Item.useTime = Item.useAnimation = 34;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 0.5f;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item15; //phaseblade sound effect
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<WulfrumDroid>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override void HoldItem(Player player)
        {
            player.Calamity().rightClickListener = true;
            player.Calamity().mouseWorldListener = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                position = Main.MouseWorld;
                velocity.X = 0;
                velocity.Y = 0;
                int droid = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 1f);
                if (Main.projectile.IndexInRange(droid))
                    Main.projectile[droid].originalDamage = Item.damage;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WulfrumMetalScrap>(9).
                AddTile(TileID.Anvils).
                Register();
        }
    }

    public class WulfrumControllerPlayer : ModPlayer
    {
        public int buffingDrones = 0;

        public override void ResetEffects()
        {
        }

        public override void UpdateDead()
        {
            buffingDrones = 0;
        }

        public override void UpdateLifeRegen()
        {
            if (buffingDrones > 0)
            {
                // 1 life regen per drone.
                Player.lifeRegen += (int)(buffingDrones);

                // 3 defense per drone.
                Player.statDefense += buffingDrones * 3;

                buffingDrones = 0;
            }
        }

        public override void PostUpdateMiscEffects()
        {
            if (buffingDrones > 0 && Main.rand.NextBool(3))
            {
                Vector2 dustPos = Player.position + (Player.height * Main.rand.NextFloat(0.7f, 1f)  + Player.gfxOffY) * Vector2.UnitY + Vector2.UnitX * Main.rand.NextFloat() * Player.width;

                Dust chust = Dust.NewDustPerfect(dustPos, 274, -Vector2.UnitY * Main.rand.NextFloat(1.4f, 7f) + Player.velocity , Alpha: 100, Scale: Main.rand.NextFloat(1.2f, 1.8f));
                chust.noGravity = true;
                chust.noLight = true;
            }
        }
    }
}
