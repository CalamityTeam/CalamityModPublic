using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Hydra : ModItem
    {
        //Editable stats:
        public const int BulletsPerShot = 4;
        public const float ShotSpread = 10f; //in degrees
        public const int TimeToSpawnHead = 3; //in seconds
        public const int MaximumHeadCount = 3;

        public int HeadID = ModContent.ProjectileType<HydraHead>();
        public int HeadSpawnTimer = 0;

        //There could be new sounds for these. Vanilla sounds for now.
        public static readonly SoundStyle SpawnSound = SoundID.Item60;
        public static readonly SoundStyle LaunchSound = SoundID.Item1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hydra");
            Tooltip.SetDefault("Fusion of heavy machinery and Lernaean blood\n" +
                            "Fires a spread of " + BulletsPerShot + " toxic bullets\n"+
                            "Grows a gun-head every " + TimeToSpawnHead + " seconds, up to a maximum of " + MaximumHeadCount + " heads\n" +
                            "Left-click to command any existing heads to fire\n" +
                            "Right-click to launch the heads for powerful explosive damage");
            SacrificeTotal = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 66;
            Item.height = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.damage = 44;
            Item.DamageType = DamageClass.Ranged;
            Item.useAnimation = Item.useTime = 66;
            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ModContent.ProjectileType<HydrasBlood>();
            Item.shootSpeed = 10f;
            Item.knockBack = 10f;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = null; //does so in Shoot
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => player.altFunctionUse == 2 ? false : true;

        public override void UpdateInventory(Player player)
        {
            //Spawn heads while holding
            if (player.ActiveItem().type == Item.type && player.ownedProjectileCounts[HeadID] < MaximumHeadCount)
            {
                HeadSpawnTimer++;
                if (HeadSpawnTimer >= TimeToSpawnHead * 60)
                {
                    HeadSpawnTimer = 0;
                    SoundEngine.PlaySound(SpawnSound, player.Center);
                    Projectile.NewProjectile(Item.GetSource_FromThis(), player.Top + Vector2.UnitY * 8f, Vector2.Zero, HeadID, 0, 0, player.whoAmI);
                }
            }    
            else if (player.ActiveItem().type != Item.type || player.dead || !player.active)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    //Destroy all heads
                    if (Main.projectile[i].type == HeadID && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].active)
                        Main.projectile[i].Kill();
                }
                HeadSpawnTimer = 0;
            }
        }

        public override bool AltFunctionUse(Player player)
        {
            //Does nothing if there's no heads
            if (player.ownedProjectileCounts[HeadID] <= 0)
                return false;

            //Otherwise, launch the guns
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                SoundEngine.PlaySound(LaunchSound, player.Center);
                if (Main.projectile[i].type == HeadID && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].active)
                    Main.projectile[i].ai[1] = -1f;
            }
            return false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 shootDirection = velocity.SafeNormalize(Vector2.Zero);
            for (int i = 0; i < 4; i++)
            {
                int CurrentHeadCount = player.ownedProjectileCounts[HeadID];
                //Exponentially louder the more heads, slightly
                SoundStyle FireSound = new("CalamityMod/Sounds/Item/Hydra") { Volume = 0.3f + (float)(Math.Pow(CurrentHeadCount, 1.5D) * 0.2f) };
                SoundEngine.PlaySound(FireSound, player.Center);

                Vector2 spreadDirection = shootDirection.RotatedByRandom(MathHelper.ToRadians(ShotSpread / 2f));
                float spreadVelocity = Item.shootSpeed * Main.rand.NextFloat(1f, 1.4f);
                Vector2 newPos = player.MountedCenter + shootDirection * 50f;

                Projectile.NewProjectile(source, newPos, spreadVelocity * spreadDirection, Item.shoot, damage, knockback, player.whoAmI);
            }

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                //Force all heads to shoot
                if (Main.projectile[i].type == HeadID && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].active)
                    Main.projectile[i].ai[1] = 1f;
            }
            return false;
        }

        public override void HoldItem(Player player) => player.Calamity().mouseWorldListener = true;

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.direction = Math.Sign((player.Calamity().mouseWorld - player.Center).X);
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 7f;
            Vector2 itemSize = new Vector2(Item.width, Item.height);
            Vector2 itemOrigin = new Vector2(-17, 3);
            CalamityUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin);
            base.UseStyle(player, heldItemFrame);
        }

        public override void UseItemFrame(Player player)
        {
            player.direction = Math.Sign((player.Calamity().mouseWorld - player.Center).X);

            float animProgress = 1 - player.itemTime / (float)player.itemTimeMax;
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
            if (animProgress < 0.4f)
                rotation += -0.45f * (float)Math.Pow((0.4f - animProgress) / 0.4f, 2) * player.direction;

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.QuadBarrelShotgun).
                AddIngredient(ItemID.Nanites, 25).
                AddIngredient(ItemID.VialofVenom, 25).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
