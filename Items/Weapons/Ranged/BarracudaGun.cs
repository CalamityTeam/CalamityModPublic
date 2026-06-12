using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class BarracudaGun : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.IsRangedSpecialistWeapon[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<Laceration>()];
        }
        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 28;
            Item.damage = 52;
            Item.channel = true;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.value = CalamityGlobalItem.RarityPurpleBuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = new SoundStyle("CalamityMod/Sounds/Item/GunShotBig") with { Volume = 0.5f, Pitch = Main.rand.NextFloat(0.2f, 0.3f) };
            Item.autoReuse = true;
            Item.shootSpeed = 15f;
            Item.shoot = ModContent.ProjectileType<MechanicalBarracuda>();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int numProj = 4;
            float rotation = MathHelper.ToRadians(3);
            for (int i = 0; i < numProj; i++)
            {
                // adds slight variance to the speed of the projectiles like piranha gun does
                Vector2 projVelocity = velocity * Main.rand.NextFloat(0.8f, 1.2f);
                Vector2 perturbedSpeed = projVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.PiranhaGun).
                AddIngredient(ItemID.LunarBar, 5).
                AddIngredient(ItemID.SharkFin, 2).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
