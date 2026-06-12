using CalamityMod.Projectiles.Rogue;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Weapons.Rogue
{
    public class SporeKnife : RogueWeapon
    {
        public static readonly SoundStyle ThrowSound = new SoundStyle("CalamityMod/Sounds/Item/SporeKnifeThrow", 1, 2) with { PitchVariance = 0.2f, MaxInstances = 2 };
        public static readonly SoundStyle ImpactSound = new SoundStyle("CalamityMod/Sounds/Item/SporeKnifeImpact") with { PitchVariance = 0.25f, MaxInstances = 10 };
        public static readonly SoundStyle StealthImpactSound = new SoundStyle("CalamityMod/Sounds/Item/SporeKnifeStealthImpact");
        public static readonly SoundStyle ChompSound = new SoundStyle("CalamityMod/Sounds/Item/SporeKnifeChomp", 1, 3) with { PitchVariance = 0.25f, MaxInstances = 10 };
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.Poisoned];
        }
        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 40;
            Item.damage = 18;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.knockBack = 1.75f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<SporeKnifeProj>();
            Item.shootSpeed = 15f;
            Item.DamageType = RogueDamageClass.Instance;
        }
        public override float StealthDamageMultiplier => 2f;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                SoundEngine.PlaySound(ThrowSound, position);
                int stealth = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.JungleSpores, 12).
                AddIngredient(ItemID.Stinger, 8).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
