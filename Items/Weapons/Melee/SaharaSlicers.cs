using CalamityMod.Projectiles.Melee.Shortswords;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using CalamityMod.Projectiles.Melee;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("AquaticDischarge")]
    public class SaharaSlicers : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public bool AltProjectile = true;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 43;
            Item.height = 34;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 23;
            Item.DamageType = TrueMeleeDamageClass.Instance;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.shoot = ModContent.ProjectileType<SaharaSlicersBolt>();
            Item.shootSpeed = 3.3f;
            Item.knockBack = 6f;
            Item.UseSound = null;
            Item.channel = true;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2) // Bolts
            {
                Item.useStyle = ItemUseStyleID.Swing;
                if (player.Calamity().saharaSlicersBolts > 0)
                {
                    SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaivePierce with { Pitch = 1.5f }, player.Center);
                    Projectile.NewProjectile(source, position, velocity * 2, ModContent.ProjectileType<SaharaSlicersBolt>(), (int)(damage * 1.2), knockback * 1.2f, player.whoAmI, 1);
                    player.Calamity().saharaSlicersBolts--;
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.Item7 with { Pitch = 0.2f }, player.Center);
                }
            }
            else // Shortswords
            {
                Item.useStyle = ItemUseStyleID.Shoot;
                if (AltProjectile)
                {
                    SoundEngine.PlaySound(SoundID.Item1 with { Pitch = 0.4f }, player.Center);
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SaharaSlicersBladeAlt>(), damage, knockback, player.whoAmI);
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.Item1 with { Pitch = 0.9f }, player.Center);
                    Projectile.NewProjectile(source, position, velocity * 0.75f, ModContent.ProjectileType<SaharaSlicersBlade>(), damage, knockback, player.whoAmI);
                }
                AltProjectile = !AltProjectile;
            }
            return false;
        }
        public override bool MeleePrefix() => true;
    }
}
