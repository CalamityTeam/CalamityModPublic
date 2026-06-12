using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Viscera : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<Laceration>(), ModContent.BuffType<BurningBlood>()];
        }

        public const int BoomLifetime = 40;
        public int Counter = 0;

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 52;
            Item.damage = 229;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 18;
            Item.useTime = 7;
            Item.useAnimation = 22;
            Item.reuseDelay = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<VisceraBeam>();
            Item.shootSpeed = 6f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Counter++;
            SoundStyle fire = new("CalamityMod/Sounds/Item/MagnaCannonShot");
            SoundEngine.PlaySound(fire with { Volume = 0.2f, Pitch = 0.95f }, position);
            position = position + velocity.RotatedBy(-0.75f * player.direction) * 1.8f;

            Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.025f), type, (int)(damage * (1 + (Counter - 1) * 0.2)), knockback, player.whoAmI, 0f, Counter == 4 ? 1 : 0);
            if (Counter >= 4)
                Counter = 0;
            
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BloodstoneCore>(4).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
