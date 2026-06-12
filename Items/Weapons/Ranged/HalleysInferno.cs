using System.Linq;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class HalleysInferno : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        public static readonly SoundStyle ShootSound = new("CalamityMod/Sounds/Item/HalleysInfernoShoot") { Volume = 0.68f };
        public static readonly SoundStyle Hit = new("CalamityMod/Sounds/Item/HalleysInfernoHit") { Volume = 0.75f };
        public static float MaxStarburstPerComet => 1;
        public static float MaxStarburstPerStar => 0.5f;
        public static float LostAccuracyPerMiss => 4;
        public static float MaxAccuracy => 50;

        public static float StarburstDmgMult => 2.5f;

        public static float StarburstVelMult => 0.75f;
        public override void SetDefaults()
        {
            Item.width = 84;
            Item.height = 34;
            Item.damage = 444;
            Item.knockBack = 5.5f;
            Item.DamageType = DamageClass.Ranged;
            Item.crit = 20;

            // Burst of 5, one every 5 frames for 25 total. Cooldown of 39 frames.
            Item.useTime = 5;
            Item.useAnimation = 25;
            Item.reuseDelay = 39;
            Item.useLimitPerAnimation = 5;
            Item.autoReuse = true;

            Item.useAmmo = AmmoID.Gel;
            Item.consumeAmmoOnFirstShotOnly = true;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<HalleysInfernoHoldout>();

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = ShootSound;
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
        }
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void HoldItem(Player player)
        {
            if (player.mount.Type == MountID.Drill)
                return;

            if (Main.LocalPlayer == player)
            {
                if (!Main.projectile.Any(x=> x.active && x.owner == player.whoAmI && x.type == Item.shoot))
                {
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item),player.Center,Vector2.Zero,Item.shoot,0,0,player.whoAmI);
                }
            }
            player.Calamity().StratusStarburstResetTimer = (int)MathHelper.Max(player.Calamity().StratusStarburstResetTimer, 600);
            player.Calamity().ammoCost *= 0.5f;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ElfMelter).
                AddIngredient<Lumenyl>(6).
                AddIngredient<RuinousSoul>(4).
                AddIngredient<ExodiumCluster>(12).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
